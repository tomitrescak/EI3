import * as _ from 'lodash';
import * as SRD from 'storm-react-diagrams';
import { EntityLinkModel } from './entity_link_model';

export class EntityPortModel extends SRD.PortModel {
	position: string | 'top' | 'bottom' | 'left' | 'right';

	constructor(pos: string = 'top') {
		super(pos);
		this.position = pos;
		this.name = pos;
	}

	serialize() {
		return _.merge(super.serialize(), {
			position: this.position
		});
	}

	deSerialize(data: any) {
		super.deSerialize(data);
		this.position = data.position;
	}

	createLinkModel() {
		let linkModel = new EntityLinkModel();
		linkModel.setSourcePort(this);
		return linkModel;
	}

	get linkArray() {
		const keys = Object.keys(this.links);
		return keys.map(k => this.links[k]);
	}
}