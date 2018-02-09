import * as React from 'react';

import { action, observable } from 'mobx';
import { MobxRouter, Route } from './mobx_router';

declare global {
  namespace App { type ViewStore = ViewStoreModel; }
}

export class ViewStoreModel {
  @observable view = 'home';
  @observable viewParameters: any = {};

  router: MobxRouter;
  store: App.Store;

  constructor(store: App.Store) {
    this.store = store;
  }

  startRouter(routes: Route[]) {
    this.router = new MobxRouter(this);
    this.router.startRouter(routes);
  }


  @action
  showView(name: string, params?: any) {
    this.view = name;
    this.viewParameters = params || {};
  }

  @action
  showOrganisation(id: string, name: string) {
    this.showView('organisation', { id: id.toUrlName(), name: name.toUrlName() });
  }

  @action
  showRole(id: string, name: string) {
    this.showView('role', { id: id.toUrlName(), name: name.toUrlName() });
  }

  @action
  showType(id: string, name: string) {
    this.showView('type', { id: id.toUrlName(), name: name.toUrlName() });
  }

  parseEvent(e: React.MouseEvent<HTMLAnchorElement>) {
    const target = e.currentTarget;
    return {
      workflowId: target.getAttribute('data-workflow-id'),
      workflowName: target.getAttribute('data-workflow-name'),
      id: target.getAttribute('data-id')
    };
  }

  showActionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    let p = this.parseEvent(e);
    this.showAction(p.workflowId, p.workflowName, p.id);
  };

  showAction = (workflowId: string, workflowName: string, actionId: string) => {
    this.showView('action', { id: workflowId, name: workflowName.toUrlName(), actionId });
  };

  showWorkflow(workflowId: string, workflowName: string) {
    this.showView('workflow', { id: workflowId, name: workflowName.toUrlName() });
  }

  showStateClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    let p = this.parseEvent(e);
    this.showState(p.workflowId, p.workflowName, p.id);
  };

  showState = (workflowId: string, workflowName: string, stateId: string) => {
    this.showView('state', { id: workflowId, name: workflowName.toUrlName(), stateId });
  };

  showTransitionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    let p = this.parseEvent(e);
    this.showTransition(p.workflowId, p.workflowName, p.id);
  };

  showTransition(workflowId: string, workflowName: string, transitionId: string) {
    this.showView('transition', { id: workflowId, name: workflowName.toUrlName(), transitionId });
  }

  showConnectionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    let p = this.parseEvent(e);
    this.showConnection(p.workflowId, p.workflowName, p.id);
  };

  showConnection(workflowId: string, workflowName: string, connectionId: string) {
    this.showView('connection', { id: workflowId, name: workflowName.toUrlName(), connectionId });
  }
}
