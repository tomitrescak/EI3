import * as React from "react";

// import { action, observable } from 'mobx';
// import { EiDao } from '../modules/ei/ei_model';

// declare global {
//   namespace App { type ViewStore = ViewStoreModel; }
// }

// export class ViewStoreModel {
//   @observable view = 'home';
//   @observable viewParameters: any = {};

//   store: App.Store;
//   context: App.Context;

//   ei: EiDao;

//   constructor(context: App.Context) {
//     this.context = context;
//     this.store = context;
//   }

//   startRouter(routes: Route[]) {
//     this.router = new MobxRouter(this.context);
//     this.router.startRouter(routes);
//   }

//   @action
//   showView(name: string, params?: any) {
//     this.view = name;

//     params = this.ei ? { ...params, eiId: this.ei.Id, eiName: this.ei.Name.toUrlName() } : params;

//     this.viewParameters = params || {};
//   }

//   @action
//   showExecution(eiId: string, eiName: string) {
//     this.showView('execution', { eiName, eiId });
//   }

//   @action
//   showExperiment(eiId: string, eiName: string, experimentId: string, experimentName: string, viewName: string) {
//     this.showView(viewName, { eiName, eiId, experimentId, experimentName });
//   }

//   @action
//   showOrganisation(id: string, name: string, eiId?: string, eiName?: string) {
//     this.showView('organisation', { eiName, eiId, id: id.toUrlName(), name: name.toUrlName() });
//   }

//   @action
//   showRole(id: string, name: string, eiId?: string, eiName?: string) {
//     this.showView('role', { eiName, eiId, id: id.toUrlName(), name: name.toUrlName() });
//   }

//   @action
//   showType(id: string, name: string, eiId?: string, eiName?: string) {
//     this.showView('type', { eiName, eiId, id: id.toUrlName(), name: name.toUrlName() });
//   }

//   parseEvent(e: React.MouseEvent<HTMLAnchorElement>) {
//     const target = e.currentTarget;
//     return {
//       workflowId: target.getAttribute('data-workflow-id'),
//       workflowName: target.getAttribute('data-workflow-name'),
//       id: target.getAttribute('data-id')
//     };
//   }

//   showActionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
//     let p = this.parseEvent(e);
//     this.showAction(p.workflowId, p.workflowName, p.id);
//   };

//   showAuthorisation = (index: string, eiId?: string, eiName?: string) => {
//     this.showView('authorisation', { eiName, eiId, index });
//   }

//   showEi(eiId: string, eiName: string) {
//     this.showView('ei', { eiId, eiName });
//   }

//   showAction = (workflowId: string, workflowName: string, actionId: string, eiId?: string, eiName?: string) => {
//     this.showView('action', { eiName, eiId, id: workflowId, name: workflowName.toUrlName(), actionId });
//   };

//   showWorkflow(workflowId: string, workflowName: string, eiId?: string, eiName?: string) {
//     this.showView('workflow', { eiName, eiId, id: workflowId, name: workflowName.toUrlName() });
//   }

//   showStateClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
//     let p = this.parseEvent(e);
//     this.showState(p.workflowId, p.workflowName, p.id);
//   };

//   showState = (workflowId: string, workflowName: string, stateId: string, eiId?: string, eiName?: string) => {
//     this.showView('state', { eiName, eiId, id: workflowId, name: workflowName.toUrlName(), stateId });
//   };

//   showTransitionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
//     let p = this.parseEvent(e);
//     this.showTransition(p.workflowId, p.workflowName, p.id);
//   };

//   showTransition(workflowId: string, workflowName: string, transitionId: string, eiId?: string, eiName?: string) {
//     this.showView('transition', { eiName, eiId, id: workflowId, name: workflowName.toUrlName(), transitionId });
//   }

//   showConnectionClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
//     let p = this.parseEvent(e);
//     this.showConnection(p.workflowId, p.workflowName, p.id);
//   };

//   showConnection(workflowId: string, workflowName: string, connectionId: string, eiId?: string, eiName?: string) {
//     this.showView('connection', { eiName, eiId, id: workflowId, name: workflowName.toUrlName(), connectionId });
//   }
// }
