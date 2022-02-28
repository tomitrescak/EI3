import { IObservableArray, makeObservable, observable } from "mobx";

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

export class Postcondition {
  Condition: string;
  Action: string;

  constructor(postcondition: PostconditionDao) {
    if (postcondition) {
      this.Condition = postcondition.Condition;
      this.Action = postcondition.Action;
    }

    makeObservable(this, {
      Condition: observable,
      Action: observable,
    });
  }

  get json() {
    return {
      Condition: this.Condition,
      Action: this.Action,
    };
  }
}

export class AccessCondition {
  Role: string;
  Organisation: string;
  Precondition: string;
  Postconditions: IObservableArray<Postcondition>;

  constructor(condition: Partial<AccessConditionDao>) {
    this.Role = condition.Role;
    this.Organisation = condition.Organisation;
    this.Precondition = condition.Precondition;
    this.Postconditions = observable(
      (condition.Postconditions || []).map((c) => new Postcondition(c))
    );

    makeObservable(this, {
      Role: observable,
      Organisation: observable,
      Precondition: observable,
      Postconditions: observable,
    });
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
