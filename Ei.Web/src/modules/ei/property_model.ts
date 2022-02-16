import { makeObservable, observable } from "mobx";

export interface PropertyDao {
  Name: string;
  DefaultValue: string;
  Type: string;
}

export class Property {
  @observable Name: string;
  @observable DefaultValue: string;
  @observable Type: string;

  constructor(model: PropertyDao) {
    this.Name = model.Name;
    this.DefaultValue =
      model.Type === "string"
        ? model.DefaultValue.substring(1, model.DefaultValue.length - 1)
        : model.DefaultValue;
    this.Type = model.Type;

    makeObservable(this);
  }

  get json() {
    return {
      Name: this.Name,
      DefaultValue:
        this.Type === "string" ? `"${this.DefaultValue}"` : this.DefaultValue,
      Type: this.Type,
    };
  }
}
