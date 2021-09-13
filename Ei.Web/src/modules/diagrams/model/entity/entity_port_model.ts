import * as _ from "lodash";
import * as SRD from "@projectstorm/react-diagrams";
import { EntityLinkModel } from "./entity_link_model";
import { PortModelAlignment } from "@projectstorm/react-diagrams";

export class EntityPortModel extends SRD.PortModel {
  // position: string | "top" | "bottom" | "left" | "right";

  constructor(name: string, alignment: PortModelAlignment) {
    super({
      type: "entity",
      name,
      alignment,
    }); // isIn, pos, label
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
    let linkModel = new EntityLinkModel();
    linkModel.setSourcePort(this);
    return linkModel;
  }

  get linkArray() {
    const keys = Object.keys(this.links);
    return keys.map((k) => this.links[k]);
  }
}
