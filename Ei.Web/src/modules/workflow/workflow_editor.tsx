import React from "react";

import { observer } from "mobx-react";
import { Checkbox, Form } from "semantic-ui-mobx";
import { Button, Header, Message } from "semantic-ui-react";

import { AccessEditor } from "../access/access_editor";
import { EntityEditor } from "../core/entity_view";
import { PropertyView } from "../properties/property_view";
import { useAppContext } from "../../config/context";
import { useParams } from "react-router-dom";

export const WorkflowEditor = observer(() => {
  const { workflowId } = useParams<{ workflowId: string }>();
  const context = useAppContext();
  let ei = context.ei;
  let workflow = ei.Workflows.find((o) => o.Id === workflowId);

  if (!workflow) {
    return <Message content="Workflow Deleted" />;
  }

  return (
    <Form>
      <EntityEditor entity={workflow} />

      <Checkbox owner={workflow.fields.Static} label="Static" />
      <Checkbox owner={workflow.fields.Stateless} label="Stateless" />

      <PropertyView owner={workflow} types={ei.types} />

      <Header as="h4" icon="legal" content="Allow Create" dividing />
      <AccessEditor
        ei={ei}
        access={workflow.AllowCreate}
        name={"allow_create_" + workflow.Id}
        workflow={workflow}
      />

      <Header as="h4" icon="legal" content="Allow Join" />
      <AccessEditor
        ei={ei}
        access={workflow.AllowJoin}
        name={"allow_join_" + workflow.Id}
        workflow={workflow}
      />

      {ei.MainWorkflow !== workflow.Id && (
        <Button
          style={{ margin: "auto", marginTop: 8 }}
          icon="trash"
          content="Delete"
          labelPosition="left"
          color="red"
          type="button"
          onClick={workflow.delete}
        />
      )}
    </Form>
  );
});
