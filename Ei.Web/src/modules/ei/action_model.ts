import { IObservableArray, observable } from 'mobx';
import { field, FieldCollection } from 'semantic-ui-mobx';

import { Group } from './group_model';
import { ParametricEntity, ParametricEntityDao } from './parametric_entity_model';

// #region ############### Action ####################
export interface ActionDao extends ParametricEntityDao {
  $type: string;
}

export class Action extends ParametricEntity { 
  Icon = '';
  $type = '';

  constructor(action: Partial<ActionDao>) {
    super(action);
    this.$type = action.$type;
  }
}
// #endregion

// #region ############### ActionJoinWorkflow ####################
export interface ActionJoinWorkflowDao extends ActionDao {
  WorkflowId: string;
}

export class ActionJoinWorkflow extends Action { 
  @field WorkflowId: string;

  Icon = '📁';

  constructor(action: Partial<ActionJoinWorkflowDao>) {
    super(action);
    this.WorkflowId = action.WorkflowId;
  }

  get json() {
    return {
      $type: 'ActionJoinWorkflowDao',
      ...super.json,
      WorkflowId: this.WorkflowId
    }
  }
}
// #endregion

// #region ############### ActionMessage ####################
export interface ActionMessageDao extends ActionDao {
  NotifyAgents: string[];
  NotifyGroups: Group[];
}

export class ActionMessage extends Action { 
  NotifyAgents: FieldCollection<string>;
  NotifyGroups: IObservableArray<Group>;

  Icon = '✉️';

  constructor(action: Partial<ActionMessageDao>) {
    super(action);
    this.NotifyAgents = new FieldCollection((action.NotifyAgents || []));
    this.NotifyGroups = observable((action.NotifyGroups || []).map(g => new Group(g)));
  }

  get json() {
    return {
      $type: 'ActionMessageDao',
      ...super.json,
      NotifyAgents: this.NotifyAgents.array.map(a => a),
      NotifyGroups: this.NotifyGroups.map(g => g.json)
    }
  }
}
// #endregion

// #region ############### ActionTimeout ####################
export class ActionTimeout extends Action { 
  Icon = '🕐';

  get json() {
    return {
      $type: 'ActionTimeoutDao',
      ...super.json,
    }
  }
}
// #endregion