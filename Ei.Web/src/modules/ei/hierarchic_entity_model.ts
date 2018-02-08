import { action, IObservableArray, observable } from 'mobx';
import { field } from 'semantic-ui-mobx';
import { PointModel } from 'storm-react-diagrams';

import { EntityLinkModel } from '../diagrams/model/entity/entity_link_model';
import { EntityPortModel } from '../diagrams/model/entity/entity_port_model';
import { Ei } from './ei_model';
import { ParametricEntity, ParametricEntityDao } from './parametric_entity_model';

export interface PointDao {
  x: number;
  y: number;
}

export interface HierarchicEntityDao extends ParametricEntityDao {
  Parent: string;
  LinkPoints: PointDao[];
}

export abstract class HierarchicEntity extends ParametricEntity {
  @field Parent: string;
  @observable selected: boolean;
  parentLink: EntityLinkModel;
  parents: IObservableArray<HierarchicEntity>;
  ei: Ei;

  constructor(model: HierarchicEntityDao, parents: IObservableArray<HierarchicEntity>, ei: Ei, allowEditIcon = false) {
    super(model, allowEditIcon);

    this.ei = ei;
    this.parents = parents;

    // add ports
    this.addPort(new EntityPortModel('top'));
		this.addPort(new EntityPortModel('bottom'));

    // add listeners
    this.addListener({
			selectionChanged: ({ isSelected }) => {
				isSelected ? this.select() : this.deselect();
			}
		})

    if (model) {
      this.Parent = model.Parent;

      // if there is a panel, create a link
      if (this.Parent) {
        // create links
        this.parentLink = new EntityLinkModel();
        
        // create points
        if (model.LinkPoints && model.LinkPoints.length) {
          this.parentLink.setPoints(model.LinkPoints.map(p => new PointModel(this.parentLink, p)));
        }

        // add ports
        const parent = parents.find(p => p.Id === this.Parent);
        this.parentLink.setSourcePort(parent.getPort('bottom'));
        this.parentLink.setTargetPort(this.getPort('top'));
      }
    }
  }

  @action remove() {
    // remove from collection
    this.parents.remove(this);

    // adjust all children
    for (let entity of this.parents) {
      if (entity.Parent === this.Id) {
        entity.Parent = null
        entity.parentLink = null;
      }
    }
  }

  get json(): HierarchicEntityDao {
    return {
      ...super.json,
      Parent: this.Parent,
      LinkPoints: this.parentLink ? this.parentLink.getPoints().map(p => ({ x: p.x, y: p.y })) : []
    };
  }

  abstract select(): void;
  deselect() { /**/ }
}