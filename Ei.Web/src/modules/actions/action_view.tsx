import * as React from "react";

import { observer } from "mobx-react";
import { Form, Select } from "semantic-ui-mobx";

import { Button, Header } from "semantic-ui-react";
import { style } from "typestyle";
import { EntityEditor } from "../core/entity_view";
import { FieldCollectionEditor } from "../core/field_collection_editor";
import { GroupsEditor } from "../core/group_editor";
import { Action, ActionMessage } from "../ei/action_model";
import { Ei } from "../ei/ei_model";
import { PropertyView } from "../properties/property_view";
import { AppContext, Context } from "../../config/context";

interface Props {
  workflowId: string;
  id: string;
}

const deleteButton = style({
  marginTop: "10px",
  textAlign: "center",
});

@observer
export class ActionView extends React.Component<Props, {}, AppContext> {
  static displayName = "ActionView";
  static contextType = Context;

  action: Action;

  removeAgent = (e: React.MouseEvent<HTMLDivElement>) => {
    const idx = parseInt(e.currentTarget.getAttribute("data-index"), 10);
    (this.action as ActionMessage).NotifyAgents.removeAt(idx);
  };

  renderAction(action: Action, ei: Ei) {
    switch (action.$type) {
      case "ActionJoinWorkflowDao":
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

  delete = async () => {
    const ui = this.context.Ui;
    if (
      await ui.confirmDialogAsync(
        "Do you want to delete this action? This may break some references!"
      )
    ) {
      const ei = this.context.ei;
      const workflow = ei.Workflows.find((w) => w.Id === this.props.workflowId);
      const action = workflow.Actions.find((a) => a.Id === this.props.id);
      workflow.Actions.remove(action);
      ui.history.step();
    }
  };

  render() {
    const ei = this.props.context.ei;
    const workflow = ei.Workflows.find((w) => w.Id === this.props.workflowId);
    if (!workflow) {
      return <div>Workflow does not exist: {this.props.workflowId} </div>;
    }
    const action = workflow.Actions.find((a) => a.Id === this.props.id);
    if (!action) {
      return <div>Action does not exist: {this.props.id} </div>;
    }
    this.action = action;

    return (
      <Form>
        <EntityEditor entity={action} />

        {this.renderAction(action, ei)}

        <div className={deleteButton}>
          <Header as="h5" style={{ color: "red" }} dividing />
          <Button
            style={{ margin: "auto" }}
            icon="trash"
            content="Delete"
            labelPosition="left"
            color="red"
            onClick={this.delete}
          />
        </div>
      </Form>
    );
  }
}
