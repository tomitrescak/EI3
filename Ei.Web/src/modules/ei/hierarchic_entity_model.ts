import { action, IObservableArray, observable } from 'mobx';
import { field } from 'semantic-ui-mobx';
import { PointModel } from 'storm-react-diagrams';

import { Ui } from '../../helpers/client_helpers';
import { EntityLinkModel } from '../diagrams/model/entity/entity_link_model';
import { EntityPortModel } from '../diagrams/model/entity/entity_port_model';
import { Ei } from './ei_model';
import { ParametricEntity, ParametricEntityDao } from './parametric_entity_model';

export interface PointDao {
  x: number;
  y: number;
}

export interface HierarchicEntityDao extends ParametricEntityDao {
  Parent?: string;
  LinkPoints?: PointDao[];
}

export abstract class HierarchicEntity extends ParametricEntity {
  @observable selected: boolean;
  ei: Ei;

  @field private _parent: string;
  private _parentLink: EntityLinkModel;
  private parents: IObservableArray<HierarchicEntity>;
  private points: PointDao[];

  constructor(
    model: HierarchicEntityDao,
    parents: IObservableArray<HierarchicEntity>,
    ei: Ei,
    allowEditIcon = false
  ) {
    super(model, allowEditIcon);

    this.ei = ei;
    this.parents = parents;
    this.points = model && model.LinkPoints;

    // add ports
    this.addPort(new EntityPortModel('top'));
    this.addPort(new EntityPortModel('bottom'));

    // add listeners
    this.addListener({
      selectionChanged: ({ isSelected }) => {
        isSelected ? this.select() : this.deselect();
      }
    });

    if (model) {
      this.Parent = model.Parent;

      // if there is a panel, create a link
      this.update();
    }
  }

  update() {
    if (this.Parent) {
      // create links
      this.parentLink = new EntityLinkModel();

      // create points
      if (this.points && this.points.length) {
        this.parentLink.setPoints(this.points.map(p => new PointModel(this.parentLink, p)));
      }

      // add ports
      const parent = this.parents.find(p => p.Id === this.Parent);
      this.parentLink.setSourcePort(parent.getPort('bottom'));
      this.parentLink.setTargetPort(this.getPort('top'));
    } else {
      this.parentLink = null;
    }
  }

  // setters
  get parentLink() {
    return this._parentLink;
  }

  set parentLink(link: EntityLinkModel) {
    // Ui.history.assignValue(this, 'parentLink', link);
    this._parentLink = link;
  }

  get Parent() {
    return this._parent;
  }

  set Parent(parent: string) {
    this._parent = parent;
    this.update();
  }

  @action
  removeItem() {
    // remove from collection
    this.Parent = null;
    this.parents.remove(this);

    // adjust all children
    for (let entity of this.parents) {
      if (entity.Parent === this.Id) {
        entity.Parent = null;
        entity.parentLink = null;
      }
    }

    Ui.history.step();
  }

  async remove(): Promise<void> {
    if (
      this.parents.length === 1 &&
      (this.constructor.name === 'Role' || this.constructor.name === 'Organisation')
    ) {
      Ui.alertDialog('You cannot remove the last item. Institution needs to contain at least one.');
      return;
    }
    if (
      await Ui.confirmDialogAsync(
        'Do you want to delete this record? This can break your existing references!',
        'Deleting record'
      )
    ) {
      this.removeItem();
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
  deselect() {
    /**/
  }
}
