import * as React from "react";

import { observer } from "mobx-react";
import styled from "@emotion/styled";

import { FormGroup, Header } from "semantic-ui-react";
import { EntityEditor } from "../core/entity_view";
import { Transition, TransitionSplit } from "../ei/transition_model";
import { useAppContext } from "../../config/context";
import { useQuery } from "../../helpers/client_helpers";
import { Checkbox, Form, Formix, Input } from "../Form";

export const StateInput = styled(Input)`
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
            <Formix key={i} initialValues={n}>
              <FormGroup>
                <StateInput width={8} disabled name={"stateId"} label="State" />
                <Input width={8} name={"name"} label="Agent Name" />
              </FormGroup>
            </Formix>
          ))}
          <Checkbox name="Shallow" label="Shallow" />
        </>
      );
    }
    return false;
  }

  const { id, w: workflowId } = useQuery<{ id: string; w: string }>();
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
    <Formix initialValues={transition}>
      <>
        <EntityEditor entity={transition} />

        {renderTransition(transition)}

        <Header as="h4" icon="unhide" content="Visual Properties" />
        <Checkbox name="Horizontal" label="Horizontal" />
      </>
    </Formix>
  );
});
