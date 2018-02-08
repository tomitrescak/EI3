import * as React from 'react';

import { observer } from 'mobx-react';
import { style } from 'typestyle';

import { Entity } from '../ei/entity_model';

interface Props {
  entity: Entity;
}

const icon = style({ paddingRight: '8px' });

export const IconView = observer(({ entity }: Props) => (<span className={icon}>{entity.Icon}</span> ));
