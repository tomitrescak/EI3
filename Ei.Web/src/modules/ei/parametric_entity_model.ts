import { IObservableArray, makeObservable, observable } from "mobx";
import { Entity, EntityDao } from "./entity_model";
import { Property, PropertyDao } from "./property_model";

export interface ParametricEntityDao extends EntityDao {
  Properties?: PropertyDao[];
}

const emptyProperties: Property[] = [];

export class ParametricEntity extends Entity {
  Properties: IObservableArray<Property>;

  constructor(model: Partial<ParametricEntityDao>, allowEditIcon = false) {
    super(model, allowEditIcon);
    this.Properties = observable(
      ((model && model.Properties) || emptyProperties)
        .map((p) => new Property(p))
        .sort((a, b) => (a.Name < b.Name ? -1 : 1))
    );
    makeObservable(this);
  }

  get json(): ParametricEntityDao {
    return {
      ...super.json,
      Properties: this.Properties.map((p) => p.json),
    };
  }
}
