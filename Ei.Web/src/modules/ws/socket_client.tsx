import * as React from "react";

import { observable } from "mobx";
import { AppContext } from "../../config/context";

enum SocketState {
  Closed = "Closed",
  Closing = "Closing",
  Connecting = "Connecting",
  Open = "Open",
  Unknown = "Unknown",
}

type ErrorHandler = (message: any) => void;

enum MessageType {
  Text = 0,
  MethodInvocation = 1,
  ConnectionEvent = 2,
}

interface SocketMessage {
  messageType: MessageType;
  data: string;
}

interface MethodInfo {
  methodName: string;
  arguments: [number, ...any[]];
}

let queryUid = 0;

// export class QueryHandler {
//   private observers: Observer[] = [];

//   createObserver(
//     client: SocketClient,
//     query: string,
//     variables: any[] = [],
//     observer?: Observer
//   ) {
//     let id = queryUid++;

//     // initialise observer
//     if (!observer) {
//       observer = new Observer(id, client, query, variables);
//     } else {
//       observer.id = id;
//     }
//     this.observers.push(observer);
//     return observer;
//   }

//   queryResult(observerIdString: string, data: string) {
//     let observerId = parseInt(observerIdString, 10);
//     let result = typeof data === "string" ? JSON.parse(data) : data;
//     let observer = this.observers.find((o) => o.id === observerId);
//     observer.update(result);

//     // remove this observer
//     this.observers.splice(this.observers.indexOf(observer), 1);
//   }

//   subscribe(observerIdString: string, data: string) {
//     let observerId = parseInt(observerIdString, 10);
//     let result = typeof data === "string" ? JSON.parse(data) : data;
//     let observer = this.observers.find((o) => o.id === observerId);
//     observer.update(result);
//   }
// }

type SendOptions<T = any> = {
  query: string;
  variables?: any[];
  receiver?: T;
  isSubscription?: boolean;
};

let uid: number = 0;

export class SocketClient {
  static loadingComponent = () => <div>Loading ...</div>;

  @observable state: SocketState;
  @observable error: string;

  connectionId: string;
  url: string;
  socket: WebSocket;

  handlers: Map<string, Map<number, SendOptions>> = new Map();

  // private messageHandler: QueryHandler;
  private errorHandler: ErrorHandler;

  constructor(
    url: string,
    private context: AppContext,
    errorHandler?: ErrorHandler
  ) {
    this.url = url;
    // this.messageHandler = messageHandler;
    this.errorHandler = errorHandler;
  }

  async close() {
    if (!this.socket || this.state !== SocketState.Open) {
      return;
    }
    this.socket.close(1000, "Closing from client");
  }

  unsubscribe(query: string, id: number) {
    this.handlers.get(query).delete(id);
  }

  send(options: SendOptions): number {
    // observer = this.messageHandler.createObserver(
    //   this,
    //   query,
    //   variables,
    //   observer
    // );

    const { query, receiver } = options;
    let id = uid++;

    if (receiver) {
      // add handler
      if (!this.handlers.has(query)) {
        this.handlers.set(query, new Map());
      }
      this.handlers.get(query).set(id, options);
    }

    let variables = [id].concat(options.variables || []);

    this.connect().then(() => {
      this.socket.send(
        JSON.stringify({ methodName: query, arguments: variables })
      );
    });

    return id;
  }

  async connect() {
    return new Promise<void>((resolve, reject) => {
      // we may be already connected
      if (this.state === SocketState.Open) {
        resolve();
        return;
      }

      try {
        this.socket = new WebSocket(this.url);

        this.socket.onopen = () => {
          this.state = SocketState.Open;
          resolve();
        };

        this.socket.onclose = () => {
          this.state = SocketState.Closed;
          reject();
        };

        this.socket.onerror = (event) => {
          this.state = SocketState.Closed;

          if (this.errorHandler) {
            this.errorHandler(event);
          }
          this.context.Ui.alertError("Server Connection Error");

          reject(event);
        };

        this.socket.onmessage = (event) => {
          const message: SocketMessage = JSON.parse(event.data);
          if (message.messageType === MessageType.ConnectionEvent) {
            this.connectionId = message.data;
          } else if (message.messageType === MessageType.MethodInvocation) {
            const methodInfo: MethodInfo = JSON.parse(message.data);

            const handler = this.handlers
              .get(methodInfo.methodName)
              ?.get(methodInfo.arguments[0]);

            if (handler?.receiver) {
              handler.receiver(...methodInfo.arguments.slice(1));

              // remove if this is not a subscription
              if (!handler.isSubscription) {
                this.handlers
                  .get(methodInfo.methodName)
                  .delete(methodInfo.arguments[0]);
              }
            }
          } else {
            // tslint:disable-next-line:no-console
            console.warn(event.data);
          }
        };
      } catch (ex) {
        this.context.Ui.alertError(ex.message);
      }
    });
  }
}
