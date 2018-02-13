import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Checkbox, Form, Input, Label, Radio, Select } from 'semantic-ui-mobx';
import { DropdownItemProps, Header } from 'semantic-ui-react';

import { action } from 'mobx';
import { Ui } from '../../helpers/client_helpers';
import { AccessEditor } from '../access/access_editor';
import { EntityEditor } from '../core/entity_view';
import { ActionDisplayType, Connection } from '../ei/connection_model';

interface Props {
  id: string;
  workflowId: string;
  context?: App.Context;
}

const emptyOptions: DropdownItemProps[] = [];

const statePositionOptions = [
  { text: 'North', value: 'north' },
  { text: 'NorthEast', value: 'northeast' },
  { text: 'East', value: 'east' },
  { text: 'South East', value: 'southeast' },
  { text: 'South', value: 'south' },
  { text: 'South West', value: 'southwest' },
  { text: 'West', value: 'west' },
  { text: 'North West', value: 'northwest' },
  { text: 'Disconnect', value: '' }
];

const joinToOptions = [
  { text: 'Join 1', value: 'join1' },
  { text: 'Join 2', value: 'join3' },
  { text: 'Join 3', value: 'join4' },
  { text: 'Disconnect', value: '' }
];

const joinFromOptions = [{ text: 'Yield', value: 'yield' }, { text: 'Disconnect', value: '' }];

const splitFromOptions = [
  { text: 'Split 1', value: 'split1' },
  { text: 'Split 2', value: 'split2' },
  { text: 'Split 3', value: 'split3' },
  { text: 'Disconnect', value: '' }
];

const splitToOptions = [{ text: 'Input', value: 'input' }, { text: 'Disconnect', value: '' }];

@inject('context')
@observer
export class ConnectionEditor extends React.Component<Props> {
  connection: Connection;

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
      this.connection.From = '';
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
      this.connection.To = '';
      this.connection.TargetPort = null;
    }
    this.connection.update();
    this.connection.workflow.Connections.remove(this.connection);
    this.connection.workflow.Connections.push(this.connection);

    Ui.history.step();
  });

  componentWillMount() {
    this.props.context.store.selectWorkflowElement(this.props.workflowId, 'Connections', this.props.id, 'link');
  }

  componentWillUpdate(props: Props) {
    props.context.store.selectWorkflowElement(props.workflowId, 'Connections', props.id, 'link');
  }

  render() {
    const { id, context } = this.props;
    let ei = context.store.ei;

    let workflow = ei.Workflows.find(w => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    let connection = workflow.Connections.find(a => a.Id === id);
    if (!connection) {
      return <div>Connection does not exist: {id} </div>;
    }

    this.connection = connection;

    let fromOptions = emptyOptions;
    switch (connection.fromElementType) {
      case 'State':
        fromOptions = statePositionOptions;
        break;
      case 'TransitionJoin':
        fromOptions = joinFromOptions;
        break;
      case 'TransitionSplit':
        fromOptions = splitFromOptions;
        break;
    }

    let toOptions = emptyOptions;
    switch (connection.toElementType) {
      case 'State':
        toOptions = statePositionOptions;
        break;
      case 'TransitionJoin':
        toOptions = joinToOptions;
        break;
      case 'TransitionSplit':
        toOptions = splitToOptions;
        break;
    }

    return (
      <Form>
        <EntityEditor entity={connection} />
        <Form.Group>
          <Select
            owner={connection.fields.From}
            width={9}
            fluid
            label="From"
            options={workflow.connectionOptions}
            onChange={this.changeSourcePosition}
          />
          <Select
            owner={connection.fields.SourcePort}
            fluid
            width={7}
            label="Port"
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
            options={workflow.connectionOptions}
            onChange={this.changeTargetPosition}
          />
          <Select
            owner={connection.fields.TargetPort}
            fluid
            width={7}
            label="Port"
            options={toOptions}
            onChange={this.changeTargetPort}
          />
        </Form.Group>
        <Select
          owner={connection.fields.ActionId}
          label="Action"
          options={workflow.actionOptions}
        />
        <Input owner={connection.fields.AllowLoops} type="number" label="Allowed Loops" />

        <Header as="h4" icon="unhide" content="Visual Properties" dividing />

        <Label label="Display Type" />
        <Form.Group>
          <Radio owner={connection.fields.ActionDisplay} name="DisplayType" label="Icon Only" value={ActionDisplayType.IconOnly} />
          <Radio owner={connection.fields.ActionDisplay} name="DisplayType" label="Icon and Text" value={ActionDisplayType.IconAndText} />
          <Radio owner={connection.fields.ActionDisplay} name="DisplayType" label="Full" value={ActionDisplayType.Full} />
        </Form.Group>
        <Checkbox owner={connection.fields.RotateLabel} label="Rotate Label" />

        <Header as="h4" icon="legal" content="Access Rules" dividing />
        <AccessEditor ei={ei} access={connection.Access} name={'state_entry_' + connection.Id} />

        <Header as="h4" icon="legal" content="Effects" />
        <AccessEditor ei={ei} access={connection.Effects} name={'state_exit_' + connection.Id} />
      </Form>
    );
  }
}
