import * as React from 'react';

import { observer } from 'mobx-react';

import { StoreModel } from '../../config/store';
import { DiagramView } from '../diagrams/diagram_view';
import { EntityDiagramModel } from '../diagrams/model/entity/entity_diagram_model';
import { Ei } from '../ei/ei_model';
import { HierarchicEntity } from '../ei/hierarchic_entity_model';

interface Props {
  store: StoreModel;
  id: string;
  type: string;
  entities: (ei: Ei) => HierarchicEntity[];
}

@observer
export class EntitiesView extends React.Component<Props> {
  static displayName = 'EntitiesView';

  selectedNode: HierarchicEntity;
  
  entities(props: Props = this.props) {
    return props.entities(this.props.store.ei);
  }

  componentWillMount() {
    if (this.props.id) {
      this.selectedNode = this.entities().find(o => o.Id.toLowerCase() === this.props.id.toLowerCase()) as HierarchicEntity;
      this.selectedNode.setSelected(true);
    }
  }

  componentWillUpdate(nextProps: Props) {
    if (nextProps.id) {
      const nextNode = this.entities(nextProps).find(o => o.Id === nextProps.id) as HierarchicEntity;
      if (this.selectedNode) {
        this.selectedNode.setSelected(false);
      }
      if (nextNode) {
        nextNode.setSelected(true);
      }
      this.selectedNode = nextNode;
    }
  }

  render() {
    let model = new EntityDiagramModel();
    model.version;

    for (let node of this.entities()) {
      model.addNode(node);
      if (node.parentLink) {
        model.addLink(node.parentLink);
        
      }
      node.Parent; // subscribe
      
    }

    // listen and store offsets
    model.addListener({
      offsetUpdated: ({ offsetX, offsetY }) => {
        localStorage.setItem(`EntityDiagram.${this.props.type}.offsetX`, offsetX.toString());
        localStorage.setItem(`EntityDiagram.${this.props.type}.offsetY`, offsetY.toString());
      }
    });

    // set offsets
    const currentOffsetX = localStorage.getItem(`EntityDiagram.${this.props.type}.offsetX`);
    const currentOffsetY = localStorage.getItem(`EntityDiagram.${this.props.type}.offsetY`);
    if (currentOffsetX) {
      model.setOffset(parseInt(currentOffsetX, 10), parseInt(currentOffsetY, 10));
    }

    return <DiagramView engine={this.props.store.ei.engine} diagram={model} />;
  }
}

