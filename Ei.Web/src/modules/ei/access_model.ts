import { IObservableArray, makeObservable, observable } from "mobx";
import { field, FormState } from "semantic-ui-mobx";

export interface PostconditionDao {
  Condition: string;
  Action: string;
}

export interface AccessConditionDao {
  Role: string;
  Organisation: string;
  Precondition: string;
  Postconditions: PostconditionDao[];
}

export class Postcondition extends FormState {
  @field Condition: string;
  @field Action: string;

  constructor(postcondition: PostconditionDao) {
    super();

    if (postcondition) {
      this.Condition = postcondition.Condition;
      this.Action = postcondition.Action;
    }

    makeObservable(this);
  }

  get json() {
    return {
      Condition: this.Condition,
      Action: this.Action,
    };
  }
}

export class AccessCondition extends FormState {
  @field Role: string;
  @field Organisation: string;
  @field Precondition: string;
  Postconditions: IObservableArray<Postcondition>;

  constructor(condition: Partial<AccessConditionDao>) {
    super();

    this.Role = condition.Role;
    this.Organisation = condition.Organisation;
    this.Precondition = condition.Precondition;
    this.Postconditions = observable(
      (condition.Postconditions || []).map((c) => new Postcondition(c))
    );
  }

  get json() {
    return {
      Role: this.Role,
      Organisation: this.Organisation,
      Precondition: this.Precondition,
      Postconditions: this.Postconditions.map((c) => c.json),
    };
  }
}
