import { LinkModel } from 'storm-react-diagrams';

import { action } from 'mobx';

import { HierarchicEntity } from '../../../ei/hierarchic_entity_model';
import { EntityDiagramModel } from './entity_diagram_model';

export class EntityLinkModel extends LinkModel {

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
    if (model.links[this.id]) {
      model.removeLink(this);
    }
  }

  @action safeRemoveParent() {
    if (this.targetPort) {
      let port = this.targetPort.name === 'top' ? this.targetPort : this.sourcePort;
      let node = port.parentNode as HierarchicEntity;
      node.setParent(null);
      node.parentLink = null;
    }
  }

  validateCreate(model: EntityDiagramModel) {
    if (this.targetPort == null) {
      this.safeRemove(model);
      model.version ++;
    }
  }
}