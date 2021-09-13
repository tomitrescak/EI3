import * as SRD from "@projectstorm/react-diagrams-defaults";

import { HierarchicEntity } from "../../../ei/hierarchic_entity_model";
import { EntityNodeWidgetFactory } from "./entity_node_widget";

export class EntityNodeFactory extends SRD.DefaultNodeFactory {
  constructor() {
    super();
  }

  generateReactWidget({ model }: { model: HierarchicEntity }): JSX.Element {
    if (model instanceof HierarchicEntity) {
      return EntityNodeWidgetFactory({ node: model });
    }
  }

  getNewInstance(): null {
    return null;
  }
}
