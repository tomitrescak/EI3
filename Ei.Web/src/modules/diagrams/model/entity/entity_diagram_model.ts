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
              if (e.link.getSourcePort().getName() === "top") {
                let sourcePort = e.link.getSourcePort();
                e.link.setSourcePort(e.link.getTargetPort());
                e.link.setTargetPort(sourcePort);
              }
              const fromPort = e.link.getSourcePort() as EntityPortModel;
              const toPort = e.link.getTargetPort() as EntityPortModel;
              ////////////////////////////////////
              // we do not allow multiple inheritance
              if (toPort.linkArray.length > 1) {
                link.safeRemove(this);
                return;
              }
              /////////////////////////////////////
              // we cannot join top or bottoms
              if (fromPort.getPosition() === toPort.getPosition()) {
                link.safeRemove(this);
                return;
              }
              ///////////////////////////////////
              // we cannot create loops
              let parentNode = fromPort.getParent() as HierarchicEntity;
              let parentNodeTopPort = parentNode.getPort(
                "top"
              ) as EntityPortModel;
              let childNode = toPort.getParent() as HierarchicEntity;
              let childNodeBottomPort = childNode.getPort(
                "bottom"
              ) as EntityPortModel;
              let parentLinks = parentNodeTopPort.linkArray;
              if (
                parentLinks.length &&
                parentLinks[0].getSourcePort() === childNodeBottomPort
              ) {
                link.safeRemove(this);
                return;
              }
              childNode.setParentId(parentNode.Id);
              // childNode.parentLink = link;
            },
          } as any);
        }
      },
    } as DiagramListener);
  }
}
