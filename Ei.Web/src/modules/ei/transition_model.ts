import { action, IObservableArray, makeObservable, observable } from "mobx";

import { Router, Ui } from "../../helpers/client_helpers";
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
  Horizontal: boolean;
  selected: boolean;

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);
    this.$type = transition.$type;

    this.Horizontal = transition.Horizontal;
    makeObservable(this, {
      Horizontal: observable,
      selected: observable,
    });
  }

  get json() {
    return {
      ...super.json,
      Horizontal: this.Horizontal,
      removeItem: action,
    };
  }

  removeItem() {
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
    Router.push(
      this.ei.createWorkflowUrl(this.workflow, "transition", this.Id)
    );
  }
}

export interface TransitionSplitDao extends TransitionDao {
  Shallow: boolean;
  Names: string[][];
}

export class SplitInfo {
  stateId: string;
  name: string;

  constructor(stateId: string, name: string) {
    this.stateId = stateId;
    this.name = name;

    makeObservable(this, {
      name: observable,
    });
  }
}

let id = 0;

export class TransitionSplit extends Transition {
  Icon = "⑃";

  Shallow: boolean;
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

    // this.addPort(
    //   new WorkflowPortModel(workflow, "input", PortModelAlignment.TOP)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "split1", PortModelAlignment.BOTTOM)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "split2", PortModelAlignment.BOTTOM)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "split3", PortModelAlignment.BOTTOM)
    // );

    this.$type = "TransitionSplitDao";

    makeObservable(this, {
      Shallow: observable,
    });
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

    // this.addPort(
    //   new WorkflowPortModel(workflow, "yield", PortModelAlignment.BOTTOM)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "join1", PortModelAlignment.TOP)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "join2", PortModelAlignment.TOP)
    // );
    // this.addPort(
    //   new WorkflowPortModel(workflow, "join3", PortModelAlignment.TOP)
    // );

    this.$type = "TransitionJoinDao";
  }

  get json() {
    return {
      $type: "TransitionJoinDao",
      ...super.json,
    };
  }
}
