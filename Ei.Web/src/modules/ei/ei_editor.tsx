import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form, Select, TextArea } from 'semantic-ui-mobx';
import { Header, Message } from 'semantic-ui-react';
import { style } from 'typestyle';

import { EntityEditor } from '../core/entity_view';
import { CodeEditor } from '../core/monaco_editor';
import { PropertyView } from '../properties/property_view';

interface Props {
  context?: App.Context;
}

const pane = style({
  padding: '12px'
});

@inject('context')
@observer
export class EiEditor extends React.Component<Props> {
  static displayName = 'EiEditor';

  update = value => {
    this.props.context.store.ei.Expressions = value
  };

  value = () => this.props.context.store.ei.Expressions;

  render() {
    let ei = this.props.context.store.ei;

    if (!ei) {
      return <Message content="Deleted" />;
    }

    return (
      <Form className={pane}>
        <EntityEditor entity={ei} />

        <Select label="Main Workflow" options={ei.workflowOptions} owner={ei.fields.MainWorkflow} />

        <Header as="h5" content="Expressions" dividing icon="code" />

        <CodeEditor update={this.update} value={this.value} i={ei.Properties} />

        <PropertyView owner={ei} types={ei.types} />
      </Form>
    );
  }
}
