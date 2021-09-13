import { DefaultLinkModel } from "@projectstorm/react-diagrams";

import { action } from "mobx";

import { HierarchicEntity } from "../../../ei/hierarchic_entity_model";
import { EntityDiagramModel } from "./entity_diagram_model";

export class EntityLinkModel extends DefaultLinkModel {
  // constructor(child: HierarchicEntity, parent: string) {
  //   super('default', parent + '->' + child.Id);
  // }

  @action safeRemove(model: EntityDiagramModel) {
    this.safeRemoveParent();

    if (this.sourcePort) {
      this.sourcePort.removeLink(this);
    }
    if (this.targetPort) {
      this.targetPort.removeLink(this);
    }
    if (model.getLinks()[this.getID()]) {
      model.removeLink(this);
    }
  }

  @action safeRemoveParent() {
    if (this.targetPort) {
      let port =
        this.targetPort.getName() === "top" ? this.targetPort : this.sourcePort;
      let node = port.getParent() as HierarchicEntity;
      node.setParentId(null);
      node.parentLink = null;
    }
  }

  validateCreate(model: EntityDiagramModel) {
    if (this.targetPort == null) {
      this.safeRemove(model);
      model.version++;
    }
  }
}
