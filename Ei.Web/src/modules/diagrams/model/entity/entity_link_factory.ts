import * as React from 'react';
import * as SRD from 'storm-react-diagrams';

import { EntityLinkModel } from './entity_link_model';

export class EntityLinkFactory extends SRD.LinkFactory {
	constructor(type: string) {
		super(type);
	}

	generateReactWidget(diagramEngine: SRD.DiagramEngine, link: SRD.LinkModel): JSX.Element {
		return React.createElement(SRD.DefaultLinkWidget, {
			link: link,
			diagramEngine: diagramEngine
		});
	}

	getNewInstance(_initialConfig?: any): SRD.LinkModel {
		return new EntityLinkModel();
	}
}