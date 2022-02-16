import React from "react";

import { observer } from "mobx-react";
import { Header } from "semantic-ui-react";

import { AccessEditor } from "../access/access_editor";
import { EntityEditor } from "../core/entity_view";
import { useAppContext } from "../../config/context";
import { useQuery } from "../../helpers/client_helpers";
import { Checkbox, Form, Formix, Input } from "../Form";

export const StateEditor = observer(() => {
  const context = useAppContext();
  const { id, w: workflowId } = useQuery<{ id: string; w: string }>();
  let ei = context.ei;

  let workflow = ei.Workflows.find(
    (w) => w.Id.toLowerCase() === workflowId.toLowerCase()
  );
  if (!workflow) {
    return <div>Workflow does not exist: {workflowId} </div>;
  }
  let state = workflow.States.find(
    (a) => a.Id.toLowerCase() === id.toLowerCase()
  );
  if (!state) {
    return <div>State does not exist: {id} </div>;
  }

  return (
    <Formix initialValues={state}>
      <>
        <EntityEditor entity={state} />
        <Input name={"Timeout"} label="Timeout" type="number" />
        <Checkbox name={"IsOpen"} label="Open" />
        <Checkbox name={"IsStart"} label="Start State" />
        <Checkbox name={"IsEnd"} label="End State" />

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
        <Checkbox name={"ShowRules"} label="Show Rules" />
      </>
    </Formix>
  );
});
