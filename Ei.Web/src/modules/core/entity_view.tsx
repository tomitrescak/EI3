import * as React from 'react';

import { observer } from 'mobx-react';
import { Input, TextArea } from 'semantic-ui-mobx';
import { Header, Label } from 'semantic-ui-react';
import { style } from 'typestyle';
import { Entity } from '../ei/entity_model';
import { IconView } from './entity_icon_view';

const floatRight = style({
  float: 'right'
});

interface Props {
  entity: Entity;
  hideHeader?: boolean;
}

export const EntityEditor = observer(({ entity, hideHeader }: Props) => (
  <>
    {!hideHeader && (
      <Header dividing>
        <Header.Content>
          <IconView entity={entity} />
          {entity.Name || entity.Id || '<Empty>'}
        </Header.Content>
        <Label color="green" size="tiny" className={floatRight}>
          Id: {entity.Id}
        </Label>
      </Header>
    )}
    <Input owner={entity.fields.Name} label="Name" />
    <TextArea owner={entity.fields.Description} label="Description" />
    {entity.allowEditIcon && <Input owner={entity.fields.Icon} label="Icon" />}
  </>
));
