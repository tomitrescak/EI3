import { IObservableArray, makeObservable, observable } from "mobx";
import { field, FormState } from "semantic-ui-mobx";
import { Group, GroupDao } from "./group_model";

// tslint:disable-next-line:no-empty-interface
export interface AuthorisationDao {
  Organisation: string;
  User: string;
  Password: string;
  Groups: GroupDao[];
}
export class Authorisation extends FormState {
  @field Organisation: string;
  @field User: string;
  @field Password: string;
  Groups: IObservableArray<Group>;

  constructor(authorisation: Partial<AuthorisationDao> = {}) {
    super();

    this.Organisation = authorisation.Organisation;
    this.User = authorisation.User;
    this.Password = authorisation.Password;
    this.Groups = observable(
      (authorisation.Groups || []).map((g) => new Group(g))
    );

    makeObservable(this);
  }

  get json() {
    return {
      Organisation: this.Organisation,
      User: this.User,
      Password: this.Password,
      Groups: this.Groups.map((g) => g.json),
    };
  }
}
