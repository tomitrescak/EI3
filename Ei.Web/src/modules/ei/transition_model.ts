import { action, IObservableArray, makeObservable, observable } from "mobx";
import { field, FormState } from "semantic-ui-mobx";

import { Ui } from "../../helpers/client_helpers";
import { WorkflowPortModel } from "../diagrams/model/workflow/workflow_port_model";
import { Ei } from "./ei_model";
import { EntityDao } from "./entity_model";
import { PositionModel } from "./position_model";
import { Workflow } from "./workflow_model";

export interface TransitionDao extends EntityDao {
  $type?: string;
  Horizontal: boolean;
}

export class Transition extends PositionModel {
  Icon = "chevron right";
  $type: string;
  @field Horizontal: boolean;

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);
    this.$type = transition.$type;

    this.Horizontal = transition.Horizontal;
    makeObservable(this);
  }

  get json() {
    return {
      ...super.json,
      Horizontal: this.Horizontal,
    };
  }

  @action removeItem() {
    // adjust all children
    for (let connection of this.workflow.Connections) {
      if (connection.From === this.Id || connection.To === this.Id) {
        this.workflow.Connections.remove(connection);
      }
    }
    // remove from collection
    this.workflow.Transitions.remove(this);
  }

  async remove(): Promise<void> {
    if (
      await Ui.confirmDialogAsync(
        "Do you want to delete this transition? This will delete all its connections!",
        "Deleting transition"
      )
    ) {
      this.removeItem();
    }
  }

  select() {
    this.ei.context.viewStore.showTransition(
      this.workflow.Id,
      this.workflow.Name,
      this.Id
    );
  }
}

export interface TransitionSplitDao extends TransitionDao {
  Shallow: boolean;
  Names: string[][];
}

export class SplitInfo extends FormState {
  stateId: string;
  @field name: string;

  constructor(stateId: string, name: string) {
    super();

    this.stateId = stateId;
    this.name = name;
  }
}

let id = 0;

export class TransitionSplit extends Transition {
  Icon = "⑃";

  @field Shallow: boolean;
  Names: IObservableArray<SplitInfo>;
  uid = id++;

  constructor(
    transition: Partial<TransitionSplitDao>,
    workflow: Workflow,
    ei: Ei
  ) {
    super(transition, workflow, ei);

    this.Names = observable(
      (transition.Names || []).map((n) => new SplitInfo(n[0], n[1]))
    );
    this.Shallow = transition.Shallow;

    this.addPort(new WorkflowPortModel(workflow, false, "input"));
    this.addPort(new WorkflowPortModel(workflow, false, "split1"));
    this.addPort(new WorkflowPortModel(workflow, false, "split2"));
    this.addPort(new WorkflowPortModel(workflow, false, "split3"));

    this.$type = "TransitionSplitDao";
  }

  get json() {
    return {
      $type: "TransitionSplitDao",
      ...super.json,
      Names: this.Names.map((n) => [n.stateId, n.name]),
      Shallow: this.Shallow,
    };
  }
}

export class TransitionJoin extends Transition {
  Icon = "⑂";

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);

    this.addPort(new WorkflowPortModel(workflow, false, "yield"));
    this.addPort(new WorkflowPortModel(workflow, false, "join1"));
    this.addPort(new WorkflowPortModel(workflow, false, "join2"));
    this.addPort(new WorkflowPortModel(workflow, false, "join3"));

    this.$type = "TransitionJoinDao";
  }

  get json() {
    return {
      $type: "TransitionJoinDao",
      ...super.json,
    };
  }
}
