import { makeObservable } from "mobx";
import { field, FormState } from "semantic-ui-mobx";

export interface GroupDao {
  OrganisationId: string;
  RoleId: string;
}

export class Group extends FormState {
  @field OrganisationId: string;
  @field RoleId: string;

  constructor(group: GroupDao) {
    super();

    this.OrganisationId = group.OrganisationId;
    this.RoleId = group.RoleId;

    makeObservable(this);
  }

  get json() {
    return {
      OrganisationId: this.OrganisationId,
      RoleId: this.RoleId,
    };
  }
}
