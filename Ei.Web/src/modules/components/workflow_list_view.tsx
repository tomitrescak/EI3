import React from "react";

import { observer } from "mobx-react";
import { Accordion, Icon, Label } from "semantic-ui-react";

import { Ei } from "../ei/ei_model";
import { Workflow } from "../ei/workflow_model";
import { AccordionButton } from "./hierarchic_entity_view";
import { WorkflowComponentList } from "./workflow_component_view";
import { Link } from "react-router-dom";
import styled from "@emotion/styled";
import { AccordionContent, AccordionTitle } from "./accordion";

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
        <AccordionContent active={active}>
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
        </AccordionContent>
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
  return (
    <>
      <Accordion.Title active={active} index={index} onClick={handleClick}>
        <Icon name="dropdown" />
        {/* <Icon name="sitemap" /> */}
        {workflow.Name}
        <AccordionButton
          as={Link}
          to={ei.createWorkflowUrl(workflow)}
          floated="right"
          icon="sitemap"
          compact
          color="orange"
          onClick={(e) => e.stopPropagation()}
        />
      </Accordion.Title>
      <AccordionContent active={active}>
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
