import * as React from "react";

import { observer } from "mobx-react";
import { Checkbox, Form, Input } from "semantic-ui-mobx";

import { Header } from "semantic-ui-react";
import { EntityEditor } from "../core/entity_view";
import { Transition, TransitionSplit } from "../ei/transition_model";
import { useParams } from "react-router";
import { useAppContext } from "../../config/context";
import styled from "@emotion/styled";

export const StateInput = styled(Form.Input)`
  opacity: 0.9 !important;
`;

export const TransitionEditor = observer(() => {
  // componentWillUpdate(props: Props) {
  //   props.context.selectWorkflowElement(
  //     props.workflowId,
  //     "Transitions",
  //     props.id
  //   );
  // }

  function renderTransition(transition: Transition) {
    if (transition.$type === "TransitionSplitDao") {
      const tr = transition as TransitionSplit;
      return (
        <>
          <Header content="Splits" icon="fork" as="h4" dividing />
          {tr.Names.map((n, i) => (
            <Form.Group key={i}>
              <StateInput width={8} disabled value={n.stateId} label="State" />
              <Input width={8} owner={n.fields.name} label="Agent Name" />
            </Form.Group>
          ))}
          <Checkbox owner={tr.fields.Shallow} label="Shallow" />
        </>
      );
    }
    return false;
  }

  const { id, workflowId } = useParams<{ id: string; workflowId: string }>();
  let ei = useAppContext().ei;

  let workflow = ei.Workflows.find((w) => w.Id === workflowId);
  if (!workflow) {
    return <div>Workflow does not exist: {workflowId} </div>;
  }
  let transition = workflow.Transitions.find((a) => a.Id === id);
  if (!transition) {
    return <div>Transition does not exist: {id} </div>;
  }

  return (
    <Form>
      <EntityEditor entity={transition} />

      {renderTransition(transition)}

      <Header as="h4" icon="unhide" content="Visual Properties" />
      <Checkbox owner={transition.fields.Horizontal} label="Horizontal" />
    </Form>
  );
});
