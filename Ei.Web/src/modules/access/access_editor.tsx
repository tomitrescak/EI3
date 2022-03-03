import * as React from "react";

import { Observer, observer } from "mobx-react";

import {
  Accordion,
  Button,
  FormButton,
  FormGroup,
  Icon,
  Popup,
  Segment,
} from "semantic-ui-react";

import { CodeEditor } from "../core/monaco_editor";
import { AccessCondition, Postcondition } from "../ei/access_model";
import { Action } from "../ei/action_model";
import { Ei, Organisation, Role } from "../ei/ei_model";
import { Workflow } from "../ei/workflow_model";
import { AccordionHandler } from "../../config/context";
import styled from "@emotion/styled";
import { makeObservable, observable, observe } from "mobx";
import { Checkbox, Formix, Select } from "../Form";

interface AccessProps {
  access: AccessCondition[];
  ei: Ei;
  name: string;
  action?: Action;
  workflow: Workflow;
  hidePreconditions?: boolean;
  hideActionCondition?: boolean;
}

const AddButton = styled(Button)`
  margin-top: 12px !important;
`;
const FieldRow = styled(FormGroup)`
  margin: 0px !important;
`;
const EditorHolder = styled(Segment)`
  padding: 0px !important;
`;
const HeaderHolder = styled(Segment)`
  padding: 3px 12px !important;
`;

export const AccessEditor = observer((props: AccessProps) => {
  const {
    access,
    name,
    ei,
    action,
    workflow,
    hideActionCondition,
    hidePreconditions,
  } = props;
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

      <AddButton
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
});

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

class AccessConditionState {
  showPrecondition: boolean = false;
  showPostcondition: boolean = false;

  constructor(show: boolean) {
    this.showPrecondition = show;

    makeObservable(this, {
      showPrecondition: observable,
      showPostcondition: observable,
    });
  }
}

export const AccessConditionEditor = observer((props: AccessConditionProps) => {
  const accessState: AccessConditionState = React.useMemo(() => {
    return new AccessConditionState(!!props.condition.Precondition);
  }, [props.condition]);

  const actionUpdate = (value: any) => {
    props.condition.Precondition = value;
  };

  const actionValue = () => props.condition.Precondition;

  // componentWillUpdate(nextProps: AccessConditionProps) {
  //   accessState.showPrecondition = !!nextProps.condition.Precondition;
  // }

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
  } = props;
  const organisation =
    ei.Organisations.find((o) => o.Id === condition.Organisation) ||
    ei.Organisations[0];
  const role = ei.Roles.find((o) => o.Id === condition.Role) || ei.Roles[0];
  return (
    <Formix initialValues={condition}>
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
              <FieldRow>
                <Select
                  fluid
                  name={"Organisation"}
                  label="Organisation"
                  selection
                  options={ei.organisationsOptions}
                />
                <Select
                  fluid
                  name={"Role"}
                  selection
                  label="Role"
                  options={ei.roleOptions}
                />
                <FormButton
                  label="&nbsp;"
                  type="button"
                  name="removeRule"
                  color="red"
                  onClick={remove}
                  icon="trash"
                />
              </FieldRow>
            </Segment>
            <Formix initialValues={accessState}>
              <>
                {!hidePreconditions && (
                  <Segment tertiary={true} attached as="h5" icon="legal">
                    <Icon name="legal" />
                    <Popup
                      trigger={<span>Precondition</span>}
                      content={<Info />}
                    />
                    <div style={{ float: "right" }}>
                      <Checkbox name={"showPrecondition"} label="Show" />
                    </div>
                  </Segment>
                )}
                <Observer>
                  {() =>
                    !hidePreconditions &&
                    accessState.showPrecondition && (
                      <EditorHolder secondary attached>
                        <CodeEditor
                          update={actionUpdate}
                          value={actionValue}
                          height={ei.editorHeight(actionValue())}
                          i={ei.Properties}
                          w={workflow && workflow.Properties}
                          o={organisation.Properties}
                          r={role.Properties}
                          a={action && action.Properties}
                        />
                      </EditorHolder>
                    )
                  }
                </Observer>
                {!hideActionCondition && (
                  <Segment tertiary attached as="h5" icon="legal">
                    <Icon name="legal" />
                    <Popup
                      trigger={<span>Postconditions</span>}
                      content={<Info />}
                    />
                    <div style={{ float: "right" }}>
                      <Checkbox name={"showPostcondition"} label="Show All" />
                    </div>
                  </Segment>
                )}
              </>
            </Formix>

            <Observer>
              {() => (
                <>
                  {condition.Postconditions.map((p, i) => (
                    <PostconditionView
                      key={i}
                      p={p}
                      i={i}
                      ei={ei}
                      workflow={props.workflow}
                      organisation={organisation}
                      role={role}
                      action={props.action}
                      remove={() => condition.Postconditions.splice(i, 1)}
                      showAll={accessState.showPostcondition}
                      hideActionCondition={hideActionCondition}
                    />
                  ))}
                </>
              )}
            </Observer>
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
    </Formix>
  );
});

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

export const PostconditionView = observer((props: PostconditionProps) => {
  const actionUpdate = (value: any) => {
    props.p.Action = value;
  };

  const actionValue = () => props.p.Action;

  const conditionUpdate = (value: any) => {
    props.p.Condition = value;
  };

  const conditionValue = () => props.p.Condition;

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
  } = props;

  return (
    <Observer>
      {() => (
        <>
          {!hideActionCondition &&
            (showAll || p.Condition || (!p.Condition && !p.Action)) && (
              <>
                <HeaderHolder secondary attached>
                  Condition
                </HeaderHolder>
                <EditorHolder secondary attached>
                  <CodeEditor
                    update={conditionUpdate}
                    value={conditionValue}
                    height={ei.editorHeight(conditionValue())}
                    i={ei.Properties}
                    w={workflow && workflow.Properties}
                    o={organisation.Properties}
                    r={role.Properties}
                    a={action && action.Properties}
                  />
                </EditorHolder>
              </>
            )}
          {(!hideActionCondition ||
            showAll ||
            p.Action ||
            (!p.Condition && !p.Action)) && (
            <>
              <HeaderHolder secondary attached>
                Action
              </HeaderHolder>
              <EditorHolder secondary attached>
                <CodeEditor
                  update={actionUpdate}
                  value={actionValue}
                  height={ei.editorHeight(actionValue())}
                  i={ei.Properties}
                  w={workflow && workflow.Properties}
                  o={organisation.Properties}
                  r={role.Properties}
                  a={action && action.Properties}
                />
              </EditorHolder>
            </>
          )}
          <HeaderHolder secondary attached>
            <Button
              type="button"
              content="Remove Postcondition"
              name="addInput"
              color="red"
              onClick={remove}
              icon="trash"
            />
          </HeaderHolder>
        </>
      )}
    </Observer>
  );
});
