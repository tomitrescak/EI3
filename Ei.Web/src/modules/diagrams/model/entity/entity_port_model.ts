import * as _ from "lodash";
import * as SRD from "@projectstorm/react-diagrams";
import { EntityLinkModel } from "./entity_link_model";
import { PortModelAlignment } from "@projectstorm/react-diagrams";

export class EntityPortModel extends SRD.PortModel {
  constructor(name: string, alignment: PortModelAlignment) {
    super({
      type: "entity",
      name,
      alignment,
    });
  }

  createLinkModel() {
    return new EntityLinkModel();
  }

  get linkArray() {
    const keys = Object.keys(this.links);
    return keys.map((k) => this.links[k]);
  }
}
