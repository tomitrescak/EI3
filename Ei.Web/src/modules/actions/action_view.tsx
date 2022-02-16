import React from "react";

import { Button, Header } from "semantic-ui-react";

import { EntityEditor } from "../core/entity_view";
import { FieldCollectionEditor } from "../core/field_collection_editor";
import { GroupsEditor } from "../core/group_editor";
import { Action, ActionMessage } from "../ei/action_model";
import { Ei } from "../ei/ei_model";
import { PropertyView } from "../properties/property_view";
import { useAppContext } from "../../config/context";
import { Ui, useQuery } from "../../helpers/client_helpers";
import { Form, Formix, Select } from "../Form";

function renderAction(action: Action, ei: Ei) {
  switch (action.$type) {
    case "ActionJoinWorkflowDao":
      return (
        <>
          <Select
            name="WorkflowId"
            label="Workflow"
            options={ei.workflowOptions}
          />
          <PropertyView owner={action} types={ei.types} />
        </>
      );
    case "ActionMessageDao":
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

async function deleteAction(ei: Ei, workflowId: string, actionId: string) {
  if (
    await Ui.confirmDialogAsync(
      "Do you want to delete this action? This may break some references!"
    )
  ) {
    const workflow = ei.Workflows.find((w) => w.Id === workflowId);
    const action = workflow.Actions.find((a) => a.Id === actionId);
    workflow.Actions.remove(action);
    Ui.history.step();
  }
}

export const ActionView = () => {
  const context = useAppContext();
  const { w, id } = useQuery<{ w: string; id: string }>();

  // const removeAgent = (e: React.MouseEvent<HTMLDivElement>) => {
  //   const idx = parseInt(e.currentTarget.getAttribute("data-index"), 10);
  //   (action as ActionMessage).NotifyAgents.removeAt(idx);
  // };

  const ei = context.ei;
  const workflow = ei.Workflows.find(
    (wf) => wf.Id.toLowerCase() === w.toLowerCase()
  );
  if (!workflow) {
    return <div>Workflow does not exist: {w} </div>;
  }
  const action = workflow.Actions.find(
    (a) => a.Id.toLowerCase() === id.toLowerCase()
  );
  if (!action) {
    return <div>Action does not exist: {id} </div>;
  }

  return (
    <Formix initialValues={action}>
      <>
        <EntityEditor entity={action} />

        {renderAction(action, ei)}

        <Button
          style={{ margin: "auto", marginTop: 8 }}
          icon="trash"
          content="Delete"
          labelPosition="left"
          color="red"
          onClick={() => deleteAction(ei, w, id)}
        />
      </>
    </Formix>
  );
};
