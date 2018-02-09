import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form } from 'semantic-ui-mobx';
import { Button, Header, Message } from 'semantic-ui-react';
import { style } from 'typestyle';

import { EntityEditor } from '../core/entity_view';
import { PropertyView } from '../properties/property_view';


interface Props {
  context?: App.Context;
  id: string;
}

const deleteButton = style({
  marginTop: '10px',
  textAlign: 'center'
});

@inject('context')
@observer
export class WorkflowEditor extends React.Component<Props> {
  static displayName = 'EntityView';

  render() {
    let ei = this.props.context.store.ei;
    let workflow = ei.Workflows.find(o => o.Id === this.props.id);

    if (!workflow) {
      return <Message content="Workflow Deleted" />
    }

    return (
      <Form>
        <EntityEditor entity={workflow} />

        <PropertyView owner={workflow} types={ei.types} /> 

        { ei.MainWorkflow !== workflow.Id && (
          <div className={deleteButton}>
            <Header as="h5" style={{ color: 'red' }} dividing />
            <Button
              style={{ margin: 'auto' }}
              icon="trash"
              content="Delete"
              labelPosition="left"
              color="red"
              onClick={workflow.delete}
            />
          </div>
        )}
      </Form>
    );
  }
}
