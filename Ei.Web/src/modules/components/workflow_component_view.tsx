import * as React from 'react';

import { IObservableArray } from 'mobx';
import { observer } from 'mobx-react';
import { Accordion, Button, Icon, Label, List } from 'semantic-ui-react';

import { Link } from '../../config/router';
import { AccordionHandler } from '../../config/store';
import { IconView } from '../core/entity_icon_view';
import { Entity, entitySort } from '../ei/entity_model';
import { Workflow } from '../ei/workflow_model';
import { accordionButton } from './hierarchic_entity_view';
import { accordionContent } from './workflow_list_view';

interface WorkflowElementProps {
  workflow: Workflow;
  handler: AccordionHandler;
  index: number;
  title: string;
  collection: IObservableArray<Entity>;
  route: string;
  viewAction: any;
  createAction?: any;
}
let entity: Entity;

export const WorkflowComponentList = observer(
  ({
    collection,
    createAction,
    handler,
    index,
    route,
    title,
    viewAction,
    workflow
  }: WorkflowElementProps) => (
    <>
      <Accordion.Title active={handler.isActive(index)} index={index} onClick={handler.handleClick}>
        <Icon name="dropdown" />
        <Label size="tiny" color="blue" circular content={collection.length} /> {title}
        {createAction && (
          <Button
            floated="right"
            icon="plus"
            color="green"
            compact
            className={accordionButton}
            onClick={createAction}
          />
        )}
      </Accordion.Title>
      <Accordion.Content active={handler.isActive(index)} className={accordionContent}>
        <If condition={collection.length === 0}>
          <span>Empty</span>
        </If>
        <List>
          <For each="entity" of={collection.sort(entitySort)}>
            <List.Item
              as={Link}
              to={`/workflows/${workflow.Name.toUrlName()}/${
                workflow.Id
              }/${route}/${entity.Id.toUrlName()}`}
              action={viewAction}
              key={entity.Id}
              data-workflow-id={workflow.Id}
              data-workflow-name={workflow.Name}
              data-id={entity.Id}
            >
              <IconView entity={entity} />
              {entity.Name || entity.Id}
            </List.Item>
          </For>
        </List>
      </Accordion.Content>
    </>
  )
);
