import * as React from "react";

import { inject, observer } from "mobx-react";
import { Checkbox, Form, Input } from "semantic-ui-mobx";
import { style } from "typestyle";

import { Header } from "semantic-ui-react";
import { EntityEditor } from "../core/entity_view";
import { Transition, TransitionSplit } from "../ei/transition_model";

export const lightDisabled = style({
  opacity: ".9!important" as any,
});

interface Props {
  id: string;
  workflowId: string;
  context?: App.Context;
}

@inject("context")
@observer
export class TransitionEditor extends React.Component<Props> {
  componentWillMount() {
    this.props.context.selectWorkflowElement(
      this.props.workflowId,
      "Transitions",
      this.props.id
    );
  }

  componentWillUpdate(props: Props) {
    props.context.selectWorkflowElement(
      props.workflowId,
      "Transitions",
      props.id
    );
  }

  renderTransition(transition: Transition) {
    if (transition.$type === "TransitionSplitDao") {
      const tr = transition as TransitionSplit;
      return (
        <>
          <Header content="Splits" icon="fork" as="h4" dividing />
          {tr.Names.map((n, i) => (
            <Form.Group key={i}>
              <Form.Input
                width={8}
                className={lightDisabled}
                disabled
                value={n.stateId}
                label="State"
              />
              <Input width={8} owner={n.fields.name} label="Agent Name" />
            </Form.Group>
          ))}
          <Checkbox owner={tr.fields.Shallow} label="Shallow" />
        </>
      );
    }
    return false;
  }

  render() {
    const { id, context } = this.props;
    let ei = context.ei;

    let workflow = ei.Workflows.find((w) => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    let transition = workflow.Transitions.find((a) => a.Id === id);
    if (!transition) {
      return <div>Transition does not exist: {id} </div>;
    }

    return (
      <Form>
        <EntityEditor entity={transition} />

        {this.renderTransition(transition)}

        <Header as="h4" icon="unhide" content="Visual Properties" />
        <Checkbox owner={transition.fields.Horizontal} label="Horizontal" />
      </Form>
    );
  }
}
