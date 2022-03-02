import {
  action,
  computed,
  IObservableArray,
  makeObservable,
  observable,
} from "mobx";
import { Router } from "../../../../helpers/client_helpers";

import { Connection } from "../../../ei/connection_model";
import { Workflow } from "../../../ei/workflow_model";
import { WorkflowDiagramModel } from "./workflow_diagram_model";
import type { Point } from "../diagram_common";

export class WorkflowLinkModel {
  id: string;
  selected = false;
  connection: Connection;
  workflow: Workflow;
  model: WorkflowDiagramModel;

  points: IObservableArray<Point> = observable([]);

  constructor(connection: Connection, workflow: Workflow) {
    this.id = connection.Id;
    this.connection = connection;
    this.workflow = workflow;

    makeObservable(this, {
      selected: observable,
      action: computed,
      safeRemove: action,
      safeRemoveLink: action,
    });
  }

  get url() {
    return this.workflow.ei
      .createWorkflowUrl(this.workflow, "connection", this.connection.Id)
      .toLowerCase();
  }

  select() {
    Router.push(
      this.workflow.ei.createWorkflowUrl(
        this.workflow,
        "connection",
        this.connection.Id
      )
    );
  }

  get action() {
    if (this.connection.ActionId) {
      return this.workflow.Actions.find(
        (a) => a.Id === this.connection.ActionId
      );
    }
    return null;
  }

  safeRemove(model: WorkflowDiagramModel) {
    this.safeRemoveLink();

    if (this.sourcePort) {
      this.sourcePort.removeLink(this);
    }
    if (this.targetPort) {
      this.targetPort.removeLink(this);
    }
    model.removeLink(this);
  }

  safeRemoveLink() {
    this.workflow.Connections.remove(this.connection);

    Router.push(this.workflow.ei.createWorkflowUrl(this.workflow));

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
