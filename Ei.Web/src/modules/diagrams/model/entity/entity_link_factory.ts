import * as React from "react";
import * as SRD from "@projectstorm/react-diagrams-defaults";

import { EntityLinkModel } from "./entity_link_model";
import { DefaultLinkModel } from "@projectstorm/react-diagrams";

export class EntityLinkFactory extends SRD.DefaultLinkFactory {
  constructor(type: string) {
    super(type);
  }

  generateReactWidget({ model }: { model: DefaultLinkModel }): JSX.Element {
    return React.createElement(SRD.DefaultLinkWidget, {
      link: model,
      diagramEngine: this.engine,
    });
  }

  generateModel() {
    debugger;
    return new EntityLinkModel();
  }

  getNewInstance(_initialConfig?: any): SRD.DefaultLinkModel {
    debugger;
    return new EntityLinkModel();
  }
}
