import { action, IObservableArray, observable } from 'mobx';
import { field, intPositiveValidator } from 'semantic-ui-mobx';

import { Ui } from '../../helpers/client_helpers';
import { WorkflowPortModel } from '../diagrams/model/workflow/workflow_port_model';
import { AccessCondition, AccessConditionDao } from './access_model';
import { Ei } from './ei_model';
import { EntityDao } from './entity_model';
import { PositionModel } from './position_model';
import { Workflow } from './workflow_model';

export interface StateDao extends EntityDao {
  Open: boolean;
  Timeout?: number;
  IsStart: boolean;
  IsEnd: boolean;
  ShowRules?: boolean;
  EntryRules?: AccessConditionDao[];
  ExitRules?: AccessConditionDao[];
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

  workflow: Workflow;

  constructor(state: StateDao, workflow: Workflow, ei: Ei) {
    super(state, workflow, ei);

    this.Open = state.Open;
    this.Timeout = state.Timeout;
    this.IsStart = state.IsStart;
    this.IsEnd = state.IsEnd;
    this.EntryRules = observable((state.EntryRules || []).map(r => new AccessCondition(r)));
    this.ExitRules = observable((state.ExitRules || []).map(r => new AccessCondition(r)));
    this.ShowRules = state.ShowRules == null ? true : state.ShowRules;

    this.workflow = workflow;

    // add ports

    this.addPort(new WorkflowPortModel(workflow, 'east'));
    this.addPort(new WorkflowPortModel(workflow, 'west'));
    this.addPort(new WorkflowPortModel(workflow, 'north'));
    this.addPort(new WorkflowPortModel(workflow, 'south'));
    
    this.addPort(new WorkflowPortModel(workflow, 'northeast'));
    this.addPort(new WorkflowPortModel(workflow, 'southwest'));
    this.addPort(new WorkflowPortModel(workflow, 'northwest'));
    this.addPort(new WorkflowPortModel(workflow, 'southeast'));
    
    // this.addFormListener(() => Ui.history.step());
  }

  @action removeItem() {
    // adjust all children
    for (let connection of this.workflow.Connections) {
      if (connection.From === this.Id || connection.To === this.Id) {
        this.workflow.Connections.remove(connection)
      }
    }

    // remove from collection
    this.workflow.States.remove(this);

    Ui.history.step();
  }

  async remove(): Promise<void> {
    if (this.workflow.States.length === 1) {
      Ui.alertDialog('Workflow needs to contain at least one state!');
      return;
    }
    if (
      await Ui.confirmDialogAsync(
        'Do you want to delete this state? This will delete all its connections!',
        'Deleting state'
      )
    ) {
      this.removeItem();
    }
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