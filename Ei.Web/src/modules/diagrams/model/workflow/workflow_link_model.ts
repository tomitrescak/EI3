import { LinkModel, PointModel } from "@projectstorm/react-diagrams";

import { action, computed, IObservableArray, observable } from "mobx";

import { Connection } from "../../../ei/connection_model";
import { Workflow } from "../../../ei/workflow_model";
import { WorkflowDiagramModel } from "./workflow_diagram_model";

export class WorkflowLinkModel extends LinkModel {
  @observable selected = false;
  connection: Connection;
  workflow: Workflow;
  model: WorkflowDiagramModel;

  _points: any;

  get points(): IObservableArray<PointModel> {
    return this._points;
  }

  set points(value) {
    this._points = value;
  }

  constructor(connection: Connection, workflow: Workflow) {
    super("default", connection.Id);
    this.connection = connection;
    this.workflow = workflow;

    this.registerListener({
      selectionChanged: ({ isSelected }) => {
        isSelected ? this.select() : false;
      },
    });

    this.points = observable(this.points);
  }

  setPoints(points: PointModel[]) {
    if (!this.points) {
      this.points = observable([]);
    }
    this.points.replace(points);
  }

  select() {
    this.workflow.ei.context.viewStore.showConnection(
      this.workflow.Id,
      this.workflow.Name,
      this.connection.Id
    );
  }

  @computed get action() {
    if (this.connection.ActionId) {
      return this.workflow.Actions.find(
        (a) => a.Id === this.connection.ActionId
      );
    }
    return null;
  }

  @action safeRemove(model: WorkflowDiagramModel) {
    this.safeRemoveLink();

    if (this.sourcePort) {
      this.sourcePort.removeLink(this);
    }
    if (this.targetPort) {
      this.targetPort.removeLink(this);
    }
    model.removeLink(this);
  }

  @action safeRemoveLink() {
    this.workflow.Connections.remove(this.connection);
    this.workflow.ei.context.viewStore.showWorkflow(
      this.workflow.Id,
      this.workflow.Name
    );

    // remove split info
    this.connection.checkSplit(true);
    // if (this.targetPort) {
    //   let port = this.targetPort.name === 'top' ? this.targetPort : this.sourcePort;
    //   let node = port.parentNode as HierarchicEntity;
    //   node.Parent = null;
    //   node.parentLink = null;
    // }
  }

  validateCreate(model: WorkflowDiagramModel) {
    if (this.targetPort == null) {
      this.safeRemove(model);
      model.version++;
    }
  }
}
