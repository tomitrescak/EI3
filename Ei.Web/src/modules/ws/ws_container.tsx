import * as React from 'react';

import { inject, observer } from 'mobx-react';

import { ChildProps, ComponentDecorator, Options } from './interface';
import { Observer } from './observer';
import { SocketClient } from './socket_client';

let data = require('./ConnectionTest.json');
if (localStorage.getItem('ws.Connection Test 1') == null) {
  localStorage.setItem('ws.Connection Test 1', JSON.stringify(data));
}


interface WProps {
  client: SocketClient;
}

export function ws<DataResult = {}, QueryProps = {}, TChildProps = ChildProps<QueryProps, DataResult>>(
  query: any,
  { props = null, name = 'data', variables, waitForData, cache, loadingComponent }: Options<DataResult, QueryProps> = {}
): ComponentDecorator<QueryProps, TChildProps> {
  return function(Wrapper) {
    @inject('client')
    @observer
    class WebSocketContainer extends React.Component<WProps, {}> {
      observe: Observer;
      cachedData: any;

      render() {
        
        // TEMPORARY Disable
        const receivedData = cache && localStorage.getItem('ws.' + variables[0])
        ? {
          [query]: JSON.parse(localStorage.getItem('ws.' + variables[0])),
          loading: false,
          version: 1
        }
        : {
          ...this.observe.data,
          loading: this.observe.loading,
          version: this.observe.version
        };
        const modifiedProps = props ? props({ [name]: receivedData, ownProps: this.props } as any) : {};
        const newProps = {
          ...this.props,
          [name]: receivedData,
          ...modifiedProps
        };

        // we may request that we want to render only loading component until data is loaded
        if (receivedData.loading && waitForData) {
          const loading = loadingComponent ? loadingComponent() : SocketClient.loadingComponent();
          if (!loading) {
            throw new Error('WSContainer: Loading component for "waitForData" is not defined.');
          }
          return loading;
        }

        return <Wrapper {...newProps} />;
      }

      // componentWillUpdate(nextProps: WProps) {
      //   const client = nextProps.client;
      //   this.observe.check(variables);
      // }

      // componentWillMount() {
      //   const client = this.props.client;
      //   this.observe = client.send(query, variables);
      // }
    }

    return WebSocketContainer as any;
  };
}