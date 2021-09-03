import * as React from "react";

import { inject, observer } from "mobx-react";
import { Checkbox, Form } from "semantic-ui-mobx";
import { Button, Header, Message } from "semantic-ui-react";
import { style } from "typestyle";

import { AccessEditor } from "../access/access_editor";
import { EntityEditor } from "../core/entity_view";
import { PropertyView } from "../properties/property_view";

interface Props {
  context?: App.Context;
  id: string;
}

const deleteButton = style({
  marginTop: "10px",
  textAlign: "center",
});

@inject("context")
@observer
export class WorkflowEditor extends React.Component<Props> {
  static displayName = "EntityView";

  render() {
    let ei = this.props.context.ei;
    let workflow = ei.Workflows.find((o) => o.Id === this.props.id);

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
          <div className={deleteButton}>
            <Header as="h5" style={{ color: "red" }} dividing />
            <Button
              style={{ margin: "auto" }}
              icon="trash"
              content="Delete"
              labelPosition="left"
              color="red"
              type="button"
              onClick={workflow.delete}
            />
          </div>
        )}
      </Form>
    );
  }
}
