import { action, IObservableArray, makeObservable, observable } from "mobx";

import { Router, Ui } from "../../helpers/client_helpers";
import { Ei } from "./ei_model";
import { EntityDao } from "./entity_model";
import { PositionModel } from "./position_model";
import { Workflow } from "./workflow_model";

export interface TransitionDao extends EntityDao {
  $type?: string;
  Vertical: boolean;
}

export const transitionHeight = 20;
export const transitionWidth = 80;

export class Transition extends PositionModel {
  $type: string;
  Vertical: boolean;

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);
    this.Icon = "chevron right";
    this.$type = transition.$type;

    this.Vertical = transition.Vertical;
    makeObservable(this, {
      Vertical: observable,
    });
  }

  get url() {
    return this.ei
      .createWorkflowUrl(this.workflow, "transition", this.Id)
      .toLowerCase();
  }

  get labelSize() {
    let labelSize = (this.Name || this.Id).length * 8 + 8;
    labelSize = labelSize < transitionWidth ? transitionWidth : labelSize;
    return labelSize;
  }

  get json() {
    return {
      ...super.json,
      Vertical: this.Vertical,
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
  Shallow: boolean;
  Names: IObservableArray<SplitInfo>;
  uid = id++;

  ports = {
    input: () => ({
      orientation: this.Vertical ? "west" : "north",
      x: !this.Vertical
        ? this.labelSize / 2
        : this.labelSize / 2 - transitionHeight / 2,
      y: !this.Vertical ? -5 : transitionHeight / 2,
    }),
    split1: () => ({
      orientation: this.Vertical ? "east" : "south",
      x: !this.Vertical ? 5 : this.labelSize / 2 + transitionHeight / 2,
      y: !this.Vertical
        ? transitionHeight
        : transitionHeight / 2 - this.labelSize / 2,
    }),
    split2: () => ({
      orientation: this.Vertical ? "east" : "south",
      x: !this.Vertical
        ? this.labelSize / 2
        : this.labelSize / 2 + transitionHeight / 2,
      y: !this.Vertical ? transitionHeight : transitionHeight / 2,
    }),
    split3: () => ({
      orientation: this.Vertical ? "east" : "south",
      x: !this.Vertical
        ? this.labelSize - 5
        : this.labelSize / 2 + transitionHeight / 2,
      y: !this.Vertical
        ? transitionHeight
        : this.labelSize / 2 + transitionHeight / 2,
    }),
  };

  constructor(
    transition: Partial<TransitionSplitDao>,
    workflow: Workflow,
    ei: Ei
  ) {
    super(transition, workflow, ei);

    this.Icon = "⑃";

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
  ports = {
    join1: () => ({
      orientation: this.Vertical ? "west" : "north",
      x: this.Vertical ? this.labelSize / 2 - transitionHeight / 2 : 6,
      y: this.Vertical ? transitionHeight / 2 - this.labelSize / 2 + 6 : 0,
    }),
    join2: () => ({
      orientation: this.Vertical ? "west" : "north",
      x: this.Vertical
        ? this.labelSize / 2 - transitionHeight / 2
        : this.labelSize / 2,
      y: this.Vertical ? transitionHeight / 2 : 0,
    }),
    join3: () => ({
      orientation: this.Vertical ? "west" : "north",
      x: this.Vertical
        ? this.labelSize / 2 - transitionHeight / 2
        : this.labelSize - 6,
      y: this.Vertical ? transitionHeight / 2 + this.labelSize / 2 - 6 : 0,
    }),
    yield: () => ({
      orientation: this.Vertical ? "east" : "south",
      x: this.Vertical
        ? this.labelSize / 2 + transitionHeight / 2
        : this.labelSize / 2,
      y: this.Vertical ? transitionHeight / 2 : transitionHeight,
    }),
  };

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);

    this.Icon = "⑂";

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
