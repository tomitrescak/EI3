import * as React from "react";

import { observer } from "mobx-react";
import { Input, TextArea } from "semantic-ui-mobx";
import { Header, Label } from "semantic-ui-react";
import { Entity } from "../ei/entity_model";
import { IconView } from "./entity_icon_view";
import styled from "@emotion/styled";

const HeaderContent = styled(Header.Content)`
  width: 100%;
  display: flex !important;
  align-items: center;

  .header {
    flex: 1;
  }
`;

interface Props {
  entity: Entity;
  hideHeader?: boolean;
}

export const EntityEditor = observer(({ entity, hideHeader }: Props) => (
  <>
    {!hideHeader && (
      <Header dividing>
        <HeaderContent>
          <IconView entity={entity} />
          <div className="header">{entity.Name || entity.Id || "<Empty>"}</div>
          <Label color="green" size="tiny">
            Id: {entity.Id}
          </Label>
        </HeaderContent>
      </Header>
    )}
    <Input owner={entity.fields.Name} label="Name" />
    <TextArea owner={entity.fields.Description} label="Description" />
    {entity.allowEditIcon && <Input owner={entity.fields.Icon} label="Icon" />}
  </>
));
