import * as SRD from 'storm-react-diagrams';

import { State } from '../../../ei/state_model';
import { TransitionJoin, TransitionSplit } from '../../../ei/transition_model';
import { FreeWidgetFactory } from './widget_free';
import { StateWidgetFactory } from './widget_state';
import { TransitionJoinWidgetFactory } from './widget_transition_join';
import { TransitionSplitWidgetFactory } from './widget_transition_split';

export class WorkflowNodeFactory extends SRD.NodeFactory {
	constructor() {
		super('entity');
	}

	generateReactWidget(_diagramEngine: SRD.DiagramEngine, node: SRD.NodeModel): JSX.Element {
		if (node.constructor.name === 'State') {
			return StateWidgetFactory({ node: node as State });
		} else if (node.constructor.name === 'TransitionJoin') {
			return TransitionJoinWidgetFactory({ node: node as TransitionJoin });
		} else if (node.constructor.name === 'TransitionSplit') {
			return TransitionSplitWidgetFactory({ node: node as TransitionSplit });
		} else if (node.constructor.name === 'FreeJoint') {
			return FreeWidgetFactory({ node: node });
		} 
	}

	getNewInstance(): null {
		return null;
	}
}