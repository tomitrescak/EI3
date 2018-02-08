// @ts-ignore
import { Router } from 'director/build/director';

import { autorun, computed } from 'mobx';

export interface Route {
  name: string;
  route: string;
  component?: () => JSX.Element;
  components?: (params: { [index: string]: string }) => { [index: string]: JSX.Element };
  action: (...params: string[]) => void;
}

export class MobxRouter {
  routes: { [name: string]: Route };
  store: App.ViewStore;

  constructor(store: App.ViewStore) {
    this.store = store;
  }

  @computed
  get currentPath() {
    let route = this.routes[this.store.view].route;

    // replace all parameters with :name
    Object.getOwnPropertyNames(this.store.viewParameters).forEach(
      p => (route = route.replace(':' + p, this.store.viewParameters[p]))
    );
    return route;
  }

  @computed
  get view() {
    return this.routes[this.store.view].components(this.store.viewParameters);
  }

  startRouter(r: Route[]) {
    // initialise routes from route definition
    this.routes = {};
    for (let route of r) {
      this.routes[route.name] = route;
    }

    // initialise external router to parse the initial routes
    let keys = Object.getOwnPropertyNames(this.routes);
    let routes: any = {};

    for (let key of keys) {
      routes[this.routes[key].route] = this.routes[key].action;
    }

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
