import * as React from "react";

import { IObservableArray } from "mobx";
import { observer } from "mobx-react";
import { Accordion, Button, Icon, Label, List } from "semantic-ui-react";
import { style } from "typestyle";

import { Link } from "react-router-dom";
import { AccordionHandler } from "../../config/store";
import { IconView } from "../core/entity_icon_view";
import { Ei } from "../ei/ei_model";
import { Entity, entitySort } from "../ei/entity_model";
import { Workflow } from "../ei/workflow_model";
import { AccordionButton } from "./hierarchic_entity_view";
import { accordionContent } from "./workflow_list_view";

interface WorkflowElementProps {
  workflow: Workflow;
  handler: AccordionHandler;
  index: number;
  title: string;
  collection: IObservableArray<Entity>;
  route: string;
  viewAction: any;
  createAction?: any;
  ei: Ei;
  showId?: boolean;
}
let entity: Entity;

let idLabel = style({
  color: "grey",
  fontSize: "9px",
  float: "right",
  marginRight: "12px",
});

export const WorkflowComponentList = observer(
  ({
    collection,
    createAction,
    ei,
    handler,
    index,
    route,
    title,
    viewAction,
    workflow,
    showId,
  }: WorkflowElementProps) => (
    <>
      <Accordion.Title
        active={handler.isActive(index)}
        index={index}
        onClick={handler.handleClick}
      >
        <Icon name="dropdown" />
        <Label
          size="tiny"
          color="blue"
          circular
          content={collection.length}
        />{" "}
        {title}
        {createAction && (
          <AccordionButton
            floated="right"
            icon="plus"
            color="green"
            compact
            onClick={createAction}
          />
        )}
      </Accordion.Title>
      <Accordion.Content
        active={handler.isActive(index)}
        className={accordionContent}
      >
        {collection.length === 0 && <span>Empty</span>}
        <List>
          {collection
            .slice()
            .sort(entitySort)
            .map((entity) => (
              <List.Item
                as={Link}
                to={`/${ei.Name.toUrlName()}/${
                  ei.id
                }/workflows/${workflow.Name.toUrlName()}/${
                  workflow.Id
                }/${route}/${entity.Id.toUrlName()}`}
                action={viewAction}
                key={entity.Id}
                data-workflow-id={workflow.Id}
                data-workflow-name={workflow.Name}
                data-id={entity.Id}
              >
                <IconView entity={entity} />
                {entity.Name || entity.Id}{" "}
                {showId && <span className={idLabel}>[{entity.Id}]</span>}
              </List.Item>
            ))}
        </List>
      </Accordion.Content>
    </>
  )
);
