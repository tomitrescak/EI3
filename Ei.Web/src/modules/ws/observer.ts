import { action, observable } from "mobx";

import { SocketClient } from "./socket_client";
import { shallowCompare } from "./utils";

// export class Observer {
//   id: number;
//   data: any = {};
//   @observable loading = true;
//   @observable version = 0;

//   client: SocketClient;
//   variables: any;
//   query: string;

//   constructor(id: number, client: SocketClient, query: string, variables: any) {
//     this.id = id;
//     this.client = client;
//     this.query = query;
//     this.variables = variables;
//   }

//   @action update(data: string) {
//     this.data = { [this.query]: data };
//     this.loading = false;
//     this.version++;

//     // save to cache
//     // localStorage.setItem('ws.' + this.variables[0], data);
//   }

//   check(variables: any) {
//     if (!shallowCompare(this.variables, variables)) {
//       this.loading = true;
//       this.variables = variables;
//       this.client.send(this.query, variables)
//     }
//   }
// }
