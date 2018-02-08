import * as React from 'react';
import * as SRD from 'storm-react-diagrams';

import { WorkflowLink } from './widget_link';

export class WorkflowLinkFactory extends SRD.LinkFactory {
	constructor(type: string) {
		super(type);
	}

	generateReactWidget(diagramEngine: SRD.DiagramEngine, link: SRD.LinkModel): JSX.Element {
		return React.createElement(WorkflowLink, {
			link: link,
			diagramEngine: diagramEngine
		});
	}

	getNewInstance(_initialConfig?: any): SRD.LinkModel {
		return null;
		// return new WorkflowLinkModel();
	}
}