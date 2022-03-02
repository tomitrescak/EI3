import * as _ from "lodash";

import { Workflow } from "../../../ei/workflow_model";
import { WorkflowLinkModel } from "./workflow_link_model";

export class WorkflowPortModel {
  // position: string | 'left' | 'right';
  workflow: Workflow;

  constructor(workflow: Workflow, pos: string, alignment: any) {
    super({
      name: pos,
      alignment,
    });
    this.workflow = workflow;
  }

  serialize() {
    return _.merge(super.serialize(), {
      position: this.position,
    });
  }

  deSerialize(data: any) {
    super.deserialize(data);
    this.position = data.position;
  }

  createLinkModel() {
    // create a new connection
    const connection = this.workflow.createConnection("NoName");
    let linkModel = new WorkflowLinkModel(connection, this.workflow);

    connection.link = linkModel;
    linkModel.setSourcePort(this);

    return linkModel;
  }

  get linkArray() {
    const keys = Object.keys(this.links);
    return keys.map((k) => this.links[k]);
  }
}
