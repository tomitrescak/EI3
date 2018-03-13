import { Router, Ui } from '../helpers/client_helpers';
import { SocketClient } from '../modules/ws/socket_client';
import { store } from './store';

import '../helpers/class_helpers';

declare global {
  namespace App { type Context = ContextModel; }
}

// const socketUrl = 'ws://10.211.55.4:5000/wd';
const socketUrl = 'ws://localhost:5000/wd';

export class ContextModel {
  serverUrl: string;
  store: App.Store;
  client: SocketClient;
  Ui: typeof Ui;
  Router: typeof Router;

  constructor(cache = true) {
    this.store = store(cache, this);
    this.serverUrl = socketUrl;
    this.client = new SocketClient(this.serverUrl);

    this.Ui = { ...Ui };
    this.Router = Router;
  }
}

let current: ContextModel;

export function context(cache = true) {
  if (!current || !cache) {
    current = new ContextModel();
  }
  return current;
}
