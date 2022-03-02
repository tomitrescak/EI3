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
import { AuthorisationEditor } from "./modules/authorisations/authorisation_editor";
import { EntitiesView } from "./modules/diagrams/entities_view";
// import { supabase } from "./config/supabase";
import { configure } from "mobx";
import { HierarchicEntityEditor } from "./modules/diagrams/hierarchic_entity_editor";
// import Account from "./auth/account";
import { WorkflowView } from "./modules/workflow/workflow_view";
import { ActionView } from "./modules/actions/action_view";
import { StateEditor } from "./modules/states/state_editor";
import { TransitionEditor } from "./modules/transitions/transitions_editor";
import { ConnectionEditor } from "./modules/connections/connection_editor";
import { WorkflowEditor } from "./modules/workflow/workflow_editor";
import { observer } from "mobx-react";
import { ExperimentEditor } from "./modules/experiments/experiment_editor";

import "react-semantic-toasts/styles/react-semantic-alert.css";

configure({ enforceActions: "never" });

const App = () => {
  // init router
  const history = useHistory();
  const context = useAppContext();
  context.assignRouter(history);

  // const [session, setSession] = useState(null);
  // useEffect(() => {
  //   setSession(supabase.auth.session());

  //   supabase.auth.onAuthStateChange((_event, session) => {
  //     setSession(session);
  //   });
  // }, []);

  // if (!session) {
  //   return <Auth />;
  // }

  return (
    <Switch>
      <Route exact path="/">
        <EiListContainer />
      </Route>
      {/* <Route
        path="/profile"
        render={() => <Account key={session.user.id} session={session} />}
      /> */}
      <Route path="/ei/:eiName">
        <EiContainer>
          <Switch>
            <Route
              path="/ei/:eiName/authorisation"
              render={() => <EiLayout Main={AuthorisationEditor} />}
            />
            <Route
              path="/ei/:eiName/organisations/:organisationName?"
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
                      collection={(ei) => ei.Organisations}
                    />
                  )}
                />
              )}
            />

            <Route
              path="/ei/:eiName/roles/:roleName?"
              render={() => (
                <EiLayout
                  Main={() => (
                    <EntitiesView type="roles" entities={(ei) => ei.Roles} />
                  )}
                  Editor={() => (
                    <HierarchicEntityEditor collection={(ei) => ei.Roles} />
                  )}
                />
              )}
            />
            <Route
              path="/ei/:eiName/types/:typeName?"
              render={() => (
                <EiLayout
                  Main={() => (
                    <EntitiesView type="types" entities={(ei) => ei.Types} />
                  )}
                  Editor={() => (
                    <HierarchicEntityEditor collection={(ei) => ei.Types} />
                  )}
                />
              )}
            />

            <Route
              path="/ei/:eiName/workflows/:workflowName/action/:actionName?"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={ActionView} />
              )}
            />
            <Route
              path="/ei/:eiName/workflows/:name/state/:stateName?"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={StateEditor} />
              )}
            />
            <Route
              path="/ei/:eiName/workflows/:name/transition/:transitionName?"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={TransitionEditor} />
              )}
            />
            <Route
              path="/ei/:eiName/workflows/:name/connection/:connectionName?"
              render={() => (
                <EiLayout
                  Main={WorkflowView}
                  Editor={ConnectionEditor as any}
                />
              )}
            />
            <Route
              path="/ei/:eiName/workflows/:name"
              render={() => (
                <EiLayout Main={WorkflowView} Editor={WorkflowEditor} />
              )}
            />

            <Route
              exact
              path="/ei/:eiName/experiment/:experimentName?"
              render={() => <EiLayout Main={ExperimentEditor} />}
            />
            {/* <Route
              exact
              path="/ei/:eiName/execution"
              render={() => <EiLayout Main={ExecutionView} />}
            /> */}
            <Route
              exact
              path="/ei/:eiName"
              render={() => <EiLayout Main={EiEditor} />}
            />

            {/* <Route
              path="/ei/:eiName/execution"
              render={() => (
                <EiLayout
                  Main={() => <ExecutionView />}
                  Editor={() => <div></div>}
                />
              )}
            /> */}
          </Switch>
        </EiContainer>
      </Route>
    </Switch>
  );
};

// const ReactiveContext1 = observer(() => {
//   const context = React.useMemo(() => new AppContext(), []);

//   // this subscribes to undo stack
//   console.log("Rendering Version: " + context.Ui.history.version);

//   // subscribe
//   return (
//     <Context.Provider value={context}>
//       <Router>
//         <SemanticToastContainer />
//         <App />
//       </Router>
//     </Context.Provider>
//   );
// });

export const ReactiveContext = observer(() => {
  const context = React.useMemo(() => new AppContext(), []);

  // this subscribes to undo stack
  console.log("Rendering Version: " + context.Ui.history.version);

  // subscribe
  return (
    <Context.Provider value={context}>
      <Router>
        <SemanticToastContainer />
        <App />
      </Router>
    </Context.Provider>
  );
});

// export const render = () => {
//   // render application
//   ReactDOM.render(<ReactiveContext />, document.querySelector("#root"));
// };

// set stateful modules
// setStatefulModules((name) => name.match(/store|context|apollo/) != null);

// const App1 = () => <div>wewewe</div>;

export default ReactiveContext;
