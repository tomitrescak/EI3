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

import { action } from "mobx";
import { Ui } from "../../helpers/client_helpers";
import { AccessEditor } from "../access/access_editor";
import { IconView } from "../core/entity_icon_view";
import { EntityEditor } from "../core/entity_view";
import { ActionDisplayType, FreeJoint } from "../ei/connection_model";
import { useAppContext } from "../../config/context";
import styled from "@emotion/styled";
import { useParams } from "react-router";

const FloatedLabel = styled(SUILabel)`
  float: right;
`;

const LimitedForm = styled(Form)`
  .accordion.ui {
    margin: 0px !important;
  }
`;

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
  { text: "Join 2", value: "join2" },
  { text: "Join 3", value: "join3" },
  // { text: "Disconnect", value: "" },
];

const joinFromOptions = [
  { text: "Yield", value: "yield" },
  // { text: "Disconnect", value: "" },
];

const splitFromOptions = [
  { text: "Split 1", value: "split1" },
  { text: "Split 2", value: "split2" },
  { text: "Split 3", value: "split3" },
  // { text: "Disconnect", value: "" },
];

const splitToOptions = [
  { text: "Input", value: "input" },
  // { text: "Disconnect", value: "" },
];

export const ConnectionEditor = observer(() => {
  const context = useAppContext();

  const changeSourcePort = action((_e: any, { value }: any) => {
    const workflow = connection.workflow;
    const link = connection.link;

    connection.SourcePort = value;

    const fromPosition = workflow.findPosition(connection.From);
    link.getSourcePort().removeLink(link);
    link.setSourcePort(fromPosition.getPort(value));

    // connection.workflow.Connections.remove(connection);
    // connection.workflow.Connections.push(connection);

    context.engine.repaintCanvas();

    Ui.history.step();
  });

  const changeSourcePosition = action((_e: any, { value }: any) => {
    const workflow = connection.workflow;
    const link = connection.link;

    // remove old joints

    link.getSourcePort().removeLink(link);

    const fromPosition = workflow.findPosition(value);
    if (fromPosition) {
      const port =
        fromPosition.getPorts()[Object.keys(fromPosition.getPorts())[0]];
      connection.SourcePort = port.getName();
      link.setSourcePort(port);

      connection.From = value;
    } else {
      connection.From = "";
      connection.SourcePort = null;
    }

    const model = context.engine.getModel();
    connection.update(model);

    // connection.workflow.Connections.remove(connection);
    // connection.workflow.Connections.push(connection);

    context.engine.repaintCanvas();

    Ui.history.step();
  });

  const changeTargetPort = action((_e: any, { value }: any) => {
    const workflow = connection.workflow;
    const link = connection.link;

    connection.TargetPort = value;

    const toPosition = workflow.findPosition(connection.To);
    link.getTargetPort().removeLink(link);
    link.setTargetPort(toPosition.getPort(value));

    // connection.workflow.Connections.remove(connection);
    // connection.workflow.Connections.push(connection);

    context.engine.repaintCanvas();

    Ui.history.step();
  });

  const changeTargetPosition = action((_e: any, { value }: any) => {
    const workflow = connection.workflow;
    const link = connection.link;

    link.getTargetPort().removeLink(link);

    const parent = link.getTargetPort().getParent();

    // if it is a free widget remove it
    if (value != "") {
      let freeJoint = parent as FreeJoint;
      context.engine.getModel().removeNode(freeJoint);
    }

    const toPosition = workflow.findPosition(value);
    if (toPosition) {
      let port = toPosition.getPorts()[Object.keys(toPosition.getPorts())[0]];
      connection.TargetPort = port.getName();
      link.setTargetPort(port);

      connection.To = value;
    } else {
      connection.To = "";
      connection.TargetPort = null;
    }

    const model = context.engine.getModel();
    connection.update(model);

    context.engine.repaintCanvas();
    Ui.history.step();
  });

  // componentWillMount() {
  //   context.selectWorkflowElement(
  //     props.workflowId,
  //     "Connections",
  //     props.id,
  //     "link"
  //   );
  // }

  // componentWillUpdate(props: Props) {
  //   context.selectWorkflowElement(
  //     props.workflowId,
  //     "Connections",
  //     props.id,
  //     "link"
  //   );
  // }

  const { workflowId, id } = useParams<{ workflowId: string; id: string }>();
  let ei = context.ei;

  let workflow = ei.Workflows.find(
    (w) => w.Id.toLowerCase() === workflowId.toLowerCase()
  );
  if (!workflow) {
    return <div>Workflow does not exist: {workflowId} </div>;
  }
  let connection = workflow.Connections.find((a) => a.Id === id);
  if (!connection) {
    return <div>Connection does not exist: {id} </div>;
  }

  connection = connection;

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

  connection.From;
  connection.To;

  let handler = context.createAccordionHandler("Connection_" + id, [0]);

  return (
    <LimitedForm>
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
            <FloatedLabel color="green" size="tiny">
              Id: {connection.Id}
            </FloatedLabel>
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
              onChange={changeSourcePosition}
            />
            <Select
              owner={connection.fields.SourcePort}
              fluid
              width={7}
              label="Port"
              search
              options={fromOptions}
              onChange={changeSourcePort}
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
              onChange={changeTargetPosition}
            />
            <Select
              owner={connection.fields.TargetPort}
              fluid
              width={7}
              label="Port"
              search
              options={toOptions}
              onChange={changeTargetPort}
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
    </LimitedForm>
  );
});
