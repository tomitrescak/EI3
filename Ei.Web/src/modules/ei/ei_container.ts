import { inject } from 'mobx-react';

import { compose } from '../ws/utils';
import { ws } from '../ws/ws_container';
import { EiLayout } from './ei_layout';
import { Ei } from './ei_model';


type Data = {
  LoadInstitution: string;
}

type Props = {
  context?: App.Context;
  views: { [index: string]: JSX.Element };
}

export const EiContainer = compose<Props>(
  inject('context'),
  ws<Data, Props>('LoadInstitution', {
    variables: ['Connection Test 1'],
    cache: true,
    props: ({ ownProps, data: { loading, LoadInstitution } }) => {      
      if (!loading) {
        if (!ownProps.context.store.ei) { // do not update if refreshed
          let ei = new Ei(LoadInstitution as any, ownProps.context.store);
          ownProps.context.store.ei = ei;
        }
      }
      return {
        loading,
        load: JSON.stringify(LoadInstitution),
        context: ownProps.context,
        views: ownProps.views
      };
    }
  })
)(EiLayout);

EiContainer.displayName = 'EiContainer';
