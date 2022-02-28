import { Router, Ui } from "../helpers/client_helpers";
import { SocketClient } from "../modules/ws/socket_client";

import "../helpers/class_helpers";
import React from "react";
import { makeObservable, observable } from "mobx";
import { Ei } from "../modules/ei/ei_model";
import { Entity } from "../modules/ei/entity_model";
import type { History } from "history";
import { DiagramEngine } from "@projectstorm/react-diagrams-core";

// const socketUrl = 'ws://10.211.55.4:5000/wd';
const socketUrl = "ws://localhost:5000/wd";

export interface CompilationError {
  Message: string;
  Line: number;
  Code: string[];
}

export interface AccordionHandler {
  handleClick(e: any, titleProps: any): void;
  isActive(index: any): boolean;
}

export class AppContext {
  serverUrl: string;
  client: SocketClient;
  Ui: typeof Ui;
  Router: typeof Router;

  previewImage = "";
  saving = false;
  compiledCode = "";
  compiling = false;
  ei: Ei;
  messages = observable([] as string[]);
  errors = observable([] as CompilationError[]);
  storedHandlers: any;

  handlers: { [index: string]: AccordionHandler } = {};
  selectedEntity: Entity;
  engine: DiagramEngine;

  constructor(_cache = true) {
    this.serverUrl = socketUrl;
    this.client = new SocketClient(this.serverUrl, this);

    this.Ui = { ...Ui };
    this.Router = Router;

    makeObservable(this, {
      previewImage: observable,
      saving: observable,
      compiledCode: observable,
      compiling: observable,
      ei: observable,
    });
  }

  assignRouter(history: History) {
    this.Router.router = history;
    this.Router.push = history.push;
    this.Router.replace = history.replace;
  }

  warn(message: string) {
    /** */
    // tslint:disable-next-line:no-console
    console.warn(message);
  }

  selectWorkflowElement(
    wid: string,
    collection: string,
    id: string,
    subSelection: string = null
  ) {
    let workflow = this.ei.Workflows.find((w) => w.Id === wid);
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
    if (!entity.isSelected()) {
      entity.setSelected(true);
      this.selectedEntity = entity;
    }
  }

  createAccordionHandler(id: string, openedNodes: number[] = []) {
    // we store all in local storage

    if (this.storedHandlers == null) {
      let storedString = localStorage.getItem("ei.accordions");
      this.storedHandlers =
        storedString != null ? JSON.parse(storedString) : {};
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

          localStorage.setItem(
            "ei.accordions",
            JSON.stringify(this.storedHandlers)
          );
        },
        isActive(index: number) {
          return activeIndices.indexOf(index) >= 0;
        },
      };
    }

    return this.handlers[id];
  }
}

export const Context = React.createContext<AppContext>(null);

export const useAppContext = () => React.useContext(Context);
