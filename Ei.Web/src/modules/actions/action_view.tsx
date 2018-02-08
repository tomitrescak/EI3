import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form, Select } from 'semantic-ui-mobx';

import { Header } from 'semantic-ui-react';
import { EntityEditor } from '../core/entity_view';
import { FieldCollectionEditor } from '../core/field_collection_editor';
import { GroupsEditor } from '../core/group_editor';
import { Action, ActionMessage } from '../ei/action_model';
import { Ei } from '../ei/ei_model';
import { PropertyView } from '../properties/property_view';

interface Props {
  context?: App.Context;
  workflowId: string;
  id: string;
}

@inject('context')
@observer
export class ActionView extends React.Component<Props> {
  static displayName = 'ActionView';
  action: Action;

  removeAgent = (e: React.MouseEvent<HTMLDivElement>) => {
    const idx = parseInt(e.currentTarget.getAttribute('data-index'), 10);
    (this.action as ActionMessage).NotifyAgents.removeAt(idx);
  };

  renderAction(action: Action, ei: Ei) {
    switch (action.$type) {
      case 'ActionJoinWorkflowDao':
        return (
          <>
            <Select
              owner={action.fields.WorkflowId}
              label="Workflow"
              options={ei.workflowOptions}
            />
            <PropertyView owner={action} types={ei.types} />
          </>
        );
      case 'ActionMessageDao':
        let am = action as ActionMessage;
        return (
          <>
            <Header as="h4" icon="user" content="Notify Agent" dividing />
            <FieldCollectionEditor collection={am.NotifyAgents} />
            <Header as="h4" icon="users" content="Notify Roles" dividing />
            <GroupsEditor groups={am.NotifyGroups} ei={ei} />
            <PropertyView owner={action} types={ei.types} />
          </>
        );
      default:
        return false;
    }
  }

  render() {
    let ei = this.props.context.store.ei;
    let workflow = ei.Workflows.find(w => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    let action = workflow.Actions.find(a => a.Id === this.props.id);
    if (!action) {
      return <div>Action does not exist: {this.props.id} </div>;
    }
    this.action = action;

    return (
      <Form>
        <EntityEditor entity={action} />

        {this.renderAction(action, ei)}
      </Form>
    );
  }
}
