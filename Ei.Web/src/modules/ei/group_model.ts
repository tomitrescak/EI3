import { makeObservable, observable } from "mobx";

export interface GroupDao {
  OrganisationId: string;
  RoleId: string;
}

export class Group {
  @observable OrganisationId: string;
  @observable RoleId: string;

  constructor(group: GroupDao) {
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
