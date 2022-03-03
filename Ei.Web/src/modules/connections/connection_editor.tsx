import React from "react";

import { observer } from "mobx-react";
import {
  Accordion,
  DropdownItemProps,
  Header,
  Icon,
  Label as SUILabel,
  Form as SUIForm,
  Label,
} from "semantic-ui-react";

import { action } from "mobx";
import { Ui, useQuery } from "../../helpers/client_helpers";
import { AccessEditor } from "../access/access_editor";
import { IconView } from "../core/entity_icon_view";
import { EntityEditor } from "../core/entity_view";
import { ActionDisplayType } from "../ei/connection_model";
import { useAppContext } from "../../config/context";
import styled from "@emotion/styled";
import { State } from "../ei/state_model";
import { Checkbox, Form, Formix, Input, Radio, Select } from "../Form";

const FloatedLabel = styled(SUILabel)`
  float: right;
`;

const LimitedForm = styled.div`
  .accordion.ui {
    margin: 0px !important;
  }
`;

const emptyOptions: DropdownItemProps[] = [];

const statePositionOptions = [
  { text: "North", value: "north" },
  { text: "NorthEast", value: "northEast" },
  { text: "East", value: "east" },
  { text: "South East", value: "southEast" },
  { text: "South", value: "south" },
  { text: "South West", value: "southWest" },
  { text: "West", value: "west" },
  { text: "North West", value: "northWest" },
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

const actionOrientations = [
  { text: "Top/Bottom", value: "TopBottom" },
  { text: "Bottom/Top", value: "Bottom" },
  { text: "Left/Right", value: "LeftRight" },
  // { text: "Disconnect", value: "" },
];

export const ConnectionEditor = observer(() => {
  const context = useAppContext();

  const changeSourcePort = action((_e: any, { value }: any) => {
    connection.SourcePort = value;
    Ui.history.step();
  });

  const changeSourcePosition = action((_e: any, { value }: any) => {
    const fromPosition = workflow.findPosition(value);
    if (fromPosition) {
      const port = Object.keys(fromPosition.ports)[0];

      connection.SourcePort = port;
      connection.From = value;
    } else {
      connection.From = "";
      connection.SourcePort = null;
    }
    Ui.history.step();
  });

  const changeTargetPort = action((_e: any, { value }: any) => {
    connection.TargetPort = value;
    Ui.history.step();
  });

  const changeTargetPosition = action((_e: any, { value }: any) => {
    const toPosition = workflow.findPosition(value);
    if (toPosition) {
      const port = Object.keys(toPosition.ports)[0];

      connection.TargetPort = port;
      connection.To = value;
    } else {
      connection.To = "";
      connection.TargetPort = null;
    }

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

  const { w: workflowId, id } = useQuery<{ w: string; id: string }>();
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
    <Formix initialValues={connection}>
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
            <SUIForm.Group>
              <Select
                name={"From"}
                width={9}
                fluid
                label="From"
                search
                selection
                options={workflow.connectionOptions}
                onChange={changeSourcePosition}
              />
              <Select
                name={"SourcePort"}
                fluid
                selection
                width={7}
                label="Port"
                search
                options={fromOptions}
                onChange={changeSourcePort}
              />
            </SUIForm.Group>
            <SUIForm.Group>
              <Select
                name={"To"}
                fluid
                selection
                width={9}
                label="To"
                search
                options={workflow.connectionOptions}
                onChange={changeTargetPosition}
              />
              <Select
                name={"TargetPort"}
                fluid
                selection
                width={7}
                label="Port"
                search
                options={toOptions}
                onChange={changeTargetPort}
              />
            </SUIForm.Group>
            <Select
              name={"ActionId"}
              label="Action"
              selection
              search
              options={workflow.actionOptions}
            />

            <Input name={"AllowLoops"} type="number" label="Allowed Loops" />
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
            <Select
              name={"ActionConnection"}
              label="Action Connections"
              selection
              options={actionOrientations}
            />

            <SUIForm.Group>
              {/* <Radio
                name={"ActionDisplay"}
                label="Icon Only"
                value={ActionDisplayType.IconOnly}
              /> */}
              <Radio
                name={"ActionDisplay"}
                label="Icon and Text"
                value={ActionDisplayType.IconAndText}
              />
              <Radio
                name={"ActionDisplay"}
                label="Full"
                value={ActionDisplayType.Full}
              />
            </SUIForm.Group>
            <Checkbox name={"RotateLabel"} label="Rotate Label" />
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
    </Formix>
  );
});
