import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom";

import { SemanticToastContainer } from "react-semantic-toasts";
import {
  BrowserRouter as Router,
  Switch,
  Route,
  useHistory,
} from "react-router-dom";
import { EiListContainer } from "./modules/ei/ei_list";
import { AppContext, Context, useAppContext } from "./config/context";
import { EiContainer } from "./modules/ei/ei_container";
import { EiLayout } from "./modules/ei/ei_layout";
import { EiEditor } from "./modules/ei/ei_editor";
import { ExecutionView } from "./modules/execution/execution_view";
import { AuthorisationEditor } from "./modules/authorisations/authorisation_editor";
import { EntitiesView } from "./modules/diagrams/entities_view";
import { supabase } from "./config/supabase";
import { configure } from "mobx";
import { HierarchicEntityEditor } from "./modules/diagrams/hierarchic_entity_editor";
import Account from "./auth/account";
import { WorkflowView } from "./modules/workflow/workflow_view";
import { ActionView } from "./modules/actions/action_view";
import { StateEditor } from "./modules/states/state_editor";
import { TransitionEditor } from "./modules/transitions/transitions_editor";
import { ConnectionEditor } from "./modules/connections/connection_editor";

configure({ enforceActions: "never" });

const App = () => {
  // init router
  const history = useHistory();
  const context = useAppContext();
  context.assignRouter(history);

  const [session, setSession] = useState(null);
  useEffect(() => {
    setSession(supabase.auth.session());

    supabase.auth.onAuthStateChange((_event, session) => {
      setSession(session);
    });
  }, []);

  // if (!session) {
  //   return <Auth />;
  // }

  return (
    <Switch>
      <Route exact path="/">
        <EiListContainer />
      </Route>
      <Route
        path="/profile"
        render={() => <Account key={session.user.id} session={session} />}
      />
      <Route path="/ei/:eiName/:eiId">
        <EiContainer>
          <Switch>
            <Route
              path="/ei/:eiName/:eiId/authorisation/:authorisationId"
              render={() => <EiLayout Main={AuthorisationEditor} />}
            />
            <Route
              path="/ei/:eiName/:eiId/organisations/:organisationName?/:organisationId?"
              render={() => (
                <EiLayout
                  Main={() => (
                    <EntitiesView
                      type="organisations"
                      entities={(ei) => ei.Organisations}
                    />
                  )}
                  Editor={() => (
                    <HierarchicEntityEditor
                      paramName="organisationId"
                      collection={(ei) => ei.Organisations}
                    />
                  )}
                />
              )}
            />

            <Route
              path="/ei/:eiName/:eiId/roles/:roleName?/:roleId?"
              render={() => (
                <EiLayout
                  Main={() => (
                    <EntitiesView type="roles" entities={(ei) => ei.Roles} />
                  )}
                  Editor={() => (
                    <HierarchicEntityEditor
                      paramName="roleId"
                      collection={(ei) => ei.Roles}
                    />
                  )}
                />
              )}
            />
            <Route
              path="/ei/:eiName/:eiId/types/:typeName?/:typeId?"
              render={() => (
                <EiLayout
                  Main={() => (
                    <EntitiesView type="types" entities={(ei) => ei.Types} />
                  )}
                  Editor={() => (
                    <HierarchicEntityEditor
                      paramName="typeId"
                      collection={(ei) => ei.Types}
                    />
                  )}
                />
              )}
            />
            <Route
              path="/ei/:eiName/:eiId/workflows/:name/:workflowId/action/:actionId"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={ActionView} />
              )}
            />
            <Route
              path="/ei/:eiName/:eiId/workflows/:name/:workflowId/state/:id"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={StateEditor} />
              )}
            />
            <Route
              path="/ei/:eiName/:eiId/workflows/:name/:workflowId/transition/:id"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={TransitionEditor} />
              )}
            />
            <Route
              path="/ei/:eiName/:eiId/workflows/:name/:workflowId/connection/:id"
              render={() => (
                <EiLayout
                  Main={WorkflowView}
                  Editor={ConnectionEditor as any}
                />
              )}
            />
            <Route
              exact
              path="/ei/:eiName/:eiId/execution"
              render={() => <EiLayout Main={ExecutionView} />}
            />
            <Route
              exact
              path="/ei/:eiName/:eiId"
              render={() => <EiLayout Main={EiEditor} />}
            />
          </Switch>
        </EiContainer>
      </Route>
    </Switch>
  );
};

export const render = () => {
  // render application
  ReactDOM.render(
    <Context.Provider value={new AppContext()}>
      <Router>
        <SemanticToastContainer />
        <App />
      </Router>
    </Context.Provider>,
    document.querySelector("#root")
  );
};

// set stateful modules
// setStatefulModules((name) => name.match(/store|context|apollo/) != null);
