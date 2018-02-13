import * as React from 'react';

import { DiagramEngine, DiagramWidget, LinkModel, PointModel } from 'storm-react-diagrams';

import { Entity } from '../ei/entity_model';
import { EntityDiagramModel } from './model/entity/entity_diagram_model';

interface Props {
  engine: DiagramEngine;
  diagram: EntityDiagramModel;
}

export class DiagramView extends React.Component<Props> {
  static displayName = 'DiagramView';

  onKeyUp = (event: any) => {
    // delete elements but only if we are not deleting stuff in inputs or textboxes 
    if (event.target.nodeName.toLowerCase() === 'body' && event.keyCode === 8 || event.keyCode === 46) {
      let selected = this.props.diagram.getSelectedItems();
      for (let element of selected) {
        // only delete items which are not locked
        if (!this.props.engine.isModelLocked(element)) {
          if (element instanceof Entity || element instanceof LinkModel || selected.every(e => e instanceof PointModel)) {
            element.remove();
          }
        }
      }
    }
  };

  componentDidMount() {
    window.addEventListener('keyup', this.onKeyUp, false);
  }

  componentWillUnmount() {
    window.removeEventListener('keyup', this.onKeyUp);
  }

  render() {
    this.props.engine.setDiagramModel(this.props.diagram);
    return <DiagramWidget deleteKeys={[]} diagramEngine={this.props.engine} />;
  }
}
