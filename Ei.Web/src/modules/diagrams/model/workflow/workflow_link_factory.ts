import * as React from "react";
import * as SRD from "@projectstorm/react-diagrams";

import { WorkflowLink } from "./widget_link";
// import { WorkflowLinkModel } from "./workflow_link_model";

export class WorkflowLinkFactory extends SRD.DefaultLinkFactory {
  constructor(type: string) {
    super(type);
  }

  generateReactWidget({ model }: { model: SRD.DefaultLinkModel }): JSX.Element {
    return React.createElement(WorkflowLink, {
      link: model,
      diagramEngine: this.engine,
    });
  }

  getNewInstance(_initialConfig?: any): SRD.DefaultLinkModel {
    return null;
    // return new WorkflowLinkModel();
  }
}
