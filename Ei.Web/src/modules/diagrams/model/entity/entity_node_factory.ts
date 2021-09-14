import * as SRD from "@projectstorm/react-diagrams-defaults";
import { DefaultNodeWidget } from "@projectstorm/react-diagrams-defaults";
import React from "react";

import { HierarchicEntity } from "../../../ei/hierarchic_entity_model";
import { EntityNodeWidgetFactory } from "./entity_node_widget";

export class EntityNodeFactory extends SRD.DefaultNodeFactory {
  constructor() {
    super();
  }

  generateReactWidget(e: { model: HierarchicEntity }): JSX.Element {
    if (e.model instanceof HierarchicEntity) {
      return EntityNodeWidgetFactory({ node: e.model });
    } else {
      return React.createElement(DefaultNodeWidget, {
        node: e.model,
        engine: this.engine,
      });
    }
  }

  getNewInstance(): null {
    return null;
  }
}
