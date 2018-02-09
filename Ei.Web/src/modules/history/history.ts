import { action, IObservableArray } from 'mobx';
import { Ei, EiDao } from '../ei/ei_model';

interface HistoryStep {
  undo: Function;
  redo: Function;
}

class Step {
  previous: Step;
  next: Step;

  constructor(private steps: HistoryStep[]) { }

  @action
  undo() {
    this.steps.forEach(s => s.undo());
  }

  @action
  redo() {
    this.steps.forEach(s => s.redo());
    // this.steps.reverse();
  }
}

export class WorkHistory {
  ei: Ei;

  currentEi: EiDao;
  currentStep: Step;
  inProcess: boolean;

  currentSteps: HistoryStep[] = [];
  timeout: any;

  startHistory(ei: Ei) {
    this.ei = ei;
    // this.first = ei.json;
    this.currentEi = ei.json;
    this.currentStep = new Step(null);
  }

  addToCollection(collection: IObservableArray<any>, element: any) {
    // collection.push(element);

    this.step(() => collection.remove(element), () => collection.push(element));
  }

  removeFromCollection(collection: IObservableArray<any>, element: any) {
    // collection.remove(element);

    this.step(() => collection.push(element), () => collection.remove(element));
  }

  assignValue<T, K extends keyof T>(owner: T, property: K, newValue: any, oldValue?: any) {
    oldValue = oldValue || owner[property];
    this.step(() => (owner[property] = oldValue), () => (owner[property] = newValue));
  }

  step = (undo: Function, redo: Function) => {
    if (this.inProcess) {
      return;
    }
    if (this.timeout) {
      clearTimeout(this.timeout);
    }

    this.currentSteps.push({ undo, redo });

    this.timeout = setTimeout(() => {
      const step = new Step(this.currentSteps);
      step.previous = this.currentStep;
      this.currentStep.next = step;
      this.currentStep = step;
      this.currentSteps = [];
    }, 30);
  };

  undo = () => {
    if (!this.currentStep.previous) {
      return;
    }
    this.inProcess = true;
    this.currentStep.undo();
    this.currentStep = this.currentStep.previous;
    this.inProcess = false;
  };

  redo = () => {
    if (!this.currentStep.next) {
      return;
    }
    this.inProcess = true;
    this.currentStep = this.currentStep.next;
    this.currentStep.redo();
    this.inProcess = false;
  };
}
