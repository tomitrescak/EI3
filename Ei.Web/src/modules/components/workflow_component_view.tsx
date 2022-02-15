import * as React from "react";

import { IObservableArray } from "mobx";
import { observer } from "mobx-react";
import { Accordion, Icon, Label, List } from "semantic-ui-react";

import { Link, useLocation } from "react-router-dom";
import { IconView } from "../core/entity_icon_view";
import { Ei } from "../ei/ei_model";
import { Entity, entitySort } from "../ei/entity_model";
import { Workflow } from "../ei/workflow_model";
import { AccordionButton } from "./hierarchic_entity_view";
import { AccordionHandler } from "../../config/context";
import styled from "@emotion/styled";
import { AccordionContent } from "./accordion";

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

let IdLabel = styled.span`
  color: grey;
  font-size: 9px;
  float: right;
  margin-right: 12px;
`;

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
  }: WorkflowElementProps) => {
    const history = useLocation();
    // const context = useAppContext();
    return (
      <>
        <Accordion.Title
          active={handler.isActive(index)}
          index={index}
          onClick={handler.handleClick}
        >
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
        <div style={{ paddingLeft: 16 }}>
          {collection.length === 0 && <span>Empty</span>}
          <List>
            {collection
              .slice()
              .sort(entitySort)
              .map((entity) => {
                const url =
                  `/ei/${ei.Name.toUrlName()}/workflows/${workflow.Name.toUrlName()}/${route}/${entity.Name.toUrlName()}?ei=${
                    ei.Id
                  }&id=${entity.Id.toUrlName()}&w=${workflow.Id}`.toLowerCase();

                // if (url === history.pathname) {
                // entity.setSelected(url === history.pathname);
                // entity.selected = url === history.pathname;
                // if (context.repaint) {
                //   context.repaint();
                // }
                // }

                return (
                  <List.Item
                    as={Link}
                    to={url}
                    action={viewAction}
                    key={entity.Id}
                    active={url === history.pathname}
                    data-workflow-id={workflow.Id}
                    data-workflow-name={workflow.Name}
                    data-id={entity.Id}
                  >
                    <IconView entity={entity} />
                    {entity.Name || entity.Id}{" "}
                    {showId && <IdLabel>[{entity.Id}]</IdLabel>}
                  </List.Item>
                );
              })}
          </List>
        </div>
      </>
    );
  }
);
