import * as React from "react";

import { observer } from "mobx-react";

import { Checkbox, field, Form, FormState, Select } from "semantic-ui-mobx";
import { Accordion, Button, Icon, Popup, Segment } from "semantic-ui-react";
import { style } from "typestyle";

import { AccordionHandler } from "../../config/store";
import { CodeEditor } from "../core/monaco_editor";
import { AccessCondition, Postcondition } from "../ei/access_model";
import { Action } from "../ei/action_model";
import { Ei, Organisation, Role } from "../ei/ei_model";
import { Workflow } from "../ei/workflow_model";

interface AccessProps {
  access: AccessCondition[];
  ei: Ei;
  name: string;
  action?: Action;
  workflow: Workflow;
  hidePreconditions?: boolean;
  hideActionCondition?: boolean;
}

const addButton = style({ marginTop: "12px!important" });
const fieldRow = style({ margin: "0px!important" });
const editorHolder = style({ padding: "0px!important" });
const headerHolder = style({ padding: "3px 12px!important" });

@observer
export class AccessEditor extends React.Component<AccessProps> {
  render() {
    const {
      access,
      name,
      ei,
      action,
      workflow,
      hideActionCondition,
      hidePreconditions,
    } = this.props;
    const handler = ei.context.createAccordionHandler(name);
    return (
      <>
        <Accordion>
          {access.map((g, i) => (
            <AccessConditionEditor
              condition={g}
              ei={ei}
              key={i}
              remove={() => access.splice(i, 1)}
              handler={handler}
              index={i}
              action={action}
              workflow={workflow}
              hideActionCondition={hideActionCondition}
              hidePreconditions={hidePreconditions}
            />
          ))}
        </Accordion>

        <Button
          className={addButton}
          type="button"
          name="addInput"
          primary
          onClick={() =>
            access.push(
              new AccessCondition({
                Organisation: ei.Organisations[0].Id,
                Role: ei.Roles[0].Id,
              })
            )
          }
          icon="plus"
          content={`Add Rule`}
        />
      </>
    );
  }
}

interface AccessConditionProps {
  condition: AccessCondition;
  ei: Ei;
  workflow: Workflow;
  action: Action;
  remove: any;
  handler: AccordionHandler;
  index: number;
  hidePreconditions: boolean;
  hideActionCondition: boolean;
}

class AccessConditionState extends FormState {
  @field showPrecondition: boolean = false;
  @field showPostcondition: boolean = false;

  constructor(show: boolean) {
    super();
    this.showPrecondition = show;
  }
}

@observer
export class AccessConditionEditor extends React.Component<AccessConditionProps> {
  accessState: AccessConditionState;

  remove() {
    /**/
  }

  componentWillMount() {
    this.accessState = new AccessConditionState(
      !!this.props.condition.Precondition
    );
  }

  actionUpdate = (value: any) => {
    this.props.condition.Precondition = value;
  };

  actionValue = () => this.props.condition.Precondition;

  // componentWillUpdate(nextProps: AccessConditionProps) {
  //   this.accessState.showPrecondition = !!nextProps.condition.Precondition;
  // }

  render() {
    const {
      condition,
      ei,
      remove,
      handler,
      index,
      workflow,
      action,
      hideActionCondition,
      hidePreconditions,
    } = this.props;
    const organisation =
      ei.Organisations.find((o) => o.Id === condition.Organisation) ||
      ei.Organisations[0];
    const role = ei.Roles.find((o) => o.Id === condition.Role) || ei.Roles[0];
    return (
      <>
        <Accordion.Title
          active={handler.isActive(index)}
          index={index}
          onClick={handler.handleClick}
        >
          <Icon name="dropdown" />
          <span>
            {organisation.Name} &gt;
            {role.Name}
          </span>
        </Accordion.Title>
        <Accordion.Content active={handler.isActive(index)}>
          <>
            <Segment attached="top">
              <Form.Group className={fieldRow}>
                <Select
                  fluid
                  owner={condition.fields.Organisation}
                  label="Organisation"
                  options={ei.organisationsOptions}
                />
                <Select
                  fluid
                  owner={condition.fields.Role}
                  label="Role"
                  options={ei.roleOptions}
                />
                <Form.Button
                  label="&nbsp;"
                  type="button"
                  name="removeRule"
                  color="red"
                  onClick={remove}
                  icon="trash"
                  content={`Rule`}
                />
              </Form.Group>
            </Segment>
            {!hidePreconditions && (
              <Segment tertiary={true} attached as="h5" icon="legal">
                <Icon name="legal" />
                <Popup trigger={<span>Precondition</span>} content={<Info />} />
                <div style={{ float: "right" }}>
                  <Checkbox
                    owner={this.accessState.fields.showPrecondition}
                    label="Show"
                  />
                </div>
              </Segment>
            )}
            {!hidePreconditions && this.accessState.showPrecondition && (
              <Segment secondary attached className={editorHolder}>
                <CodeEditor
                  update={this.actionUpdate}
                  value={this.actionValue}
                  height={ei.editorHeight(this.actionValue())}
                  i={ei.Properties}
                  w={workflow && workflow.Properties}
                  o={organisation.Properties}
                  r={role.Properties}
                  a={action && action.Properties}
                />
              </Segment>
            )}
            {!hideActionCondition && (
              <Segment tertiary attached as="h5" icon="legal">
                <Icon name="legal" />
                <Popup
                  trigger={<span>Postconditions</span>}
                  content={<Info />}
                />
                <div style={{ float: "right" }}>
                  <Checkbox
                    owner={this.accessState.fields.showPostcondition}
                    label="Show All"
                  />
                </div>
              </Segment>
            )}

            {condition.Postconditions.map((p, i) => (
              <PostconditionView
                key={i}
                p={p}
                i={i}
                ei={ei}
                workflow={this.props.workflow}
                organisation={organisation}
                role={role}
                action={this.props.action}
                remove={() => condition.Postconditions.splice(i, 1)}
                showAll={this.accessState.showPostcondition}
                hideActionCondition={hideActionCondition}
              />
            ))}
            <Segment attached="bottom" tertiary>
              <Button
                type="button"
                primary
                onClick={() =>
                  condition.Postconditions.push(
                    new Postcondition({ Action: "", Condition: "" })
                  )
                }
                icon="plus"
                content={`Add Postcondition`}
              />
            </Segment>
          </>
        </Accordion.Content>
      </>
    );
  }
}

const Info = () => (
  <div>
    <h5 style={{ borderBottom: "solid 1px #cdcdcd" }}>Parameters</h5>
    <ul style={{ margin: "0px", paddingLeft: "20px" }}>
      <li>i: Institution</li>
      <li>w: Workflow</li>
      <li>g: Governor</li>
      <li>o: Organisation</li>
      <li>r: Role</li>
      <li>a: Action</li>
    </ul>
  </div>
);

interface PostconditionProps {
  ei: Ei;
  workflow: Workflow;
  organisation: Organisation;
  role: Role;
  action: Action;
  p: Postcondition;
  i: number;
  remove: any;
  showAll: boolean;
  hideActionCondition: boolean;
}

@observer
export class PostconditionView extends React.Component<PostconditionProps> {
  actionUpdate = (value: any) => {
    this.props.p.Action = value;
  };

  actionValue = () => this.props.p.Action;

  conditionUpdate = (value: any) => {
    this.props.p.Condition = value;
  };

  conditionValue = () => this.props.p.Condition;

  render() {
    const {
      p,
      remove,
      showAll,
      ei,
      workflow,
      role,
      organisation,
      action,
      hideActionCondition,
    } = this.props;

    return (
      <>
        {!hideActionCondition &&
          (showAll || p.Condition || (!p.Condition && !p.Action)) && (
            <>
              <Segment secondary attached className={headerHolder}>
                Condition
              </Segment>
              <Segment secondary attached className={editorHolder}>
                <CodeEditor
                  update={this.conditionUpdate}
                  value={this.conditionValue}
                  height={ei.editorHeight(this.conditionValue())}
                  i={ei.Properties}
                  w={workflow && workflow.Properties}
                  o={organisation.Properties}
                  r={role.Properties}
                  a={action && action.Properties}
                />
              </Segment>
            </>
          )}
        {(!hideActionCondition ||
          showAll ||
          p.Action ||
          (!p.Condition && !p.Action)) && (
          <>
            <Segment secondary attached className={headerHolder}>
              Action
            </Segment>
            <Segment secondary attached className={editorHolder}>
              <CodeEditor
                update={this.actionUpdate}
                value={this.actionValue}
                height={ei.editorHeight(this.actionValue())}
                i={ei.Properties}
                w={workflow && workflow.Properties}
                o={organisation.Properties}
                r={role.Properties}
                a={action && action.Properties}
              />
            </Segment>
          </>
        )}
        <Segment secondary attached className={headerHolder}>
          <Button
            type="button"
            content="Remove Postcondition"
            name="addInput"
            color="red"
            onClick={remove}
            icon="trash"
          />
        </Segment>
      </>
    );
  }
}
