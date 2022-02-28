import { IObservableArray, makeObservable, observable } from "mobx";

import { Group } from "./group_model";
import {
  ParametricEntity,
  ParametricEntityDao,
} from "./parametric_entity_model";

// #region ############### Action ####################
export interface ActionDao extends ParametricEntityDao {
  $type?: string;
}

export class Action extends ParametricEntity {
  $type = "";

  constructor(action: Partial<ActionDao>) {
    super(action);

    this.Icon = "";
    this.$type = action.$type;
  }

  get json(): ActionDao {
    return {
      ...super.json,
    };
  }
}
// #endregion

// #region ############### ActionJoinWorkflow ####################
export interface ActionJoinWorkflowDao extends ActionDao {
  WorkflowId: string;
}

export class ActionJoinWorkflow extends Action {
  WorkflowId: string;

  constructor(action: Partial<ActionJoinWorkflowDao>) {
    super(action);

    this.Icon = "📁";
    this.WorkflowId = action.WorkflowId;

    this.$type = "ActionJoinWorkflowDao";

    makeObservable(this, {
      WorkflowId: observable,
    });
  }

  get json() {
    return {
      $type: "ActionJoinWorkflowDao",
      ...super.json,
      WorkflowId: this.WorkflowId,
    };
  }
}
// #endregion

// #region ############### ActionMessage ####################
export interface ActionMessageDao extends ActionDao {
  NotifyAgents: string[];
  NotifyGroups: Group[];
}

export class ActionMessage extends Action {
  NotifyAgents: IObservableArray<string>;
  NotifyGroups: IObservableArray<Group>;

  constructor(action: Partial<ActionMessageDao>) {
    super(action);

    this.Icon = "✉️";
    this.NotifyAgents = observable(action.NotifyAgents || []);
    this.NotifyGroups = observable(
      (action.NotifyGroups || []).map((g) => new Group(g))
    );

    this.$type = "ActionMessageDao";
  }

  get json() {
    return {
      $type: "ActionMessageDao",
      ...super.json,
      NotifyAgents: this.NotifyAgents.map((a) => a),
      NotifyGroups: this.NotifyGroups.map((g) => g.json),
    };
  }
}
// #endregion

// #region ############### ActionTimeout ####################
export class ActionTimeout extends Action {
  constructor(action: Partial<ActionDao>) {
    super(action);

    this.Icon = "🕐";
    this.$type = "ActionTimeoutDao";
  }

  get json() {
    return {
      $type: "ActionTimeoutDao",
      ...super.json,
    };
  }
}
// #endregion
