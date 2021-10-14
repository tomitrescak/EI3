import {
  DefaultLinkModel,
  DefaultLinkModelListener,
} from "@projectstorm/react-diagrams";

import { action, computed, makeObservable, observable } from "mobx";
import { Router } from "../../../../helpers/client_helpers";

import { Connection } from "../../../ei/connection_model";
import { Workflow } from "../../../ei/workflow_model";
import { WorkflowDiagramModel } from "./workflow_diagram_model";

export class WorkflowLinkModel extends DefaultLinkModel {
  @observable selected = false;
  connection: Connection;
  workflow: Workflow;
  model: WorkflowDiagramModel;

  constructor(connection: Connection, workflow: Workflow) {
    super({
      type: "default",
      id: connection.Id,
    });
    this.connection = connection;
    this.workflow = workflow;

    this.registerListener({
      selectionChanged: ({ isSelected }) => {
        isSelected ? this.select() : false;
      },
    } as DefaultLinkModelListener);

    makeObservable(this);
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
