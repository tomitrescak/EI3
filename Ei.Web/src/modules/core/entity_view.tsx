import * as React from "react";

import { observer } from "mobx-react";
import { Header, Label } from "semantic-ui-react";
import { Entity } from "../ei/entity_model";
import { IconView } from "./entity_icon_view";
import styled from "@emotion/styled";
import { Formix, Input, isRequired, TextArea } from "../Form";

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
  <Formix
    initialValues={entity}
    validationSchema={{
      Name: [isRequired],
    }}
  >
    <>
      {!hideHeader && (
        <Header dividing>
          <HeaderContent>
            <IconView entity={entity} />
            <div className="header">
              {entity.Name || entity.Id || "<Empty>"}
            </div>
            <Label color="green" size="tiny">
              Id: {entity.Id}
            </Label>
          </HeaderContent>
        </Header>
      )}
      <Input name={"Name"} label="Name" />
      <TextArea name={"Description"} label="Description" />
      {entity.allowEditIcon && <Input name={"Icon"} label="Icon" />}
    </>
  </Formix>
));
