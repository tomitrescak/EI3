import React from "react";

import { observer } from "mobx-react";
import { Checkbox, Form, Input, Label, Radio, Select } from "semantic-ui-mobx";
import {
  Accordion,
  DropdownItemProps,
  Header,
  Icon,
  Label as SUILabel,
} from "semantic-ui-react";
import { style } from "typestyle";

import { action } from "mobx";
import { Ui } from "../../helpers/client_helpers";
import { AccessEditor } from "../access/access_editor";
import { IconView } from "../core/entity_icon_view";
import { EntityEditor } from "../core/entity_view";
import { ActionDisplayType, Connection } from "../ei/connection_model";
import { AppContext, Context } from "../../config/context";

interface Props {
  id: string;
  workflowId: string;
}

const floatRight = style({
  float: "right",
});

const limited = style({
  $nest: {
    ".accordion.ui": {
      margin: "0px!important",
    },
  },
});

const emptyOptions: DropdownItemProps[] = [];

const statePositionOptions = [
  { text: "North", value: "north" },
  { text: "NorthEast", value: "northeast" },
  { text: "East", value: "east" },
  { text: "South East", value: "southeast" },
  { text: "South", value: "south" },
  { text: "South West", value: "southwest" },
  { text: "West", value: "west" },
  { text: "North West", value: "northwest" },
];

const joinToOptions = [
  { text: "Join 1", value: "join1" },
  { text: "Join 2", value: "join3" },
  { text: "Join 3", value: "join4" },
  { text: "Disconnect", value: "" },
];

const joinFromOptions = [
  { text: "Yield", value: "yield" },
  { text: "Disconnect", value: "" },
];

const splitFromOptions = [
  { text: "Split 1", value: "split1" },
  { text: "Split 2", value: "split2" },
  { text: "Split 3", value: "split3" },
  { text: "Disconnect", value: "" },
];

const splitToOptions = [
  { text: "Input", value: "input" },
  { text: "Disconnect", value: "" },
];

@observer
export class ConnectionEditor extends React.Component<Props> {
  connection: Connection;
  static contextType = Context;
  context: AppContext;

  changeSourcePort = action((_e: any, { value }: any) => {
    const workflow = this.connection.workflow;
    const link = this.connection.link;

    this.connection.SourcePort = value;

    const fromPosition = workflow.findPosition(this.connection.From);
    link.sourcePort.removeLink(link);
    link.setSourcePort(fromPosition.getPort(value));

    this.connection.workflow.Connections.remove(this.connection);
    this.connection.workflow.Connections.push(this.connection);

    Ui.history.step();
  });

  changeSourcePosition = action((_e: any, { value }: any) => {
    const workflow = this.connection.workflow;
    const link = this.connection.link;
    link.sourcePort.removeLink(link);

    const fromPosition = workflow.findPosition(value);
    if (fromPosition) {
      const port = fromPosition.ports[Object.keys(fromPosition.ports)[0]];
      this.connection.SourcePort = port.name;
      link.setSourcePort(port);

      this.connection.From = value;
    } else {
      this.connection.From = "";
      this.connection.SourcePort = null;
    }

    this.connection.update();
    this.connection.workflow.Connections.remove(this.connection);
    this.connection.workflow.Connections.push(this.connection);

    Ui.history.step();
  });

  changeTargetPort = action((_e: any, { value }: any) => {
    const workflow = this.connection.workflow;
    const link = this.connection.link;

    this.connection.TargetPort = value;

    const toPosition = workflow.findPosition(this.connection.To);
    link.targetPort.removeLink(link);
    link.setTargetPort(toPosition.getPort(value));

    this.connection.workflow.Connections.remove(this.connection);
    this.connection.workflow.Connections.push(this.connection);

    Ui.history.step();
  });

  changeTargetPosition = action((_e: any, { value }: any) => {
    const workflow = this.connection.workflow;
    const link = this.connection.link;

    link.targetPort.removeLink(link);

    const toPosition = workflow.findPosition(value);
    if (toPosition) {
      let port = toPosition.ports[Object.keys(toPosition.ports)[0]];
      this.connection.TargetPort = port.name;
      link.setTargetPort(port);

      this.connection.To = value;
    } else {
      this.connection.To = "";
      this.connection.TargetPort = null;
    }
    this.connection.update();
    this.connection.workflow.Connections.remove(this.connection);
    this.connection.workflow.Connections.push(this.connection);

    Ui.history.step();
  });

  componentWillMount() {
    this.context.selectWorkflowElement(
      this.props.workflowId,
      "Connections",
      this.props.id,
      "link"
    );
  }

  componentWillUpdate(props: Props) {
    this.context.selectWorkflowElement(
      props.workflowId,
      "Connections",
      props.id,
      "link"
    );
  }

  render() {
    const { id } = this.props;
    let ei = this.context.ei;

    let workflow = ei.Workflows.find((w) => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    let connection = workflow.Connections.find((a) => a.Id === id);
    if (!connection) {
      return <div>Connection does not exist: {id} </div>;
    }

    this.connection = connection;

    const workflowAction = connection.workflow.Actions.find(
      (a) => a.Id === connection.ActionId
    );

    let fromOptions = emptyOptions;
    switch (connection.fromElementType) {
      case "State":
        fromOptions = statePositionOptions;
        break;
      case "TransitionJoin":
        fromOptions = joinFromOptions;
        break;
      case "TransitionSplit":
        fromOptions = splitFromOptions;
        break;
    }

    let toOptions = emptyOptions;
    switch (connection.toElementType) {
      case "State":
        toOptions = statePositionOptions;
        break;
      case "TransitionJoin":
        toOptions = joinToOptions;
        break;
      case "TransitionSplit":
        toOptions = splitToOptions;
        break;
    }

    let handler = this.context.createAccordionHandler(
      "Connection_" + this.props.id,
      [0]
    );

    return (
      <Form className={limited}>
        <Accordion>
          <Accordion.Title
            active={handler.isActive(0)}
            index={0}
            onClick={handler.handleClick}
          >
            <Header dividing as="h4">
              <Header.Content>
                <Icon name="dropdown" />
                <IconView entity={connection} />
                {connection.Name || connection.Id || "<Empty>"}
              </Header.Content>
              <SUILabel color="green" size="tiny" className={floatRight}>
                Id: {connection.Id}
              </SUILabel>
            </Header>
          </Accordion.Title>
          <Accordion.Content active={handler.isActive(0)}>
            <EntityEditor entity={connection} hideHeader={true} />
            <Form.Group>
              <Select
                owner={connection.fields.From}
                width={9}
                fluid
                label="From"
                search
                options={workflow.connectionOptions}
                onChange={this.changeSourcePosition}
              />
              <Select
                owner={connection.fields.SourcePort}
                fluid
                width={7}
                label="Port"
                search
                options={fromOptions}
                onChange={this.changeSourcePort}
              />
            </Form.Group>
            <Form.Group>
              <Select
                owner={connection.fields.To}
                fluid
                width={9}
                label="To"
                search
                options={workflow.connectionOptions}
                onChange={this.changeTargetPosition}
              />
              <Select
                owner={connection.fields.TargetPort}
                fluid
                width={7}
                label="Port"
                search
                options={toOptions}
                onChange={this.changeTargetPort}
              />
            </Form.Group>
            <Select
              owner={connection.fields.ActionId}
              label="Action"
              search
              options={workflow.actionOptions}
            />
            <Input
              owner={connection.fields.AllowLoops}
              type="number"
              label="Allowed Loops"
            />
          </Accordion.Content>

          <Accordion.Title
            active={handler.isActive(1)}
            index={1}
            onClick={handler.handleClick}
          >
            <Header as="h4" dividing>
              <Header.Content>
                <Icon name="dropdown" />
                <Icon name="unhide" />
                Visual Properties
              </Header.Content>
            </Header>
          </Accordion.Title>
          <Accordion.Content active={handler.isActive(1)}>
            <Label label="Display Type" />
            <Form.Group>
              <Radio
                owner={connection.fields.ActionDisplay}
                name="DisplayType"
                label="Icon Only"
                value={ActionDisplayType.IconOnly}
              />
              <Radio
                owner={connection.fields.ActionDisplay}
                name="DisplayType"
                label="Icon and Text"
                value={ActionDisplayType.IconAndText}
              />
              <Radio
                owner={connection.fields.ActionDisplay}
                name="DisplayType"
                label="Full"
                value={ActionDisplayType.Full}
              />
            </Form.Group>
            <Checkbox
              owner={connection.fields.RotateLabel}
              label="Rotate Label"
            />
          </Accordion.Content>

          <Accordion.Title
            active={handler.isActive(2)}
            index={2}
            onClick={handler.handleClick}
          >
            <Header as="h4" dividing>
              <Header.Content>
                <Icon name="dropdown" />
                <Icon name="legal" />
                Access Rules
              </Header.Content>
            </Header>
          </Accordion.Title>
          <Accordion.Content active={handler.isActive(2)}>
            <AccessEditor
              ei={ei}
              access={connection.Access}
              name={"state_entry_" + connection.Id}
              workflow={connection.workflow}
              action={workflowAction}
            />
          </Accordion.Content>

          <Accordion.Title
            active={handler.isActive(3)}
            index={3}
            onClick={handler.handleClick}
          >
            <Header as="h4" dividing>
              <Header.Content>
                <Icon name="dropdown" />
                <Icon name="legal" />
                Effects
              </Header.Content>
            </Header>
          </Accordion.Title>
          <Accordion.Content active={handler.isActive(3)}>
            <AccessEditor
              ei={ei}
              access={connection.Effects}
              name={"state_exit_" + connection.Id}
              hideActionCondition={true}
              hidePreconditions={true}
              workflow={connection.workflow}
              action={workflowAction}
            />
          </Accordion.Content>
        </Accordion>
      </Form>
    );
  }
}
