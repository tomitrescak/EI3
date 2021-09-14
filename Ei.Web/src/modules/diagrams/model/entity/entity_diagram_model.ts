import { DiagramListener, DiagramModel } from "@projectstorm/react-diagrams";

import { observable } from "mobx";
import { AppContext } from "../../../../config/context";
import { HierarchicEntity } from "../../../ei/hierarchic_entity_model";
import { EntityLinkModel } from "./entity_link_model";
import { EntityPortModel } from "./entity_port_model";

export class EntityDiagramModel extends DiagramModel {
  store: AppContext;
  @observable version = 0;

  constructor() {
    super();

    this.registerListener({
      nodesUpdated: (e) => {
        // node was deleted, remove it from the collection
        let node = e.node as HierarchicEntity;
        if (!e.isCreated) {
          node.remove();
        }
      },
      linksUpdated: (e) => {
        const link = e.link as EntityLinkModel;
        // link deleted
        if (!e.isCreated) {
          // this.removeLink(link);
          link.safeRemove(this);
        }
        if (e.isCreated) {
          // validate on mouse up
          let checkConnection = () => {
            setTimeout(() => {
              link.validateCreate(this);
            }, 50);
            document.removeEventListener("mouseup", checkConnection);
          };
          document.addEventListener("mouseup", checkConnection);
          e.link.registerListener({
            targetPortChanged: () => {
              /////////////////////////////////////////////
              // links can only be created parent to child

              const fromPort = (
                e.link.getSourcePort().getName() === "bottom"
                  ? e.link.getSourcePort()
                  : e.link.getTargetPort()
              ) as EntityPortModel;
              const toPort = (
                e.link.getSourcePort().getName() === "top"
                  ? e.link.getSourcePort()
                  : e.link.getTargetPort()
              ) as EntityPortModel;

              if (fromPort.getName() === toPort.getName()) {
                this.removeLink(e.link);
                return;
              }

              // links cannot creat loops
              let child = toPort.getParent() as HierarchicEntity;
              let parent = fromPort.getParent() as HierarchicEntity;
              while (parent != null) {
                if (parent.Id === child.Id) {
                  link.safeRemove(this);
                  return;
                }
                parent = this.getNode(parent.ParentId) as HierarchicEntity;
              }
              child.setParentId((fromPort.getParent() as HierarchicEntity).Id);
              // childNode.parentLink = link;
            },
          } as any);
        }
      },
    } as DiagramListener);
  }
}
