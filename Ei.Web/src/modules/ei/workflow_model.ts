import {
  action,
  computed,
  IObservableArray,
  makeObservable,
  observable,
} from "mobx";
import { field } from "semantic-ui-mobx";
import { DiagramModel } from "storm-react-diagrams";
import { AppContext } from "../../config/context";

import { Ui } from "../../helpers/client_helpers";
import { WorkflowLinkModel } from "../diagrams/model/workflow/workflow_link_model";
import { AccessCondition, AccessConditionDao } from "./access_model";
import {
  Action,
  ActionDao,
  ActionJoinWorkflow,
  ActionJoinWorkflowDao,
  ActionMessage,
  ActionMessageDao,
  ActionTimeout,
} from "./action_model";
import { Connection, ConnectionDao } from "./connection_model";
import { Ei } from "./ei_model";
import {
  ParametricEntity,
  ParametricEntityDao,
} from "./parametric_entity_model";
import { PositionModel } from "./position_model";
import { State, StateDao } from "./state_model";
import {
  Transition,
  TransitionDao,
  TransitionJoin,
  TransitionSplit,
  TransitionSplitDao,
} from "./transition_model";

// tslint:disable-next-line:no-empty-interface
export interface WorkflowDao extends ParametricEntityDao {
  Stateless: boolean;
  Static: boolean;
  States: StateDao[];
  Actions?: ActionDao[];
  Transitions?: TransitionDao[];
  Connections?: ConnectionDao[];
  AllowCreate?: AccessConditionDao[];
  AllowJoin?: AccessConditionDao[];
}

export function optionSort(a: any, b: any): number {
  return a.text.localeCompare(b.text, undefined, {
    numeric: true,
    sensitivity: "base",
  });
}

export class Workflow extends ParametricEntity {
  diagram: DiagramModel;
  ei: Ei;

  @field Stateless: boolean;
  @field Static: boolean;
  States: IObservableArray<State>;
  Actions: IObservableArray<Action>;
  Transitions: IObservableArray<Transition>;
  Connections: IObservableArray<Connection>;
  AllowCreate: IObservableArray<AccessCondition>;
  AllowJoin: IObservableArray<AccessCondition>;

  constructor(workflow: WorkflowDao, ei: Ei, private context: AppContext) {
    super(workflow);

    this.ei = ei;

    this.Stateless = workflow.Stateless;
    this.Static = workflow.Static;
    this.States = observable(
      (workflow.States || []).map((s) => new State(s, this, ei))
    );
    this.AllowCreate = observable(
      (workflow.AllowCreate || []).map((s) => new AccessCondition(s))
    );
    this.AllowJoin = observable(
      (workflow.AllowJoin || []).map((s) => new AccessCondition(s))
    );

    this.Transitions = observable(
      (workflow.Transitions || []).map((t) => {
        switch (t.$type) {
          case "TransitionSplitDao":
            return new TransitionSplit(t as TransitionSplitDao, this, ei);
          case "TransitionJoinDao":
            return new TransitionJoin(t as TransitionDao, this, ei);
          default:
            throw new Error("Not implemented: " + t.$type);
        }
      })
    );

    this.Actions = observable(
      (workflow.Actions || []).map((a) => {
        switch (a.$type) {
          case "ActionJoinWorkflowDao":
            return new ActionJoinWorkflow(a as ActionJoinWorkflowDao);
          case "ActionMessageDao":
            return new ActionMessage(a as ActionMessageDao);
          case "ActionTimeoutDao":
            return new ActionTimeout(a);
          default:
            throw new Error("Not implemented: " + a.$type);
        }
      })
    );

    this.Connections = observable(
      (workflow.Connections || []).map((s) => new Connection(s, this, ei))
    );

    makeObservable(this);
  }

  findPosition(id: string): PositionModel {
    if (!id) {
      return null;
    }
    let p: PositionModel = this.States.find((s) => s.Id === id);
    if (!p) {
      p = this.Transitions.find((t) => t.Id === id);
    }
    return p;
  }

  delete = async () => {
    if (
      await Ui.confirmDialogAsync(
        "Do you want to delete this workflow? This can break some references!",
        "Deleting workflow"
      )
    ) {
      action(() => {
        this.ei.Workflows.remove(this);
        this.ei.context.viewStore.showView("home");
      })();
    }

    Ui.history.step();
  };

  @computed
  get connectionOptions() {
    return [{ value: "", text: "None" }]
      .concat(
        this.States.map((c) => ({
          value: c.Id.toString(),
          text: c.Name || c.Id,
        })).concat(
          this.Transitions.map((c) => ({
            value: c.Id.toString(),
            text: c.Name || c.Id,
          }))
        )
      )
      .sort(optionSort);
  }

  @computed
  get actionOptions() {
    return [{ value: "", text: "None" }].concat(
      this.Actions.map((c) => ({
        value: c.Id.toString(),
        text: c.Name || c.Id,
      })).sort(optionSort)
    );
  }

  createAction = async (e: any) => {
    e.stopPropagation();

    const swal = require("sweetalert2");
    const { value: formValues } = await swal({
      title: "Creating a new action",
      html: `
        <div>
          <label><b>Action Type: </b></label>
          <select id="swal-select1" class="swal2-select">
            <option value="message">Message</option>
            <option value="join">Join Workflow</option>
            <option value="timeout">Timeout</option>
          </select>
        </div>
        <input id="swal-input2" class="swal2-input" placeholder="Action Name">`,
      focusConfirm: false,
      preConfirm: () => {
        return [
          (document.getElementById("swal-select1") as HTMLSelectElement).value,
          (document.getElementById("swal-input2") as HTMLInputElement).value,
        ];
      },
    });

    if (!formValues) {
      return;
    }

    const actionType: string = formValues[0];
    const actionName: string = formValues[1];
    const Id = actionName.toId();

    // check if action exists
    let m = this.Actions.find((a) => a.Id === Id);
    if (m) {
      this.context.Ui.alertDialog("Action with this Id already exists: " + Id);
      return;
    }

    const construct = { Id, Name: actionName };
    switch (actionType) {
      case "message":
        this.Actions.push(new ActionMessage(construct));
        break;
      case "join":
        this.Actions.push(new ActionJoinWorkflow(construct));
        break;
      case "timeout":
        this.Actions.push(new ActionTimeout(construct));
        break;
      default:
        throw new Error("Not Implemented: " + actionType);
    }

    this.ei.context.viewStore.showAction(this.Id, this.Name, Id);

    Ui.history.step();
  };

  createState = async (e: any) => {
    e.stopPropagation();

    let name = await Ui.promptText("Name of the new state?");
    if (name.value) {
      this.States.push(
        new State(
          { Name: name.value, Id: name.value.toId() } as any,
          this,
          this.ei
        )
      );
    }

    Ui.history.step();
  };

  createTransition = async (e: any) => {
    e.stopPropagation();

    const swal = require("sweetalert2");
    const { value: formValues } = await swal({
      title: "Creating a new transition",
      html: `
        <div>
          <label><b>Transition Type: </b></label>
          <select id="swal-select1" class="swal2-select">
            <option value="split">Split</option>
            <option value="join">Join</option>
          </select>
        </div>
        <input id="swal-input2" class="swal2-input" placeholder="Transition Name">`,
      focusConfirm: false,
      preConfirm: () => {
        return [
          (document.getElementById("swal-select1") as HTMLSelectElement).value,
          (document.getElementById("swal-input2") as HTMLInputElement).value,
        ];
      },
    });

    if (!formValues) {
      return;
    }

    const transitionType: string = formValues[0];
    const transitionName: string = formValues[1];
    const Id = transitionName.toId();

    // check if action exists
    let m = this.Transitions.find((a) => a.Id === Id);
    if (m) {
      this.context.Ui.alertDialog(
        "Transition with this Id already exists: " + Id
      );
      return;
    }

    const construct = { Id, Name: transitionName };
    switch (transitionType) {
      case "split":
        this.Transitions.push(new TransitionSplit(construct, this, this.ei));
        break;
      case "join":
        this.Transitions.push(new TransitionJoin(construct, this, this.ei));
        break;
      default:
        throw new Error("Not Implemented: " + transitionType);
    }

    this.ei.context.viewStore.showTransition(this.Id, this.Name, Id);

    Ui.history.step();
  };

  @action addConnection = (e: any) => {
    e.stopPropagation();

    const connection = this.createConnection();
    connection.link = new WorkflowLinkModel(connection, this);
    connection.update();

    this.Connections.push(connection);
  };

  createConnection = () => {
    let idx = 0;
    let id = "c" + idx;
    while (this.Connections.some((c) => c.Id === id)) {
      id = "c" + ++idx;
    }
    return new Connection({ Id: id, Join: [] }, this, this.ei, false);
  };

  get json(): WorkflowDao {
    return {
      ...super.json,
      Stateless: this.Stateless,
      Static: this.Static,
      States: this.States.map((s) => s.json),
      Actions: this.Actions.map((s) => s.json),
      Transitions: this.Transitions.map((s) => s.json),
      Connections: this.Connections.map((s) => s.json),
      AllowCreate: this.AllowCreate.map((s) => s.json),
      AllowJoin: this.AllowJoin.map((s) => s.json),
    };
  }
}
