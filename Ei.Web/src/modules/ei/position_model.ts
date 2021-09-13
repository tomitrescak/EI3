import { makeObservable, observable } from "mobx";
import { Ei } from "./ei_model";
import { Entity, EntityDao } from "./entity_model";
import { Workflow } from "./workflow_model";

export abstract class PositionModel extends Entity {
  @observable selected: boolean;

  ei: Ei;
  workflow: Workflow;

  constructor(entity: Partial<EntityDao>, workflow: Workflow, ei: Ei) {
    super(entity);

    this.ei = ei;
    this.workflow = workflow;

    // add listeners
    this.registerListener({
      selectionChanged: ({ isSelected }) => {
        isSelected ? this.select() : this.deselect();
      },
    });

    makeObservable(this);
  }

  abstract select(): void;
}
