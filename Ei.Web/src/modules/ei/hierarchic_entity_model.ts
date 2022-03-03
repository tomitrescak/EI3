import { action, IObservableArray, makeObservable, observable } from "mobx";
import { Ui } from "../../helpers/client_helpers";
import { Point } from "../diagrams/model/diagram_common";
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
  ei: Ei;
  ParentId: string;
  selected: boolean;

  connection: React.MutableRefObject<SVGLineElement>;

  // private _parentLink: EntityLinkModel;
  private parents: IObservableArray<HierarchicEntity>;
  points: PointDao[];

  route: string;
  height = 60;

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
    // this.addPort(new EntityPortModel("top", PortModelAlignment.TOP));
    // this.addPort(new EntityPortModel("bottom", PortModelAlignment.BOTTOM));

    // // add listeners
    // this.registerListener({
    //   selectionChanged: ({ isSelected }) => {
    //     isSelected ? this.select() : this.deselect();
    //   },
    // } as NodeModelListener);

    if (
      this.ei.context.Router.router.location.pathname +
        this.ei.context.Router.router.location.search ==
      this.url
    ) {
      this.selected = true;
      ei.context.selectedEntity = this;
    }

    if (model) {
      this.setParentId(model.Parent);

      // if there is a panel, create a link
      // this.update();
    }

    makeObservable(this, {
      // _parent: observable,
      ParentId: observable,
      selected: observable,
      setParentId: action,
      removeItem: action,
    });
  }

  // set parentLink(value: EntityLinkModel) {
  //   this._parentLink = value;
  // }

  get url() {
    return `/ei/${this.ei.Name.toUrlName()}/${
      this.route
    }/${this.Name.toUrlName()}?ei=${this.ei.Id}&id=${this.Id}`.toLowerCase();
  }

  topPort(pos?: Point) {
    return {
      x: (pos?.x || this.position.x) + this.size / 2 - 2,
      y: (pos?.y || this.position.y) + 0,
    };
  }

  bottomPort(pos?: Point) {
    return {
      x: (pos?.x || this.position.x) + this.size / 2 - 2,
      y: (pos?.y || this.position.y) + this.height,
    };
  }

  setParentId(parent: string) {
    this.ParentId = parent;
  }

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

  updateConnection(entities: HierarchicEntity[], p: { x: number; y: number }) {
    let parent = entities.find((e) => e.Id === this.ParentId);

    // update all child connection

    let children = entities.filter((e) => e.ParentId === this.Id);

    for (let ent of children) {
      if (ent.connection) {
        ent.connection.current.setAttribute(
          "d",
          `M ${this.bottomPort(p).x} ${this.bottomPort(p).y} C ${
            this.bottomPort(p).x
          } ${this.bottomPort(p).y + 50}, ${ent.topPort().x} ${
            ent.topPort().y - 50
          }, ${ent.topPort().x} ${ent.topPort().y}`
        );
      }
    }

    // update parent connection

    if (parent && this.connection.current) {
      this.connection.current.setAttribute(
        "d",
        `M ${parent.bottomPort().x} ${parent.bottomPort().y} C ${
          parent.bottomPort().x
        } ${parent.bottomPort().y + 50}, ${this.topPort(p).x} ${
          this.topPort(p).y - 50
        }, ${this.topPort(p).x} ${this.topPort(p).y}`
      );
    }
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
      // LinkPoints: this._parentLink
      //   ? this._parentLink
      //       .getPoints()
      //       .map((p) => ({ x: p.getX(), y: p.getY() }))
      //   : [],
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
