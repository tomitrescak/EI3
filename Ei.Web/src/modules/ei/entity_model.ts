import { makeObservable } from "mobx";
import { field, requiredField } from "semantic-ui-mobx";
import { FormNodeStore } from "../diagrams/model/form_state_model";

export interface EntityDao {
  Id: string;
  Name: string;
  Description?: string;
  Icon?: string;
  x?: number;
  y?: number;
}

export function entitySort(a: Entity, b: Entity): number {
  return a.safeName.localeCompare(b.safeName, undefined, {
    numeric: true,
    sensitivity: "base",
  });
}

export class Entity extends FormNodeStore {
  Id: string;
  @field Icon: string;
  @requiredField Name: string;
  @field Description: string;

  allowEditIcon = false;

  constructor(model: Partial<EntityDao>, allowEditIcon = false) {
    super(model.Id);

    if (model) {
      this.Id = model.Id;
      this.Name = model.Name;
      this.Description = model.Description;

      if (allowEditIcon) {
        this.Icon = model.Icon;
      }
    }

    this.position.x = model.x || this.randomPosition();
    this.position.y = model.y || this.randomPosition();

    makeObservable(this);
  }

  deselect() {
    /**/
  }

  protected randomPosition() {
    return Math.floor(Math.random() * -100 + 1);
  }

  get safeName() {
    return (this.Name || this.Id).toLowerCase();
  }

  get json(): EntityDao {
    return {
      Id: this.Id,
      Name: this.Name,
      Description: this.Description,
      Icon: this.allowEditIcon ? this.Icon : undefined,
      x: this.position.x,
      y: this.position.y,
    };
  }
}
