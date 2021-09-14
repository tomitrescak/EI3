import {
  action,
  computed,
  IObservableArray,
  makeObservable,
  observable,
} from "mobx";
import { field } from "semantic-ui-mobx";
import {
  NodeModelListener,
  PortModelAlignment,
} from "@projectstorm/react-diagrams";
import { Ui } from "../../helpers/client_helpers";
import { EntityLinkModel } from "../diagrams/model/entity/entity_link_model";
import { EntityPortModel } from "../diagrams/model/entity/entity_port_model";
import { Ei } from "./ei_model";
import {
  ParametricEntity,
  ParametricEntityDao,
} from "./parametric_entity_model";

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
  points: PointDao[];

  route: string;

  constructor(
    model: HierarchicEntityDao,
    route: string,
    parents: IObservableArray<HierarchicEntity>,
    ei: Ei,
    allowEditIcon = false
  ) {
    super(model, allowEditIcon);

    this.route = route;
    this.ei = ei;
    this.parents = parents;
    this.points = model && model.LinkPoints;

    // add ports
    this.addPort(new EntityPortModel("top", PortModelAlignment.TOP));
    this.addPort(new EntityPortModel("bottom", PortModelAlignment.BOTTOM));

    // add listeners
    this.registerListener({
      selectionChanged: ({ isSelected }) => {
        isSelected ? this.select() : this.deselect();
      },
    } as NodeModelListener);

    if (this.ei.context.Router.router.location.pathname == this.url) {
      this.setSelected(true);
      ei.context.selectedEntity = this;
    }

    if (model) {
      this.setParentId(model.Parent);

      // if there is a panel, create a link
      // this.update();
    }

    makeObservable(this);
  }

  set parentLink(value: EntityLinkModel) {
    this._parentLink = value;
  }

  @computed
  get ParentId() {
    return this._parent;
  }

  get url() {
    return `/ei/${this.ei.Name.toUrlName()}/${this.ei.Id}/${
      this.route
    }/${this.Name.toUrlName()}/${this.Id}`;
  }

  @action setParentId(parent: string) {
    this._parent = parent;
  }

  @action
  removeItem() {
    // remove from collection
    this.setParentId(null);
    this.parents.remove(this);

    // adjust all children
    for (let entity of this.parents) {
      if (entity.ParentId === this.Id) {
        entity.setParentId(null);
      }
    }

    Ui.history.step();
  }

  async remove(): Promise<void> {
    if (
      this.parents.length === 1 &&
      (this.constructor.name === "Role" ||
        this.constructor.name === "Organisation")
    ) {
      Ui.alertDialog(
        "You cannot remove the last item. Institution needs to contain at least one."
      );
      return;
    }
    if (
      await Ui.confirmDialogAsync(
        "Do you want to delete this record? This can break your existing references!",
        "Deleting record"
      )
    ) {
      this.removeItem();
    }
  }

  get json(): HierarchicEntityDao {
    return {
      ...super.json,
      Parent: this.ParentId,
      LinkPoints: this._parentLink
        ? this._parentLink
            .getPoints()
            .map((p) => ({ x: p.getX(), y: p.getY() }))
        : [],
    };
  }

  select() {
    if (this.url !== this.ei.context.Router.router.location.pathname) {
      this.ei.context.Router.router.push(this.url);
    }
  }

  deselect() {
    /**/
  }
}
