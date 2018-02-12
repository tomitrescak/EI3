import * as React from 'react';

import { inject, observer } from 'mobx-react';

import { ChildProps, ComponentDecorator, Options } from './interface';

// tslint:disable-next-line:no-empty-interface
interface WProps {}

export function ls<
  DataResult = {},
  QueryProps = {},
  TChildProps = ChildProps<QueryProps, DataResult>
>(
  query: any,
  {
    props = null,
    name = 'data',
    variables
  }: Options<DataResult, QueryProps> = {}
): ComponentDecorator<QueryProps, TChildProps> {
  return function(Wrapper) {
    @inject('client')
    @observer
    class WebSocketContainer extends React.Component<WProps, {}> {
      cachedData: any;

      render() {
        const vars = variables(this.props as any);
        const storedName = 'ws.' + vars[0];
        // TEMPORARY Disable
        const receivedData =
          localStorage.getItem(storedName)
            ? {
                [query]: JSON.parse(localStorage.getItem(storedName)),
                loading: false,
                version: 1
              }
            : null;
        
        if (!receivedData) {
          return <div>Could not find the institution in your local storage: {storedName}</div>
        }

        const modifiedProps = props
          ? props({ [name]: receivedData, ownProps: this.props } as any)
          : {};
        const newProps = {
          ...this.props,
          [name]: receivedData,
          ...modifiedProps
        };

        // we may request that we want to render only loading component until data is loaded
        return <Wrapper {...newProps} />;
      }
    }

    return WebSocketContainer as any;
  };
}
