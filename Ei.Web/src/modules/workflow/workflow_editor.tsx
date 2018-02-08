import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form } from 'semantic-ui-mobx';

import { Message } from 'semantic-ui-react';
import { EntityEditor } from '../core/entity_view';
import { PropertyView } from '../properties/property_view';

interface Props {
  context?: App.Context;
  id: string;
}

@inject('context')
@observer
export class WorkflowEditor extends React.Component<Props> {
  static displayName = 'EntityView';

  render() {
    let ei = this.props.context.store.ei;
    let entity = ei.Workflows.find(o => o.Id === this.props.id);

    if (!entity) {
      return <Message content="Workflow Deleted" />
    }

    return (
      <Form>
        <EntityEditor entity={entity} />

        <PropertyView owner={entity} types={ei.types} /> 
      </Form>
    );
  }
}
