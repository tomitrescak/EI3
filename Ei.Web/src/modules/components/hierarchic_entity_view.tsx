import React from "react";

import { Button, Label, List } from "semantic-ui-react";

import { IconView } from "../core/entity_icon_view";
import { entitySort } from "../ei/entity_model";
import { Link, useHistory, useLocation } from "react-router-dom";
import styled from "@emotion/styled";
import { IObservableArray } from "mobx";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { observer } from "mobx-react";
import { Ei } from "../ei/ei_model";
import { AccordionTitle, AccordionContent } from "./accordion";

export const AccordionButton = styled(Button)`
  margin-top: -3px !important;
  padding: 6px !important;
  margin-right: 6px !important;
  margin-left: 0px !important;
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
    const location = useLocation();
    const history = useHistory();
    return (
      <>
        <AccordionTitle
          active={active}
          index={index}
          onClick={() => {
            handleClick();
            history.push(`/ei/${ei.Name.toUrlName()}/${ei.Id}/${url}`);
          }}
        >
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
        </AccordionTitle>
        <AccordionContent active={active}>
          {collection.length == 0 && <span>0 records</span>}
          <List>
            {collection
              .slice()
              .sort(entitySort)
              .map((entity) => (
                <List.Item
                  as={Link}
                  active={entity.url === location.pathname}
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
