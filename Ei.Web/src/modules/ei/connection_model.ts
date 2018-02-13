import { action, IObservableArray, observable } from 'mobx';
import { field } from 'semantic-ui-mobx';
import { NodeModel, PointModel } from 'storm-react-diagrams';

import { WorkflowLinkModel } from '../diagrams/model/workflow/workflow_link_model';
import { WorkflowPortModel } from '../diagrams/model/workflow/workflow_port_model';
import { AccessCondition, AccessConditionDao } from './access_model';
import { Ei } from './ei_model';
import { Entity, EntityDao } from './entity_model';
import { PointDao } from './hierarchic_entity_model';
import { SplitInfo, TransitionSplit } from './transition_model';
import { Workflow } from './workflow_model';

export interface ConnectionDao extends EntityDao {
  Join: string[];
  Access: AccessConditionDao[];
  Effects: AccessConditionDao[];
  ActionId: string;
  AllowLoops: number;

  LinkPoints: PointDao[];
  FreeFrom: PointDao;
  FreeTo: PointDao;
  SourcePort: string;
  TargetPort: string;
  RotateLabel: boolean;
  ActionDisplay: ActionDisplayType;
}

export class FreeJoint extends NodeModel {
  constructor(workflow: Workflow) {
    super('entity');

    this.addPort(new WorkflowPortModel(workflow, 'left'));
  }
}

export enum ActionDisplayType {
  IconOnly = 'iconOnly',
  IconAndText = 'iconAndText',
  Full = 'full'
}

export class Connection extends Entity {
  Icon = '➡';

  @field From: string;
  @field To: string;
  Access: IObservableArray<AccessCondition>;
  Effects: IObservableArray<AccessCondition>;
  @field ActionId: string;
  @field AllowLoops: number;

  @field RotateLabel: boolean;
  @field SourcePort: string;
  @field TargetPort: string;
  @field ActionDisplay: ActionDisplayType;

  link: WorkflowLinkModel;
  fromJoint: FreeJoint;
  toJoint: FreeJoint;

  workflow: Workflow;
  dao: Partial<ConnectionDao>;

  constructor(connection: Partial<ConnectionDao>, workflow: Workflow, _ei: Ei, createLink = true) {
    super(connection);

    this.From = connection.Join[0] || '';
    this.To = connection.Join[1] || '';

    this.Access = observable((connection.Access || []).map(a => new AccessCondition(a)));
    this.Effects = observable((connection.Effects || []).map(a => new AccessCondition(a)));
    this.ActionId = connection.ActionId;
    this.AllowLoops = connection.AllowLoops;
    this.RotateLabel = connection.RotateLabel;
    this.ActionDisplay = connection.ActionDisplay || ActionDisplayType.IconAndText;

    this.SourcePort = connection.SourcePort;
    this.TargetPort = connection.TargetPort;

    this.workflow = workflow;
    this.dao = connection;

    // create link if requested
    if (!createLink) {
      return;
    }

    this.link = new WorkflowLinkModel(this, workflow);
    if (connection.LinkPoints && connection.LinkPoints.length) {
      this.link.setPoints(connection.LinkPoints.map(p => new PointModel(this.link, p)));
    }

    this.update();
  }

  get fromElementType() {
    if (this.link == null) {
      return null;
    }
    let port = this.link.sourcePort;
    if (port == null) {
      return null;
    }
    let node = port.parentNode;
    if (node == null) {
      return null;
    }
    return node.constructor.name;
  }

  get toElementType() {
    if (this.link == null) {
      return null;
    }
    let port = this.link.targetPort;
    if (port == null) {
      return null;
    }
    let node = port.parentNode;
    if (node == null) {
      return null;
    }
    return node.constructor.name;
  }

  update() {
    const workflow = this.workflow;
    const connection = this.dao;
    const fromPosition = workflow.findPosition(this.From);
    const toPosition = workflow.findPosition(this.To);
    let random = { x: this.randomPosition(), y: this.randomPosition() };

    // free joints are displayed as separate nodes
    if (fromPosition) {
      if (this.link.sourcePort == null) {
        this.link.setSourcePort(fromPosition.getPort(connection.SourcePort || 'east'));
      } else {
        this.SourcePort = this.link.sourcePort.name;
      }
      this.fromJoint = null;
    } else {
      this.fromJoint = new FreeJoint(workflow);
      const to = toPosition ? { x: toPosition.x, y: toPosition.y } : random;
      const { x, y } = connection.FreeFrom ? connection.FreeFrom : { x: to.x - 60, y: to.y };
      this.fromJoint.setPosition(x, y);
      this.link.setSourcePort(this.fromJoint.getPort('left'));
    }
    if (toPosition) {
      if (this.link.targetPort == null) {
        this.link.setTargetPort(toPosition.getPort(connection.TargetPort || 'west'));
      } else {
        this.TargetPort = this.link.targetPort.name;
      }
      this.toJoint = null;
    } else {
      this.toJoint = new FreeJoint(workflow);
      const from = fromPosition ? { x: fromPosition.x, y: fromPosition.y } : random;
      const point = this.link.points[1];
      const { x, y } = connection.FreeTo
        ? connection.FreeTo
        : point ? point : { x: from.x + 80, y: from.y };
      this.toJoint.setPosition(x, y);
      this.link.setTargetPort(this.toJoint.getPort('left'));
    }

    // add extra points if this is self location
    if (this.To && this.To === this.From && this.link.points.length === 2) {
      const points = this.link.points;
      const p0 = points[0];
      const p1 = points[1];
      this.link.setPoints([
        p0,
        new PointModel(this.link, { x: p1.x + 60, y: p0.y - 50 }),
        new PointModel(this.link, { x: p0.x - 60, y: p0.y - 50 }),
        p1
      ]);
    }

    // check split
    this.checkSplit();
  }

  @action
  checkSplit(removed = false) {
    const fromPosition = this.workflow.findPosition(this.From);
    if (this.workflow.Connections && fromPosition && fromPosition.constructor.name === 'TransitionSplit') {
      const connections = this.workflow.Connections.concat(!removed ? [this] : []);
      const transitionSplit = fromPosition as TransitionSplit;
      // find all connections from split
      const splitConnections = connections.filter(c => c.From === fromPosition.Id);
      const names = transitionSplit.Names;

      // check added connections
      for (let con of splitConnections.filter(c => c.From === fromPosition.Id)) {
        if (!names.find(c => c.stateId === con.To)) {
          names.push(new SplitInfo(con.To, ''));
        }
      }

      // check missing connections
      for (let i = transitionSplit.Names.length - 1; i >= 0; i--) {
        if (!splitConnections.find(c => c.To === names[i].stateId)) {
          names.splice(i, 1);
        }
      }

      
    }
  }

  get json(): ConnectionDao {
    return {
      ...super.json,
      Id: this.Id,
      Join: [this.From, this.To],
      Access: this.Access.map(a => a.json),
      Effects: this.Effects.map(a => a.json),
      ActionId: this.ActionId,
      AllowLoops: this.AllowLoops,
      FreeFrom: this.fromJoint ? { x: this.fromJoint.x, y: this.fromJoint.y } : null,
      FreeTo: this.toJoint ? { x: this.toJoint.x, y: this.toJoint.y } : null,
      LinkPoints: this.link ? this.link.getPoints().map(p => ({ x: p.x, y: p.y })) : [],
      SourcePort: this.link.sourcePort.name,
      TargetPort: this.link.targetPort.name,
      RotateLabel: this.RotateLabel,
      ActionDisplay: this.ActionDisplay
    };
  }
}
