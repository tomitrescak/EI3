import * as SRD from 'storm-react-diagrams';

import { HierarchicEntity } from '../../../ei/hierarchic_entity_model';
import { EntityNodeWidgetFactory } from './entity_node_widget';

export class EntityNodeFactory extends SRD.NodeFactory {
	constructor() {
		super('entity');
	}

	generateReactWidget(_diagramEngine: SRD.DiagramEngine, node: SRD.NodeModel): JSX.Element {
		if (node instanceof HierarchicEntity) {
			return EntityNodeWidgetFactory({ node: node });
		}
	}

	getNewInstance(): null {
		return null;
	}
}