import * as React from "react";
import * as ReactDOM from "react-dom";

import { Provider } from "mobx-react";

import { context } from "./config/context";
import { App as Router } from "./config/router";
import { initRoutes } from "./config/routes";
import { SemanticToastContainer } from "react-semantic-toasts";

declare global {
  namespace App {
    export type Stores = {
      context: Context;
      store: Store;
    };
  }
}

export const render = () => {
  const currentContext = context();

  // start routes
  let routes = initRoutes(currentContext.store);
  currentContext.store.viewStore.startRouter(routes);

  // render application
  ReactDOM.render(
    <Provider
      store={currentContext.store}
      context={currentContext}
      client={currentContext.client}
    >
      <div>
        <SemanticToastContainer />
        <Router />
      </div>
    </Provider>,
    document.querySelector("#root")
  );
};

// set stateful modules
// setStatefulModules((name) => name.match(/store|context|apollo/) != null);
