import * as React from 'react';

import { observable } from 'mobx';
import { Observer } from './observer';

enum SocketState {
  Closed = 'Closed',
  Closing = 'Closing',
  Connecting = 'Connecting',
  Open = 'Open',
  Unknown = 'Unknown'
}
 
type ErrorHandler = (message: any) => void;

enum MessageType {
  Text = 0,
  MethodInvocation = 1,
  ConnectionEvent = 2
}

interface SocketMessage {
  messageType: MessageType;
  data: string;
}

interface MethodInfo {
  methodName: string;
  arguments: string[];
}

let queryUid = 0;

export class QueryHandler  {
  private observers: Observer[] = [];

  createObserver(client: SocketClient, query: string, variables: any[] = [], observer?: Observer) {
    let id = queryUid++;

    // initialise observer
    if (!observer) {
      observer = new Observer(id, client, query, variables);
    } else {
      observer.id = id;
    }
    this.observers.push(observer);
    return observer;
  }

  queryResult(observerIdString: string, data: string) {
    let observerId = parseInt(observerIdString, 10);
    let result = JSON.parse(data);
    let observer = this.observers.find(o => o.id === observerId);
    observer.update(result);

    // remove this observer
    this.observers.splice(this.observers.indexOf(observer), 1);
  }
}

export class SocketClient {
  static loadingComponent = () => <div>Loading ...</div>;
  
  @observable state: SocketState;
  @observable error: string;

  connectionId: string;
  url: string;
  socket: WebSocket;
  

  private messageHandler: QueryHandler;
  private errorHandler: ErrorHandler;
  

  constructor(url: string, messageHandler = new QueryHandler(), errorHandler?: ErrorHandler) {
    this.url = url;
    this.messageHandler = messageHandler;
    this.errorHandler = errorHandler;
  }

  async close() {
    if (!this.socket || this.state !== SocketState.Open) {
      return;
    }
    this.socket.close(1000, 'Closing from client');
  }

  send(query: string, variables: any[] = [], observer?: Observer): Observer {
    observer = this.messageHandler.createObserver(this, query, variables, observer);

    variables = [observer.id, ...variables];

    this.connect().then(() => {
      this.socket.send(JSON.stringify({ methodName: query, arguments: variables }));
    })
    
    return observer;
  }

  async connect() {
    return new Promise((resolve, reject) => {
      // we may be already connected
      if (this.state === SocketState.Open) {
        resolve();
      }

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

        if (this.messageHandler) {
          this.errorHandler(event);
        }
        reject(event);
      };
  
      this.socket.onmessage = (event) => {
        const message: SocketMessage = JSON.parse(event.data);
        if (message.messageType === MessageType.ConnectionEvent) {
          this.connectionId = message.data;
        } else if (message.messageType === MessageType.MethodInvocation) {
          const methodInfo: MethodInfo = JSON.parse(message.data);
          this.messageHandler[methodInfo.methodName](...methodInfo.arguments);
        } else {
          // tslint:disable-next-line:no-console
          console.warn(event.data);
        }
      };
    })
    
  }
}
