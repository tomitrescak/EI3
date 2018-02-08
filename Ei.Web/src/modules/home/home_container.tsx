import * as React from 'react';
import { Button } from 'semantic-ui-react';
import { Ui } from '../../helpers/client_helpers';

export const HomeContainer = () => (<div>Home<Button onClick={() => Ui.alert('qwqeqe') } content="FFF" /></div>)