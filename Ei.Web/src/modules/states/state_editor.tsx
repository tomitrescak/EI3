import * as React from "react";

import { inject, observer } from "mobx-react";
import { Checkbox, Form, Input } from "semantic-ui-mobx";
import { Header } from "semantic-ui-react";

import { AccessEditor } from "../access/access_editor";
import { EntityEditor } from "../core/entity_view";

interface Props {
  id: string;
  workflowId: string;
  context?: App.Context;
}

@inject("context")
@observer
export class StateEditor extends React.Component<Props> {
  componentWillMount() {
    this.props.context.selectWorkflowElement(
      this.props.workflowId,
      "States",
      this.props.id
    );
  }

  componentWillUpdate(props: Props) {
    props.context.selectWorkflowElement(props.workflowId, "States", props.id);
  }

  render() {
    const { id, context } = this.props;
    let ei = context.ei;

    let workflow = ei.Workflows.find((w) => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    let state = workflow.States.find((a) => a.Id === id);
    if (!state) {
      return <div>State does not exist: {id} </div>;
    }

    return (
      <Form>
        <EntityEditor entity={state} />
        <Input owner={state.fields.Timeout} label="Timeout" type="number" />
        <Checkbox owner={state.fields.IsOpen} label="Open" />
        <Checkbox owner={state.fields.IsStart} label="Start State" />
        <Checkbox owner={state.fields.IsEnd} label="End State" />

        <Header as="h4" icon="legal" content="Entry Rules" dividing />
        <AccessEditor
          ei={ei}
          access={state.EntryRules}
          name={"state_entry_" + state.Id}
          workflow={workflow}
        />

        <Header as="h4" icon="legal" content="Exit Rules" />
        <AccessEditor
          ei={ei}
          access={state.ExitRules}
          name={"state_exit_" + state.Id}
          workflow={workflow}
        />

        <Header as="h4" icon="unhide" content="Visual Properties" />
        <Checkbox owner={state.fields.ShowRules} label="Show Rules" />
      </Form>
    );
  }
}
