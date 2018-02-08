import * as React from 'react';
import * as AlertTemplate from 'react-alert-template-basic';

import { Provider, withAlert } from 'react-alert';

const View = withAlert(({ alert, context }: any) => {
  context.Ui.alerter = () => alert;
  return false;
});

interface Props {
  context: App.Context;
}

export const AlertViewConfig = ({ context }: Props) => (
  <Provider template={AlertTemplate} {...{
    position: 'top right',
    timeout: 3000,
    offset: '10px',
    transition: 'fade',
    theme: 'dark'
  }}>
    <View context={context} />
  </Provider>
)