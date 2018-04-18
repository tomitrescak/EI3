import { inject } from 'mobx-react';

import { ls } from '../ws/ls_container';
import { compose } from '../ws/utils';
import { EiLayout } from './ei_layout';
import { Ei } from './ei_model';


type Data = {
  LoadInstitution: string;
}

type Props = {
  context?: App.Context;
  params: {
    eiId: string,
    eiName: string;
  },
  views: { [index: string]: JSX.Element };
}

export const EiContainer = compose<Props>(
  inject('context'),
  ls<Data, Props>('LoadInstitution', {
    variables: ({ params }) => [params.eiId],
    cache: true,
    props: ({ ownProps, data: { loading, LoadInstitution } }) => {      
      if (!loading) {
        if (!ownProps.context.store.ei) { // do not update if refreshed
          let ei = new Ei(LoadInstitution as any, ownProps.context.store);
          ownProps.context.store.ei = ei;
          ownProps.context.Ui.history.startHistory(ei);
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
