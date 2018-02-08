import { IObservableArray, observable } from 'mobx';
import { field, intPositiveValidator } from 'semantic-ui-mobx';

import { WorkflowPortModel } from '../diagrams/model/workflow/workflow_port_model';
import { AccessCondition, AccessConditionDao } from './access_model';
import { Ei } from './ei_model';
import { EntityDao } from './entity_model';
import { PositionModel } from './position_model';
import { Workflow } from './workflow_model';

export interface StateDao extends EntityDao {
  Open: boolean;
  Timeout: number;
  IsStart: boolean;
  IsEnd: boolean;
  ShowRules: boolean;
  EntryRules: AccessConditionDao[];
  ExitRules: AccessConditionDao[];
}

export class State extends PositionModel {
  Icon = '⚪️';

  @field Open: boolean;
  @field(intPositiveValidator) Timeout: number;
  @field IsStart: boolean;
  @field IsEnd: boolean;
  @field ShowRules: boolean;
  EntryRules: IObservableArray<AccessCondition>;
  ExitRules: IObservableArray<AccessCondition>;

  constructor(state: StateDao, workflow: Workflow, ei: Ei) {
    super(state, workflow, ei);

    this.Open = state.Open;
    this.Timeout = state.Timeout;
    this.IsStart = state.IsStart;
    this.IsEnd = state.IsEnd;
    this.EntryRules = observable((state.EntryRules || []).map(r => new AccessCondition(r)));
    this.ExitRules = observable((state.ExitRules || []).map(r => new AccessCondition(r)));
    this.ShowRules = state.ShowRules == null ? true : state.ShowRules;

    // add ports

    this.addPort(new WorkflowPortModel(workflow, 'east'));
    this.addPort(new WorkflowPortModel(workflow, 'west'));
    this.addPort(new WorkflowPortModel(workflow, 'north'));
    this.addPort(new WorkflowPortModel(workflow, 'south'));
    
    this.addPort(new WorkflowPortModel(workflow, 'northeast'));
    this.addPort(new WorkflowPortModel(workflow, 'southwest'));
    this.addPort(new WorkflowPortModel(workflow, 'northwest'));
		this.addPort(new WorkflowPortModel(workflow, 'southeast'));

  }

  get json(): StateDao {
    return {
      ...super.json,
      Open: this.Open,
      Timeout: this.Timeout,
      IsStart: this.IsStart,
      IsEnd: this.IsEnd,
      ShowRules: this.ShowRules,
      EntryRules: this.EntryRules.map(r => r.json),
      ExitRules: this.ExitRules.map(r => r.json)
    }
  }

  select() {
    this.ei.store.viewStore.showState(this.workflow.Id, this.workflow.Name, this.Id);
  }
}