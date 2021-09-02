import * as React from "react";

import { inject, observer } from "mobx-react";

import { ChildProps, ComponentDecorator, Options } from "./interface";
// import { Observer } from './observer';
import { SocketClient } from "./socket_client";

interface WProps {
  client: SocketClient;
}

// export function ws<DataResult = {}, QueryProps = {}, TChildProps = ChildProps<QueryProps, DataResult>>(
//   query: any,
//   { props = null, name = 'data', variables, waitForData, loadingComponent }: Options<DataResult, QueryProps> = {}
// ): ComponentDecorator<QueryProps, TChildProps> {
//   return function(Wrapper) {
//     @inject('client')
//     @observer
//     class ApolloWrappedContainer extends React.Component<WProps, {}> {
//       observe: Observer;

//       render() {
//         const receivedData = {
//           ...this.observe.data,
//           loading: this.observe.loading,
//           version: this.observe.version
//         };
//         const modifiedProps = props ? props({ [name]: receivedData, ownProps: this.props } as any) : {};
//         const newProps = {
//           ...this.props,
//           [name]: receivedData,
//           ...modifiedProps
//         };

//         // we may request that we want to render only loading component until data is loaded
//         if (receivedData.loading && waitForData) {
//           const loading = loadingComponent ? loadingComponent() : SocketClient.loadingComponent();
//           if (!loading) {
//             throw new Error('WSContainer: Loading component for "waitForData" is not defined.');
//           }
//           return loading;
//         }

//         return <Wrapper {...newProps} />;
//       }

//       componentWillUpdate(_nextProps: WProps) {
//         // const client = nextProps.client;
//         this.observe.check(variables);
//       }

//       componentWillMount() {
//         const client = this.props.client;
//         this.observe = client.send(query, variables(this.props as any));
//       }
//     }

//     return ApolloWrappedContainer as any;
//   };
// }
