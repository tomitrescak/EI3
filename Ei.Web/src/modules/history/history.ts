import { DiffPatcher } from 'jsondiffpatch';
import { observable } from 'mobx';

import { store } from '../../config/store';
import { Ei, EiDao } from '../ei/ei_model';

const patcher = new DiffPatcher();

class Step {
  delta: any;
  previous: Step;
  next: Step;

  constructor(delta: any) {
    this.delta = delta;
  }

  undo(ei: EiDao) {
    return patcher.unpatch(ei, this.delta);
  }

  redo(ei: EiDao) {
    return patcher.patch(ei, this.delta);
  }
}

export class WorkHistory {
  ei: Ei;
  
  currentEi: EiDao;
  currentStep: Step;

  startHistory(ei: Ei) {
    this.ei = ei;
    // this.first = ei.json;
    this.currentEi = ei.json;
    this.currentStep = new Step(null);
  }

  step = () => {
    const current = this.ei.json;
    const delta = patcher.diff(this.currentEi, current);
    
    this.currentEi = current;

    const step = new Step(delta);
    step.previous = this.currentStep;
    this.currentStep.next = step;
    this.currentStep = step;
  }

  undo = () => {
    if (!this.currentStep.previous) {
      return;
    }
    this.currentEi = this.currentStep.undo(this.currentEi);
    this.currentStep = this.currentStep.previous;
    
    let s = store();
    s.ei = new Ei(this.currentEi, s);
  }

  redo = () => {
    if (!this.currentStep.next) {
      return;
    }
    this.currentStep = this.currentStep.next;
    this.currentEi = this.currentStep.redo(this.currentEi);
    
    let s = store();
    s.ei = new Ei(this.currentEi, s);
  }
}
