import { Router, Ui } from "../helpers/client_helpers";
import { SocketClient } from "../modules/ws/socket_client";
import { AppStore, store } from "./store";

import "../helpers/class_helpers";
import React from "react";

// const socketUrl = 'ws://10.211.55.4:5000/wd';
const socketUrl = "ws://localhost:5000/wd";

export class AppContext {
  serverUrl: string;
  store: AppStore;
  client: SocketClient;
  Ui: typeof Ui;
  Router: typeof Router;

  constructor(cache = true) {
    this.store = store(cache, this);
    this.serverUrl = socketUrl;
    this.client = new SocketClient(this.serverUrl, this);

    this.Ui = { ...Ui };
    this.Router = Router;
  }
}

export const Context = React.createContext<AppContext>(null);

export const useAppContext = () => React.useContext(Context);
