import React from "react";

import { IObservableArray } from "mobx";
import { observer } from "mobx-react";
import { Button, DropdownItemProps, Header } from "semantic-ui-react";

import { Form, getField, Input, Label, Select } from "semantic-ui-mobx";
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

const Group = styled(Form.Group)`
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
      <Group>
        <Input size="mini" width={5} owner={getField(propertyItem, "Name")} />
        <Select
          compact
          width={4}
          fluid
          options={types}
          owner={getField(propertyItem, "Type")}
        />
        <Input
          size="mini"
          width={6}
          owner={getField(propertyItem, "DefaultValue")}
        />
        <Button
          size="mini"
          width={1}
          icon="trash"
          color="red"
          onClick={this.delete}
        />
      </Group>
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
          <Form.Group>
            <Label width={5} label="Name" />
            <Label width={4} label="Type" />
            <Label width={6} label="Default Value" />
          </Form.Group>
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
    </>
  );
});
