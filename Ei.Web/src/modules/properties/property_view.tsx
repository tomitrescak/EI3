import * as React from "react";

import { IObservableArray } from "mobx";
import { inject, observer } from "mobx-react";
import { Button, DropdownItemProps, Header } from "semantic-ui-react";

import { Form, getField, Input, Label, Select } from "semantic-ui-mobx";
import { Property } from "../ei/property_model";

interface IPropertyOwner {
  Properties: IObservableArray<Property>;
}

interface PropertyItemProps {
  context: App.Context;
  owner: IPropertyOwner;
  propertyItem: Property;
  types: DropdownItemProps[];
}

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
      <Form.Group>
        <Input width={5} owner={getField(propertyItem, "Name")} />
        <Select
          width={4}
          fluid
          options={types}
          owner={getField(propertyItem, "Type")}
        />
        <Input width={6} owner={getField(propertyItem, "DefaultValue")} />
        <Button width={1} icon="trash" color="red" onClick={this.delete} />
      </Form.Group>
    );
  }
}

interface Props {
  context?: App.Context;
  owner: IPropertyOwner;
  types: DropdownItemProps[];
}

let index: number;
let property: Property;

@inject("context")
@observer
export class PropertyView extends React.Component<Props> {
  addField = async () => {
    let name = await this.props.context.Ui.promptText(
      "What is the name of the new property?"
    );
    if (name.value) {
      this.props.owner.Properties.push(
        new Property({ Name: name.value, Type: "int", DefaultValue: "0" })
      );
    }
  };

  sort = () => {
    this.props.owner.Properties.sort((a, b) => (a.Name < b.Name ? -1 : 1));
  };

  render() {
    return (
      <>
        <Header as="h4" icon="browser" content="Properties" dividing />

        {this.props.owner.Properties.length > 0 && (
          <>
            <Form.Group>
              <Label width={5} label="Name" />
              <Label width={4} label="Type" />
              <Label width={6} label="Default Value" />
            </Form.Group>
            {this.props.owner.Properties.map((property, index) => (
              <PropertyItem
                key={index}
                propertyItem={property}
                types={this.props.types}
                context={this.props.context}
                owner={this.props.owner}
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
          onClick={this.addField}
        />

        <Button
          type="button"
          floated="right"
          title="Sort"
          default
          icon="sort"
          onClick={this.sort}
        />
      </>
    );
  }
}
