import * as React from 'react';

// @ts-ignore
import { Router } from 'director/build/director';
import { inject, observer } from 'mobx-react';
import DevTools from 'mobx-react-devtools';

import { Layout } from '../modules/core/layout';

type Props = {
  context?: App.Context;
};

export const App = inject('context')(
  observer(({ context }: Props) => {
    return (
      <Layout>
        { context.store.viewStore.router.view }
        <DevTools position={{right: '350px'}} />
      </Layout>
    );
  })
);

App.displayName = 'App';

interface LinkParams {
  children?: any;
  className?: string;
  to: string;
  action: Function;
  onClick?: never;
}

// @ts-ignore
export const Link = ({ children, className, to, action, onClick, ...rest }: LinkParams) => <a className={className} href={to} onClick={(e) => { e.preventDefault(); action(e); } } {...rest}>{children}</a>;

