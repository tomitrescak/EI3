import * as React from "react";

import { ActionView } from "../modules/actions/action_view";
import { AuthorisationEditor } from "../modules/authorisations/authorisation_editor";
import { ConnectionEditor } from "../modules/connections/connection_editor";
import { EntitiesView } from "../modules/diagrams/entities_view";
import { HierarchicEntityEditor } from "../modules/diagrams/hierarchic_entity_editor";
import { EiContainer } from "../modules/ei/ei_container";
import { EiEditor } from "../modules/ei/ei_editor";
import { EiListContainer } from "../modules/ei/ei_list";
import { Ei } from "../modules/ei/ei_model";
import { ExecutionView } from "../modules/execution/execution_view";
import { ExperimentAgents } from "../modules/experiments/experiment_agents_view";
import { ExperimentEnvironment } from "../modules/experiments/experiment_enviroment_view";
import { ExperimentGeneral } from "../modules/experiments/experiment_general_view";
import { StateEditor } from "../modules/states/state_editor";
import { TransitionEditor } from "../modules/transitions/transitions_editor";
import { WorkflowEditor } from "../modules/workflow/workflow_editor";
import { WorkflowView } from "../modules/workflow/workflow_view";

export const organisationSelector = (ei: Ei) => ei.Organisations;
export const roleSelector = (ei: Ei) => ei.Roles;
export const typeSelector = (ei: Ei) => ei.Types;

export function initRoutes(store: App.Store): Route[] {
  let view = store.viewStore;
  return [
    {
      component: EiContainer,
      route: "/:eiName/:eiId",
      children: [
        {
          name: "ei",
          route: "",
          action: (name, id) => view.showEi(id, name),
          components: () => ({
            graph: <EiEditor />,
          }),
        },
        {
          name: "execution",
          route: "/execution",
          action: (eiName, eiId) => view.showExecution(eiName, eiId),
          components: () => ({
            graph: <ExecutionView />,
          }),
        },
        {
          name: "experimentGeneral",
          route: "/experiment/:experimentName/general/:experimentId",
          action: (eiName, eiId, experimentName, experimentId) =>
            view.showExperiment(
              eiId,
              eiName,
              experimentId,
              experimentName,
              "experimentGeneral"
            ),
          components: () => ({
            graph: <ExperimentGeneral />,
          }),
        },
        {
          name: "experimentAgents",
          route: "/experiment/:experimentName/agents/:experimentId",
          action: (eiName, eiId, experimentName, experimentId) =>
            view.showExperiment(
              eiId,
              eiName,
              experimentId,
              experimentName,
              "experimentAgents"
            ),
          components: () => ({
            graph: <ExperimentAgents />,
          }),
        },
        {
          name: "experimentEnvironment",
          route: "/experiment/:experimentName/environment/:experimentId",
          action: (eiName, eiId, experimentName, experimentId) =>
            view.showExperiment(
              eiId,
              eiName,
              experimentId,
              experimentName,
              "experimentEnvironment"
            ),
          components: () => ({
            graph: <ExperimentEnvironment />,
          }),
        },
        {
          name: "authorisation",
          route: "/authorisation/:index",
          action: (eiName, eiId, index) =>
            view.showAuthorisation(index, eiId, eiName),
          components: ({ index }) => ({
            graph: <AuthorisationEditor index={index} />,
          }),
        },
        {
          name: "organisations",
          route: "/organisations",
          action: (eiName, eiId) =>
            view.showView("organisations", { eiName, eiId }),
          components: () => ({
            graph: (
              <EntitiesView
                store={store}
                id={null}
                entities={organisationSelector}
                type="organisations"
              />
            ),
          }),
        },
        {
          name: "organisation",
          route: "/organisation/:name/:id",
          action: (eiName, eiId, name, id) =>
            view.showOrganisation(id, name, eiId, eiName),
          components: ({ id, name }) => ({
            graph: (
              <EntitiesView
                store={store}
                id={view.viewParameters.id}
                entities={organisationSelector}
                type="organisations"
              />
            ),
            editor: (
              <HierarchicEntityEditor
                id={id}
                name={name}
                collection={organisationSelector}
                minCount={1}
                parentView="organisations"
              />
            ),
          }),
        },
        {
          name: "roles",
          route: "/roles",
          action: (eiName, eiId) => view.showView("roles", { eiName, eiId }),
          components: () => ({
            graph: (
              <EntitiesView
                store={store}
                id={null}
                entities={roleSelector}
                type="roles"
              />
            ),
          }),
        },
        {
          name: "role",
          route: "/role/:name/:id",
          action: (eiName, eiId, name, id) =>
            view.showRole(id, name, eiId, eiName),
          components: ({ id, name }) => ({
            graph: (
              <EntitiesView
                store={store}
                id={view.viewParameters.id}
                entities={roleSelector}
                type="roles"
              />
            ),
            editor: (
              <HierarchicEntityEditor
                id={id}
                name={name}
                collection={roleSelector}
                minCount={1}
                parentView="roles"
              />
            ),
          }),
        },
        {
          name: "types",
          route: "/types",
          action: (eiName, eiId) => view.showView("types", { eiName, eiId }),
          components: () => ({
            graph: (
              <EntitiesView
                store={store}
                id={null}
                entities={typeSelector}
                type="types"
              />
            ),
          }),
        },
        {
          name: "type",
          route: "/type/:name/:id",
          action: (eiName, eiId, name, id) =>
            view.showType(id, name, eiId, eiName),
          components: ({ id, name }) => ({
            graph: (
              <EntitiesView
                store={store}
                id={view.viewParameters.id}
                entities={typeSelector}
                type="types"
              />
            ),
            editor: (
              <HierarchicEntityEditor
                id={id}
                name={name}
                collection={typeSelector}
                minCount={1}
                parentView="types"
              />
            ),
          }),
        },
        {
          name: "action",
          route: "/workflows/:name/:id/action/:actionId",
          action: (eiName, eiId, name, id, actionId) =>
            view.showAction(id, name, actionId, eiId, eiName),
          components: ({ id, actionId }) => ({
            graph: <WorkflowView id={id} />,
            editor: <ActionView id={actionId} workflowId={id} />,
          }),
        },
        {
          name: "state",
          route: "/workflows/:name/:id/state/:stateId",
          action: (eiName, eiId, name, id, stateId) =>
            view.showState(id, name, stateId, eiId, eiName),
          components: ({ id, stateId }) => ({
            graph: <WorkflowView id={id} />,
            editor: <StateEditor id={stateId} workflowId={id} />,
          }),
        },
        {
          name: "transition",
          route: "/workflows/:name/:id/transition/:transitionId",
          action: (eiName, eiId, name, id, transitionId) =>
            view.showTransition(id, name, transitionId, eiId, eiName),
          components: ({ id, transitionId }) => ({
            graph: <WorkflowView id={id} />,
            editor: <TransitionEditor id={transitionId} workflowId={id} />,
          }),
        },
        {
          name: "connection",
          route: "/workflows/:name/:id/connection/:connectionId",
          action: (eiName, eiId, name, id, connectionId) =>
            view.showConnection(id, name, connectionId, eiId, eiName),
          components: ({ id, connectionId }) => ({
            graph: <WorkflowView id={id} />,
            editor: <ConnectionEditor id={connectionId} workflowId={id} />,
          }),
        },
        {
          name: "workflow",
          route: "/workflows/:name/:id",
          action: (eiName, eiId, name, id) =>
            view.showWorkflow(id, name, eiId, eiName),
          components: ({ id }) => ({
            graph: <WorkflowView id={id} />,
            editor: <WorkflowEditor id={id} />,
          }),
        },
      ],
    },
    {
      name: "home",
      route: "/",
      action: () => view.showView("home"),
      component: EiListContainer,
    },
    {
      name: "notFound",
      route: "/notFound",
      action: null,
      component: () => <div>Not found</div>,
    },
  ];
}
