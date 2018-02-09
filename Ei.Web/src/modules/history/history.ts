import { Ei, EiDao } from '../ei/ei_model';

class Step {
  previous: Step;
  next: Step;
  constructor(public undo: Function, public redo: Function) {}
}


export class WorkHistory {
  ei: Ei;
  
  currentEi: EiDao;
  currentStep: Step;
  inProcess: boolean;

  startHistory(ei: Ei) {
    this.ei = ei;
    // this.first = ei.json;
    this.currentEi = ei.json;
    this.currentStep = new Step(null, null);
  }

  step = (undo: Function, redo: Function) => {
    if (this.inProcess) {
      return;
    }
    const step = new Step(undo, redo);
    step.previous = this.currentStep;
    this.currentStep.next = step;
    this.currentStep = step;
  }

  undo = () => {
    if (!this.currentStep.previous) {
      return;
    }
    this.inProcess = true;
    this.currentStep.undo();
    this.currentStep = this.currentStep.previous;
    this.inProcess = false;
  }

  redo = () => {
    if (!this.currentStep.next) {
      return;
    }
    this.inProcess = true;
    this.currentStep = this.currentStep.next;
    this.currentEi = this.currentStep.redo();
    this.inProcess = false;
  
  }
}
