import { makeObservable } from "mobx";
import { FormState, requiredField } from "semantic-ui-mobx";

export interface PropertyDao {
  Name: string;
  DefaultValue: string;
  Type: string;
}

export class Property extends FormState {
  @requiredField Name: string;
  @requiredField DefaultValue: string;
  @requiredField Type: string;

  constructor(model: PropertyDao) {
    super();

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
