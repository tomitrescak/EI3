import { PortModelAlignment } from "@projectstorm/react-diagrams-core";
import { action, IObservableArray, makeObservable, observable } from "mobx";

import { Router, Ui } from "../../helpers/client_helpers";
import { WorkflowPortModel } from "../diagrams/model/workflow/workflow_port_model";
import { AccessCondition, AccessConditionDao } from "./access_model";
import { Ei } from "./ei_model";
import { EntityDao } from "./entity_model";
import { PositionModel } from "./position_model";
import { Workflow } from "./workflow_model";

export interface StateDao extends EntityDao {
  IsOpen: boolean;
  Timeout?: number;
  IsStart: boolean;
  IsEnd: boolean;
  ShowRules?: boolean;
  EntryRules?: AccessConditionDao[];
  ExitRules?: AccessConditionDao[];
}

export class State extends PositionModel {
  IsOpen: boolean;
  Timeout: number;
  IsStart: boolean;
  IsEnd: boolean;
  ShowRules: boolean;
  EntryRules: IObservableArray<AccessCondition>;
  ExitRules: IObservableArray<AccessCondition>;

  constructor(state: StateDao, workflow: Workflow, ei: Ei) {
    super(state, workflow, ei);

    this.Icon = "⚪️";

    this.IsOpen = state.IsOpen;
    this.Timeout = state.Timeout;
    this.IsStart = state.IsStart;
    this.IsEnd = state.IsEnd;
    this.EntryRules = observable(
      (state.EntryRules || []).map((r) => new AccessCondition(r))
    );
    this.ExitRules = observable(
      (state.ExitRules || []).map((r) => new AccessCondition(r))
    );
    this.ShowRules = state.ShowRules == null ? true : state.ShowRules;

    this.workflow = workflow;

    // add ports

    this.addPort(
      new WorkflowPortModel(workflow, "east", PortModelAlignment.RIGHT)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "west", PortModelAlignment.LEFT)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "north", PortModelAlignment.TOP)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "south", PortModelAlignment.BOTTOM)
    );

    this.addPort(
      new WorkflowPortModel(workflow, "northeast", PortModelAlignment.RIGHT)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "southwest", PortModelAlignment.LEFT)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "northwest", PortModelAlignment.LEFT)
    );
    this.addPort(
      new WorkflowPortModel(workflow, "southeast", PortModelAlignment.RIGHT)
    );

    if (
      this.ei.context.Router.router.location.pathname.toLowerCase() ==
      this.url.toLowerCase()
    ) {
      this.setSelected(true);
      ei.context.selectedEntity = this;
    }

    // this.addFormListener(() => Ui.history.step());
    makeObservable(this, {
      IsOpen: observable,
      Timeout: observable,
      IsStart: observable,
      IsEnd: observable,
      ShowRules: observable,
      removeItem: action,
    });
  }

  removeItem() {
    // adjust all children
    for (let connection of this.workflow.Connections) {
      if (connection.From === this.Id || connection.To === this.Id) {
        this.workflow.Connections.remove(connection);
      }
    }

    // remove from collection
    this.workflow.States.remove(this);

    Ui.history.step();
  }

  async remove(): Promise<void> {
    if (this.workflow.States.length === 1) {
      Ui.alertDialog("Workflow needs to contain at least one state!");
      return;
    }
    if (
      await Ui.confirmDialogAsync(
        "Do you want to delete this state? This will delete all its connections!",
        "Deleting state"
      )
    ) {
      this.removeItem();
    }
  }

  get url() {
    return this.ei
      .createWorkflowUrl(this.workflow, "state", this.Id)
      .toLowerCase();
  }

  get json(): StateDao {
    return {
      ...super.json,
      IsOpen: this.IsOpen,
      Timeout: this.Timeout,
      IsStart: this.IsStart,
      IsEnd: this.IsEnd,
      ShowRules: this.ShowRules,
      EntryRules: this.EntryRules.map((r) => r.json),
      ExitRules: this.ExitRules.map((r) => r.json),
    };
  }

  select() {
    Router.push(this.url);
  }
}
