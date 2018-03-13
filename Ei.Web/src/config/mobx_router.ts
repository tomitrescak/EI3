import * as React from 'react';

// @ts-ignore
import { Router } from 'director/build/director';

import { autorun, computed } from 'mobx';

export interface Route {
  name?: string;
  route?: string;
  component?: (params?: any) => JSX.Element;
  components?: (params: { [index: string]: string }) => { [index: string]: JSX.Element };
  action?: (...params: string[]) => void;
  children?: Route[];
  layout?: (params?: any) => JSX.Element;
}

export class MobxRouter {
  routes: { [name: string]: Route };
  store: App.ViewStore;
  context: App.Context;

  constructor(context: App.Context) {
    this.context = context;
    this.store = context.store.viewStore;
  }

  @computed
  get currentPath() {
    if (!this.routes[this.store.view]) {
      throw new Error('Route does not exists for view: ' + this.store.view);
    }
    let route = this.routes[this.store.view].route;

    // replace all parameters with :name
    Object.getOwnPropertyNames(this.store.viewParameters).forEach(
      p => (route = route.replace(':' + p, this.store.viewParameters[p]))
    );
    return route;
  }

  @computed
  get view() {
    let route = this.routes[this.store.view];

    // if there is no layout just return component
    if (!route.layout) {
      return React.createElement(route.component, {
        params: this.store.viewParameters,
        context: this.context
      });
    }
    return React.createElement(route.layout, {
      views: route.components(this.store.viewParameters),
      params: this.store.viewParameters,
      context: this.context
    });
  }

  addRoutes(r: Route[], parentRoute: Route, routes: any) {
    // initialise routes from route definition
    for (let route of r) {
      this.routes[route.name] = route;

      if (parentRoute) {
        if (parentRoute.component) {
          route.layout = parentRoute.component;
        }
        if (parentRoute.route) {
          route.route = parentRoute.route + route.route;
        }
      }

      // add children
      if (route.children) {
        this.addRoutes(route.children, route, routes);
      }
    }

    // initialise external router to parse the initial routes
    let keys = Object.getOwnPropertyNames(this.routes);

    for (let key of keys) {
      routes[this.routes[key].route] = this.routes[key].action;
    }
  }

  startRouter(r: Route[]) {
    this.routes = {};
    const routes = {};
    this.addRoutes(r, null, routes);

    const router = new Router(routes);
    router.configure({
      notfound: () => this.store.showView('notFound'),
      html5history: true
    });
    router.init();

    // update url on state changes
    autorun(() => {
      const path = this.currentPath;
      if (path !== window.location.pathname) {
        window.history.pushState(null, null, path);
      }
    });
  }
}
