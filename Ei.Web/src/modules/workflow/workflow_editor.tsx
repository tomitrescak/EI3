import React from "react";

import { observer } from "mobx-react";
import { Button, Header, Message } from "semantic-ui-react";

import { AccessEditor } from "../access/access_editor";
import { EntityEditor } from "../core/entity_view";
import { PropertyView } from "../properties/property_view";
import { useAppContext } from "../../config/context";
import { useQuery } from "../../helpers/client_helpers";
import { Checkbox, Form, Formix } from "../Form";

export const WorkflowEditor = observer(() => {
  const { w } = useQuery<{ w: string }>();
  const context = useAppContext();
  let ei = context.ei;
  let workflow = ei.Workflows.find((o) => o.Id === w);

  if (!workflow) {
    return <Message content="Workflow Deleted" />;
  }

  return (
    <Formix initialValues={workflow}>
      <>
        <EntityEditor entity={workflow} />

        <Checkbox name={"Static"} label="Static" />
        <Checkbox name={"Stateless"} label="Stateless" />

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
      </>
    </Formix>
  );
});
