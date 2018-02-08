import { IObservableArray, observable } from 'mobx';
import { field, FormState } from 'semantic-ui-mobx';

import { WorkflowPortModel } from '../diagrams/model/workflow/workflow_port_model';
import { Ei } from './ei_model';
import { EntityDao } from './entity_model';
import { PositionModel } from './position_model';
import { Workflow } from './workflow_model';

export interface TransitionDao extends EntityDao {
  $type: string;
  Horizontal: boolean;
}

export class Transition extends PositionModel { 
  Icon = 'chevron right';
  $type: string;
  @field Horizontal: boolean;

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);
    this.$type = transition.$type;

    this.Horizontal = transition.Horizontal;
  }

  get json(): TransitionDao {
    return {
      ...super.json,
      $type: '',
      Horizontal: this.Horizontal
    }
  }

  select() {
    this.ei.store.viewStore.showTransition(this.workflow.Id, this.workflow.Name, this.Id);
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

export class TransitionSplit extends Transition {
  Icon = '⑃';

  @field Shallow: boolean;
  Names: IObservableArray<SplitInfo>;

  constructor(transition: Partial<TransitionSplitDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);

    this.Names = observable((transition.Names || []).map(n => new SplitInfo(n[0], n[1])));
    this.Shallow = transition.Shallow;

    this.addPort(new WorkflowPortModel(workflow, 'input'));
    this.addPort(new WorkflowPortModel(workflow, 'split1'));
    this.addPort(new WorkflowPortModel(workflow, 'split2'));
    this.addPort(new WorkflowPortModel(workflow, 'split3'));
  }

  get json() {
    return {
      ...super.json,
      $type: 'TransitionSplitDao',
      Names: this.Names.map(n => [ n.stateId, n.name ]),
      Shallow: this.Shallow
    }
  }
}

export class TransitionJoin extends Transition {
  Icon = '⑂';

  constructor(transition: Partial<TransitionDao>, workflow: Workflow, ei: Ei) {
    super(transition, workflow, ei);

    this.addPort(new WorkflowPortModel(workflow, 'yield'));
    this.addPort(new WorkflowPortModel(workflow, 'join1'));
    this.addPort(new WorkflowPortModel(workflow, 'join2'));
    this.addPort(new WorkflowPortModel(workflow, 'join3'));
  }

  get json() {
    return {
      ...super.json,
      $type: 'TransitionJoinDao',
    }
  }
}