import { DiffPatcher } from "jsondiffpatch";
import { action, observable } from "mobx";
import { AppContext } from "../../config/context";

import { store } from "../../config/store";
import { Ei, EiDao } from "../ei/ei_model";

const patcher = new DiffPatcher(); // { textDiff: 10000 }

class Step {
  deltas: any[];
  previous: Step;
  next: Step;

  constructor(deltas: any[]) {
    this.deltas = deltas;
  }

  @action
  undo(ei: EiDao) {
    let result = ei;
    for (let delta of this.deltas.reverse()) {
      result = patcher.unpatch(result, delta);
    }
    this.deltas.reverse();
    return result;
  }

  @action
  redo(ei: EiDao) {
    let result = ei;
    this.deltas.forEach((delta) => (result = patcher.patch(result, delta)));
    return result;
  }
}

export class WorkHistory {
  @observable version = 0;
  ei: Ei;
  context: AppContext;

  currentEi: EiDao;
  currentStep: Step;
  deltas: any[] = [];
  timeout: any;

  startHistory(ei: Ei, context: AppContext) {
    this.context = context;
    this.ei = ei;
    // this.first = ei.json;
    this.currentEi = ei.json;
    this.currentStep = new Step(null);
  }

  step = () => {
    const current = this.ei.json;
    const delta = patcher.diff(this.currentEi, current);

    this.currentEi = current;

    this.deltas.push(delta);

    if (this.timeout) {
      clearTimeout(this.timeout);
    }

    this.timeout = setTimeout(() => {
      const step = new Step(this.deltas);
      step.previous = this.currentStep;
      this.currentStep.next = step;
      this.currentStep = step;
      this.deltas = [];
    }, 200);
  };

  undo = () => {
    if (!this.currentStep.previous) {
      return;
    }
    this.currentEi = this.currentStep.undo(this.currentEi);
    this.currentStep = this.currentStep.previous;

    let s = this.context;
    this.ei = new Ei(this.currentEi, this.context);
    s.ei = this.ei;
    this.version++;
  };

  redo = () => {
    if (!this.currentStep.next) {
      return;
    }
    this.currentStep = this.currentStep.next;
    this.currentEi = this.currentStep.redo(this.currentEi);

    let s = this.context;
    this.ei = new Ei(this.currentEi, this.context);
    s.ei = this.ei;
    this.version++;
  };
}
