import * as React from 'react';

import { observer } from 'mobx-react';
import { Accordion, Button, Icon, Label } from 'semantic-ui-react';
import { style } from 'typestyle';

import { Ei } from '../ei/ei_model';
import { Workflow } from '../ei/workflow_model';
import { accordionButton } from './hierarchic_entity_view';
import { WorkflowComponentList } from './workflow_component_view';

interface Props {
  active: boolean;
  index: number;
  ei: Ei;
  handleClick: any;
}

let workflowItem: Workflow;

export const accordionContent = style({
  // background: '#efefef',
  padding: '0px 0px 6px 25px!important'
});

export const nestedAccordion = style({
  margin: '0px!important'
});

@observer
export class WorkflowList extends React.Component<Props> {
  render() {
    const { active, index, ei, handleClick } = this.props;
    const handler = ei.store.createAccordionHandler('workflows');

    return (
      <>
        <Accordion.Title active={active} index={index} onClick={handleClick}>
          <Icon name="dropdown" />
          <Label size="tiny" color="blue" circular content={ei.Workflows.length} /> Workflows
          <Button
            floated="right"
            icon="plus"
            compact
            color="green"
            className={accordionButton}
            onClick={ei.createWorkflow}
          />
        </Accordion.Title>
        <Accordion.Content active={active} className={accordionContent}>
          <Accordion className={nestedAccordion}>
            <For each="workflowItem" of={ei.Workflows}>
              <WorkflowDetail
                key={workflowItem.Id}
                index={workflowItem.Id.hashCode()}
                handleClick={handler.handleClick}
                workflow={workflowItem}
                active={handler.isActive(workflowItem.Id.hashCode())}
                ei={ei}
              />
            </For>
          </Accordion>
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

export const WorkflowDetail = ({ ei, workflow, active, index, handleClick }: DetailProps) => {
  const handler = ei.store.createAccordionHandler(workflow.Id);
  return (
    <>
      <Accordion.Title active={active} index={index} onClick={handleClick}>
        <Icon name="dropdown" />
        <Icon name="sitemap" />
        {workflow.Name}
        <Button
          to={`/workflows/${workflow.Name.toUrlName()}/${workflow.Id.toUrlName()}`}
          floated="right"
          icon="sitemap"
          compact
          color="orange"
          className={accordionButton}
          onClick={(e) => { e.stopPropagation(); ei.store.viewStore.showWorkflow(workflow.Id, workflow.Name) } }
        />
      </Accordion.Title>
      <Accordion.Content active={active} className={accordionContent}>
        <Accordion className={nestedAccordion}>
          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={0}
            title="Actions"
            collection={workflow.Actions}
            route="action"
            viewAction={ei.store.viewStore.showActionClick}
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
            viewAction={ei.store.viewStore.showStateClick}
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
            viewAction={ei.store.viewStore.showTransitionClick}
            createAction={workflow.createTransition}
            ei={ei}
          />

          <WorkflowComponentList
            workflow={workflow}
            handler={handler}
            index={3}
            title="Connections"
            collection={workflow.Connections}
            route="connection"
            viewAction={ei.store.viewStore.showConnectionClick}
            ei={ei}
          />

        </Accordion>
      </Accordion.Content>
    </>
  );
};

{
  /* 
   */
}
