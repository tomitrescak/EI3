import { DiagramModel } from 'storm-react-diagrams';

import { observable } from 'mobx';
import { HierarchicEntity } from '../../../ei/hierarchic_entity_model';
import { EntityLinkModel } from './entity_link_model';
import { EntityPortModel } from './entity_port_model';

export class EntityDiagramModel extends DiagramModel {
  store: App.Store;
  @observable version = 0;

  constructor() {
    super();

    this.addListener({
      nodesUpdated: e => {
        // node was deleted, remove it from the collection
        let node = e.node as HierarchicEntity;
        if (!e.isCreated) {
          node.remove();
        }
      },
      linksUpdated: e => {
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
            document.removeEventListener('mouseup', checkConnection);
          };
          document.addEventListener('mouseup', checkConnection);

          e.link.addListener({
            targetPortChanged: () => {

              /////////////////////////////////////////////
              // links can only be created parent to child
              if (e.link.sourcePort.name === 'top') {
                let sourcePort = e.link.sourcePort;
                e.link.sourcePort = e.link.targetPort;
                e.link.targetPort = sourcePort;
              }

              const fromPort = e.link.sourcePort as EntityPortModel;
              const toPort = e.link.targetPort as EntityPortModel;

              ////////////////////////////////////
              // we do not allow multiple inheritance
              if (toPort.linkArray.length > 1) {
                link.safeRemove(this);
                return;
              }

              /////////////////////////////////////
              // we cannot join top or bottoms
              if (fromPort.position === toPort.position) {
                link.safeRemove(this);
                return;
              }

              ///////////////////////////////////
              // we cannot create loops
              let parentNode = fromPort.getParent() as HierarchicEntity;
              let parentNodeTopPort = parentNode.getPort('top') as EntityPortModel;
              let childNode = toPort.getParent() as HierarchicEntity;
              let childNodeBottomPort = childNode.getPort('bottom') as EntityPortModel;
              let parentLinks = parentNodeTopPort.linkArray;

              if (parentLinks.length && parentLinks[0].sourcePort === childNodeBottomPort) {
                link.safeRemove(this);
                return;
              }

              childNode.setParent(parentNode.Id);
              childNode.parentLink = link;
            }
          } as any);
        }
      }
    });
  }
}
