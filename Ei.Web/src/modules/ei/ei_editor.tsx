import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form, Select } from 'semantic-ui-mobx';
import { Message } from 'semantic-ui-react';
import { style } from 'typestyle';

import { EntityEditor } from '../core/entity_view';
import { PropertyView } from '../properties/property_view';


interface Props {
  context?: App.Context;
}

const pane = style({
  padding: '12px'
})

@inject('context')
@observer
export class EiEditor extends React.Component<Props> {
  static displayName = 'EiEditor';

  render() {
    let ei = this.props.context.store.ei;

    if (!ei) {
      return <Message content="Deleted" />;
    }

    return (
      <Form className={pane}>
        <EntityEditor entity={ei} />

        <Select label="Main Workflow" options={ei.workflowOptions} owner={ei.fields.MainWorkflow} />

        <PropertyView owner={ei} types={ei.types} />
      </Form>
    );
  }
}
