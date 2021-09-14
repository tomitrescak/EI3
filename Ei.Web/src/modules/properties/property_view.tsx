import React from "react";

import { IObservableArray } from "mobx";
import { observer } from "mobx-react";
import { Button, DropdownItemProps, Header } from "semantic-ui-react";

import { getField, Input, Label, Select } from "semantic-ui-mobx";
import { Property } from "../ei/property_model";
import { AppContext, useAppContext } from "../../config/context";
import styled from "@emotion/styled";

interface IPropertyOwner {
  Properties: IObservableArray<Property>;
}

interface PropertyItemProps {
  context: AppContext;
  owner: IPropertyOwner;
  propertyItem: Property;
  types: DropdownItemProps[];
}

// const Group = styled(Form.Group)`
//   .dropdown {
//     font-size: 0.78571429em !important;
//     line-height: 1.21428571em;
//     padding: 0.67857143em 1em;
//   }
// `;

const Row = styled.div`
  display: flex;
  align-items: center;

  > *:nth-child(1) {
    flex: 3;

    input {
      border-top-right-radius: 0 !important;
      border-bottom-right-radius: 0 !important;
    }
  }

  > *:nth-child(2) {
    flex: 2;

    .dropdown {
      border-radius: 0 !important;
    }
  }

  > *:nth-child(3) {
    flex: 3;

    input {
      border-radius: 0 !important;
    }
  }

  > *:nth-child(4) {
    flex: 0 0 30px;
  }

  button {
    border-top-left-radius: 0 !important;
    border-bottom-left-radius: 0 !important;
  }

  > * {
    margin: 0 !important;
  }

  .dropdown {
    font-size: 0.78571429em !important;
    line-height: 1.21428571em;
    padding: 0.67857143em 1em;
  }
`;

class PropertyItem extends React.Component<PropertyItemProps> {
  delete = async () => {
    if (
      await this.props.context.Ui.confirmDialogAsync(
        "Do you want to delete this property?"
      )
    ) {
      this.props.owner.Properties.splice(
        this.props.owner.Properties.indexOf(this.props.propertyItem),
        1
      );
    }
  };
  render() {
    const { propertyItem, types } = this.props;
    return (
      <Row>
        <Input size="mini" fluid owner={getField(propertyItem, "Name")} />
        <Select
          compact
          fluid
          size="mini"
          options={types}
          owner={getField(propertyItem, "Type")}
        />
        <Input
          fluid
          size="mini"
          owner={getField(propertyItem, "DefaultValue")}
        />
        <Button
          size="mini"
          width={1}
          icon="trash"
          color="red"
          onClick={this.delete}
        />
      </Row>
    );
  }
}

interface Props {
  owner: IPropertyOwner;
  types: DropdownItemProps[];
}

const addField = async (props: Props, context: AppContext) => {
  let name = await context.Ui.promptText(
    "What is the name of the new property?"
  );
  if (name.value) {
    props.owner.Properties.push(
      new Property({ Name: name.value, Type: "int", DefaultValue: "0" })
    );
  }
};

const sort = (props: Props) => {
  props.owner.Properties.sort((a, b) => (a.Name < b.Name ? -1 : 1));
};

export const PropertyView = observer((props: Props) => {
  const context = useAppContext();

  return (
    <>
      <Header as="h4" icon="browser" content="Properties" dividing />

      {props.owner.Properties.length > 0 && (
        <>
          <Row>
            <Label width={5} label="Name" />
            <Label width={4} label="Type" />
            <Label width={6} label="Default Value" />
            <div />
          </Row>
          {props.owner.Properties.map((property, index) => (
            <PropertyItem
              key={index}
              propertyItem={property}
              types={props.types}
              context={context}
              owner={props.owner}
            />
          ))}
        </>
      )}

      <div style={{ marginTop: 8 }}>
        <Button
          type="button"
          content="Add Property"
          primary
          icon="plus"
          labelPosition="left"
          onClick={() => addField(props, context)}
        />

        <Button
          type="button"
          floated="right"
          title="Sort"
          default
          icon="sort"
          onClick={() => sort(props)}
        />
      </div>
    </>
  );
});
