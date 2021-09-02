import React from "react";
import ReactDOM from "react-dom";

import { SemanticToastContainer } from "react-semantic-toasts";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import { EiListContainer } from "./modules/ei/ei_list";
import { AppContext, Context } from "./config/context";
import { EiContainer } from "./modules/ei/ei_container";
import { EiLayout } from "./modules/ei/ei_layout";
import { EiEditor } from "./modules/ei/ei_editor";
import { ExecutionView } from "./modules/execution/execution_view";
import { AuthorisationEditor } from "./modules/authorisations/authorisation_editor";
import { EntitiesView } from "./modules/diagrams/entities_view";

import { configure } from "mobx";
import { HierarchicEntityEditor } from "./modules/diagrams/hierarchic_entity_editor";
configure({ enforceActions: "never" });

export const render = () => {
  // render application
  ReactDOM.render(
    <Context.Provider value={new AppContext()}>
      <Router>
        <SemanticToastContainer />
        <Switch>
          <Route exact path="/">
            <EiListContainer />
          </Route>
          <Route path="/ei/:eiName/:eiId">
            <EiContainer>
              <Switch>
                <Route
                  path="/ei/:eiName/:eiId/authorisation/:authorisationId"
                  render={() => <EiLayout Main={AuthorisationEditor} />}
                />
                <Route
                  path="/ei/:eiName/:eiId/organisations/:organisationName/:organisationId"
                  render={() => (
                    <EiLayout
                      Main={() => (
                        <EntitiesView
                          id={null}
                          type="organisations"
                          entities={(ei) => ei.Organisations}
                        />
                      )}
                      Editor={() => (
                        <HierarchicEntityEditor
                          paramName="organisationId"
                          name={"organisation"}
                          collection={(ei) => ei.Organisations}
                          minCount={1}
                          parentView="organisations"
                        />
                      )}
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
      </Router>
    </Context.Provider>,
    document.querySelector("#root")
  );
};

// set stateful modules
// setStatefulModules((name) => name.match(/store|context|apollo/) != null);
