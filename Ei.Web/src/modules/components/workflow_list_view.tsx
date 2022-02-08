import React from "react";

import { observer } from "mobx-react";
import { Accordion, Icon, Label } from "semantic-ui-react";

import { Ei } from "../ei/ei_model";
import { Workflow } from "../ei/workflow_model";
import { AccordionButton } from "./hierarchic_entity_view";
import { WorkflowComponentList } from "./workflow_component_view";
import { Link, useHistory } from "react-router-dom";
import styled from "@emotion/styled";
import {
  AccordionContent,
  AccordionTitle,
  SecondaryAccordionTitle,
} from "./accordion";

interface Props {
  active: boolean;
  index: number;
  ei: Ei;
  handleClick: any;
}

export const NestedAccordion = styled(Accordion)`
  margin: 0px !important;
`;

@observer
export class WorkflowList extends React.Component<Props> {
  render() {
    const { active, index, ei, handleClick } = this.props;
    const handler = ei.context.createAccordionHandler("workflows");

    return (
      <>
        <AccordionTitle active={active} index={index} onClick={handleClick}>
          <Label
            size="tiny"
            color="blue"
            circular
            content={ei.Workflows.length}
          />{" "}
          Workflows
          <AccordionButton
            floated="right"
            icon="plus"
            compact
            color="green"
            onClick={ei.createWorkflow}
          />
        </AccordionTitle>
        <Accordion.Content active={active} style={{ padding: 0 }}>
          <NestedAccordion>
            {ei.Workflows.map((workflowItem) => (
              <WorkflowDetail
                key={workflowItem.Id}
                index={workflowItem.Id.hashCode()}
                handleClick={handler.handleClick}
                workflow={workflowItem}
                active={handler.isActive(workflowItem.Id.hashCode())}
                ei={ei}
              />
            ))}
          </NestedAccordion>
        </Accordion.Content>
      </>
    );
  }
}

interface DetailProps {
  ei: Ei;
  workflow: Workflow;
  active: boolean;
  index: number;
  handleClick: any;
}

export const WorkflowDetail = ({
  ei,
  workflow,
  active,
  index,
  handleClick,
}: DetailProps) => {
  const handler = ei.context.createAccordionHandler(workflow.Id);
  const history = useHistory();
  return (
    <>
      <SecondaryAccordionTitle
        active={active}
        index={index}
        onClick={(e, p) => {
          history.push(ei.createWorkflowUrl(workflow));
          handleClick(e, p);
        }}
      >
        <Icon name="sitemap" style={{ marginRight: 8 }} />
        {workflow.Name}
      </SecondaryAccordionTitle>
      <AccordionContent active={active} className="secondary">
        <NestedAccordion>
          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={0}
            title="Actions"
            collection={workflow.Actions}
            route="action"
            viewAction={/*ei.store.viewStore.showActionClick*/ null}
            createAction={workflow.createAction}
            ei={ei}
          />

          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={1}
            title="States"
            collection={workflow.States}
            route="state"
            viewAction={/*ei.store.viewStore.showStateClick*/ null}
            createAction={workflow.createState}
            ei={ei}
          />

          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={2}
            title="Transitions"
            collection={workflow.Transitions}
            route="transition"
            viewAction={/*ei.store.viewStore.showTransitionClick*/ null}
            createAction={workflow.createTransition}
            ei={ei}
            showId={true}
          />

          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={3}
            title="Connections"
            collection={workflow.Connections}
            route="connection"
            viewAction={/*ei.store.viewStore.showConnectionClick*/ null}
            ei={ei}
            showId={true}
            createAction={workflow.addConnection}
          />
        </NestedAccordion>
      </AccordionContent>
    </>
  );
};

{
  /*
   */
}
