import { observable } from 'mobx';
import { Ei } from '../modules/ei/ei_model';
import { Entity } from '../modules/ei/entity_model';
import { ViewStoreModel } from './view_store';

declare global {
  namespace App { type Store = StoreModel; }
}

export interface AccordionHandler {
  handleClick(e: any, titleProps: any): void;
  isActive(index: any): boolean;
}

export interface CompilationError {
  Message: string;
  Line: number;
  Code: string[];
}

export class StoreModel {
  @observable previewImage = '';
  @observable saving = false;
  @observable compiledCode = '';
  @observable compiling = false;
  messages = observable([] as string[]);
  errors = observable([] as CompilationError[]);

  context: App.Context;
  @observable.shallow ei: Ei;
  viewStore: App.ViewStore;
  storedHandlers: Object;

  handlers: { [index: string]: AccordionHandler } = {};
  selectedEntity: Entity;

  constructor(context: App.Context) {
    this.context = context;
    this.viewStore = new ViewStoreModel(context);

    // autorun(() => {
    //   debugger;
    //   let parameters = this.viewParameters;

    // })
  }

  warn(message: string) {
    /** */
    // tslint:disable-next-line:no-console
    console.warn(message);
  }

  selectWorkflowElement(wid: string, collection: string, id: string, subSelection: string = null) {
    let workflow = this.ei.Workflows.find(w => w.Id === wid);
    if (!workflow) {
      return;
    }
    let entity: Entity = workflow[collection].find((a: Entity) => a.Id === id);
    if (!entity) {
      return;
    }
    if (subSelection) {
      entity = entity[subSelection];
    }
    if (this.selectedEntity) {
      this.selectedEntity.setSelected(false);
    }
    if (!entity.selected) {
      entity.setSelected(true);
      this.selectedEntity = entity;
    }
  }

  createAccordionHandler(id: string, openedNodes: number[] = []) {
    // we store all in local storage

    if (this.storedHandlers == null) {
      let storedString = localStorage.getItem('ei.accordions');
      this.storedHandlers = storedString != null ? JSON.parse(storedString) : {};
    }
    if (this.storedHandlers[id] == null) {
      this.storedHandlers[id] = openedNodes;
    }
    let storedIndices = this.storedHandlers[id];
    let activeIndices = observable(storedIndices);

    if (!this.handlers[id]) {
      this.handlers[id] = {
        handleClick: (_e: any, titleProps: any) => {
          const { index } = titleProps;
          const active = activeIndices.indexOf(index) >= 0;

          if (active) {
            activeIndices.splice(activeIndices.indexOf(index), 1);
            storedIndices.splice(activeIndices.indexOf(index), 1);
          } else {
            activeIndices.push(index);
            storedIndices.push(index);
          }

          localStorage.setItem('ei.accordions', JSON.stringify(this.storedHandlers));
        },
        isActive(index: number) {
          return activeIndices.indexOf(index) >= 0;
        }
      };
    }

    return this.handlers[id];
  }
}

let current: StoreModel;

export function store(cache = true, context?: App.Context) {
  if (!current || !cache) {
    current = new StoreModel(context);
    (global as any).__store = current;
  }
  return current;
}
