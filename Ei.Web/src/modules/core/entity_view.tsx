import * as React from "react";

import { observer } from "mobx-react";
import { Header, Label } from "semantic-ui-react";
import { Entity } from "../ei/entity_model";
import { IconView } from "./entity_icon_view";
import styled from "@emotion/styled";
import { Formix, Input, isRequired, Select, TextArea } from "../Form";

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
  entities?: Entity[];
  hideHeader?: boolean;
}

export const EntityEditor = observer(
  ({ entity, entities, hideHeader }: Props) => {
    const options = React.useMemo(() => {
      if (entities) {
        let result = entities
          .filter((e) => e.Id !== entity.Id)
          .map((e) => ({ text: e.Name, value: e.Id }));
        result.unshift({ text: "None", value: "" });
        return result;
      }
      return [];
    }, [entities, entity]);
    return (
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

          {entities && (
            <Select
              selection
              name="ParentId"
              options={options}
              label="Parent"
            />
          )}
        </>
      </Formix>
    );
  }
);
