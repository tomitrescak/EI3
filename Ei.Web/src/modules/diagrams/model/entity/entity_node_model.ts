// import * as SRD from 'storm-react-diagrams';

// import { observable } from 'mobx';

// import { HierarchicEntity } from '../../../ei/ei_model';
// import { FormNodeStore } from '../form_state_model';
// import { EntityPortModel } from './entity_port_model';

// let selectedNode: EntityNodeModel;

// export class EntityNodeModel extends FormNodeStore {
// 	public eiEntity: HierarchicEntity;
	
// 	@observable public selected: boolean = false;
	
// 	constructor(entity: HierarchicEntity, select: Function) {
// 		super(entity.Id);

// 		this.eiEntity = entity;
// 		this.addListener({
// 			selectionChanged: () => {
// 				select();
// 			}
// 		})

// 		this.addPort(new EntityPortModel('top'));
// 		this.addPort(new EntityPortModel('bottom'));
// 	}
// }