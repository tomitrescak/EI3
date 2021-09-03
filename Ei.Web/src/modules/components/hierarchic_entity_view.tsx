import React from "react";

import { Accordion, Button, Icon, Label, List } from "semantic-ui-react";

import { IconView } from "../core/entity_icon_view";
import { entitySort } from "../ei/entity_model";
import { Link, useHistory, useLocation } from "react-router-dom";
import styled from "@emotion/styled";
import { IObservableArray } from "mobx";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { observer } from "mobx-react";
import { Ei } from "../ei/ei_model";

export const AccordionButton = styled(Button)`
  margin-top: -3px !important;
  padding: 6px !important;
  margin-right: 6px !important;
  margin-left: 0px !important;
`;

export const AccordionContent = styled(Accordion.Content)`
  // background: '#dedede',
  padding: 6px 6px 6px 25px !important;

  .item {
    border-radius: 3px;
    padding: 4px !important;
  }
  .active {
    background: #cdcdcd;
  }
`;

interface Props {
  active: boolean;
  collection: IObservableArray<HierarchicEntity>;
  handleClick: any;
  index: number;
  title: string;
  url: string;
  ei: Ei;
  createEntity: (e: any) => void;
}

export const HierarchicEntityView = observer(
  ({
    active,
    collection,
    createEntity,
    handleClick,
    index,
    title,
    url,
    ei,
  }: Props) => {
    const history = useLocation();
    return (
      <>
        <Accordion.Title active={active} index={index} onClick={handleClick}>
          <Icon name="dropdown" />
          <Label
            size="tiny"
            color="blue"
            circular
            content={collection.length}
          />{" "}
          {title}
          <AccordionButton
            floated="right"
            icon="plus"
            compact
            color="green"
            onClick={createEntity}
          />
          <AccordionButton
            as={Link}
            to={`/ei/${ei.Name.toUrlName()}/${ei.id}/${url}`}
            floated="right"
            icon="sitemap"
            compact
            color="orange"
          />
        </Accordion.Title>
        <AccordionContent active={active}>
          <List>
            {collection
              .slice()
              .sort(entitySort)
              .map((entity) => (
                <List.Item
                  as={Link}
                  active={entity.url === history.pathname}
                  to={entity.url}
                  key={entity.Id}
                >
                  <IconView entity={entity} />
                  {entity.Name || entity.Id}
                </List.Item>
              ))}
          </List>
        </AccordionContent>
      </>
    );
  }
);
