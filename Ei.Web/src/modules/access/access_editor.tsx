import * as React from 'react';

import { observer } from 'mobx-react';

import {
  Checkbox,
  field,
  Form,
  FormState,
  Select,
  TextArea
} from 'semantic-ui-mobx';
import { Accordion, Button, Icon, Segment } from 'semantic-ui-react';
import { style } from 'typestyle';

import { AccordionHandler } from '../../config/store';
import { AccessCondition, Postcondition } from '../ei/access_model';
import { Ei } from '../ei/ei_model';

interface AccessProps {
  access: AccessCondition[];
  ei: Ei;
  name: string;
}

const addButton = style({ marginTop: '12px!important' });
const fieldRow = style({ margin: '0px!important' });

@observer
export class AccessEditor extends React.Component<AccessProps> {
  render() {
    const { access, name, ei } = this.props;
    const handler = ei.store.createAccordionHandler(name);
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
              new AccessCondition({ Organisation: ei.Organisations[0].Id, Role: ei.Roles[0].Id })
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
  remove: any;
  handler: AccordionHandler;
  index: number;
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

  remove() { /**/ }

  componentWillMount() {
    this.accessState = new AccessConditionState(!!this.props.condition.Precondition);
  }

  // componentWillUpdate(nextProps: AccessConditionProps) {
  //   this.accessState.showPrecondition = !!nextProps.condition.Precondition;
  // }

  render() {
    const { condition, ei, remove, handler, index } = this.props;
    const organisation = ei.Organisations.find(o => o.Id === condition.Organisation) || ei.Organisations[0];
    const role = ei.Roles.find(o => o.Id === condition.Role) || ei.Roles[0];
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
                <Select fluid owner={condition.fields.Role} label="Role" options={ei.roleOptions} />
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
            <Segment tertiary={true} attached as="h5" icon="legal">
              <Icon name="legal" />
              Precondition
              <div style={{ float: 'right' }}>
                <Checkbox owner={this.accessState.fields.showPrecondition} label="Show" />
              </div>
            </Segment>
            {this.accessState.showPrecondition && (
              <Segment attached>
                <TextArea owner={condition.fields.Precondition} />
              </Segment>
            )}
            <Segment tertiary attached as="h5" icon="legal">
              <Icon name="legal" />
              Postconditions
              <div style={{ float: 'right' }}>
                <Checkbox owner={this.accessState.fields.showPostcondition} label="Show All" />
              </div>
            </Segment>

            {condition.Postconditions.map((p, i) => (
              <PostconditionView
                key={i}
                p={p}
                i={i}
                remove={() => condition.Postconditions.splice(i, 1)}
                showAll={this.accessState.showPostcondition}
              />
            ))}
            <Segment attached="bottom" tertiary>
              <Button
                type="button"
                primary
                onClick={() =>
                  condition.Postconditions.push(new Postcondition({ Action: '', Condition: '' }))
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

interface PostconditionProps {
  p: Postcondition;
  i: number;
  remove: any;
  showAll: boolean;
}

export const PostconditionView = observer(({ p, i, remove, showAll }: PostconditionProps) => (
  <Segment secondary attached key={i}>
    {(showAll || p.Condition || (!p.Condition && !p.Action)) && (
      <TextArea
        owner={p.fields.Condition}
        label="Condition"
        rows={p.Condition ? p.Condition.split('\n').length : 1}
      />
    )}
    {(showAll || p.Action || (!p.Condition && !p.Action)) && (
      <TextArea
        owner={p.fields.Action}
        label="Action"
        rows={p.Action ? p.Action.split('\n').length : 1}
      />
    )}
    <Button
      type="button"
      content="Remove Postcondition"
      name="addInput"
      color="red"
      onClick={remove}
      icon="trash"
    />
  </Segment>
));
