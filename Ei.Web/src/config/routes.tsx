import * as React from 'react';
import { Route } from './mobx_router';

import { ActionView } from '../modules/actions/action_view';
import { ConnectionEditor } from '../modules/connections/connection_editor';
import { EntitiesView } from '../modules/diagrams/entities_view';
import { EntityView } from '../modules/diagrams/entity_view';
import { Ei } from '../modules/ei/ei_model';
import { EiProperties } from '../modules/ei/ei_properties';
import { StateEditor } from '../modules/states/state_editor';
import { TransitionEditor } from '../modules/transitions/transitions_editor';
import { WorkflowEditor } from '../modules/workflow/workflow_editor';
import { WorkflowView } from '../modules/workflow/workflow_view';

export const organisationSelector = (ei: Ei) => ei.Organisations;
export const roleSelector = (ei: Ei) => ei.Roles;
export const typeSelector = (ei: Ei) => ei.Types;

export function initRoutes(store: App.Store): Route[] {
  let view = store.viewStore;
  return [
    {
      name: 'home',
      route: '/',
      action: () => view.showView('home'),
      components: () => ({
        graph: <EiProperties />
      })
    },
    {
      name: 'organisations',
      route: '/organisations',
      action: () => view.showView('organisations'),
      components: () => ({
        graph: (
          <EntitiesView
            store={store}
            id={null}
            entities={organisationSelector}
            type="organisations"
          />
        )
      })
    },
    {
      name: 'organisation',
      route: '/organisation/:name/:id',
      action: (name, id) => view.showOrganisation(id, name),
      components: ({ id, name }) => ({
        graph: (
          <EntitiesView
            store={store}
            id={view.viewParameters.id}
            entities={organisationSelector}
            type="organisations"
          />
        ),
        editor: <EntityView id={id} name={name} collection={organisationSelector} />
      })
    },
    {
      name: 'roles',
      route: '/roles',
      action: () => view.showView('roles'),
      components: () => ({
        graph: <EntitiesView store={store} id={null} entities={roleSelector} type="roles" />
      })
    },
    {
      name: 'role',
      route: '/role/:name/:id',
      action: (name, id) => view.showRole(id, name),
      components: ({ id, name }) => ({
        graph: (
          <EntitiesView
            store={store}
            id={view.viewParameters.id}
            entities={roleSelector}
            type="roles"
          />
        ),
        editor: <EntityView id={id} name={name} collection={roleSelector} />
      })
    },
    {
      name: 'types',
      route: '/types',
      action: () => view.showView('types'),
      components: () => ({
        graph: <EntitiesView store={store} id={null} entities={typeSelector} type="types" />
      })
    },
    {
      name: 'type',
      route: '/type/:name/:id',
      action: (name, id) => view.showType(id, name),
      components: ({ id, name }) => ({
        graph: (
          <EntitiesView
            store={store}
            id={view.viewParameters.id}
            entities={typeSelector}
            type="types"
          />
        ),
        editor: <EntityView id={id} name={name} collection={typeSelector} />
      })
    },
    {
      name: 'action',
      route: '/workflows/:name/:id/action/:actionId',
      action: (name, id, actionId) => view.showAction(id, name, actionId),
      components: ({ id, actionId }) => ({
        graph: <WorkflowView id={id} />,
        editor: <ActionView id={actionId} workflowId={id} />
      })
    },
    {
      name: 'state',
      route: '/workflows/:name/:id/state/:stateId',
      action: (name, id, stateId) => view.showState(id, name, stateId),
      components: ({ id, stateId }) => ({
        graph: <WorkflowView id={id} />,
        editor: <StateEditor id={stateId} workflowId={id} />
      })
    },
    {
      name: 'transition',
      route: '/workflows/:name/:id/transition/:transitionId',
      action: (name, id, transitionId) => view.showTransition(id, name, transitionId),
      components: ({ id, transitionId }) => ({
        graph: <WorkflowView id={id} />,
        editor: <TransitionEditor id={transitionId} workflowId={id} />
      })
    },
    {
      name: 'connection',
      route: '/workflows/:name/:id/connection/:connectionId',
      action: (name, id, connectionId) => view.showConnection(id, name, connectionId),
      components: ({ id, connectionId }) => ({
        graph: <WorkflowView id={id} />,
        editor: <ConnectionEditor id={connectionId} workflowId={id} />
      })
    },
    {
      name: 'workflow',
      route: '/workflows/:name/:id',
      action: (name, id) => view.showWorkflow(id, name),
      components: ({ id }) => ({
        graph: <WorkflowView id={id} />,
        editor: <WorkflowEditor id={id} />
      })
    },
    {
      name: 'notFound',
      route: '/notFound',
      action: null,
      component: () => <div>Not found</div>
    }
  ];
}
