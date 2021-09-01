import React from "react";

import { observer } from "mobx-react";
import { Accordion, Button, Icon, Label, List } from "semantic-ui-react";
import { style } from "typestyle";

import { IObservableArray } from "mobx";
import { Link } from "../../config/router";
import { IconView } from "../core/entity_icon_view";
import { Ei } from "../ei/ei_model";
import { entitySort } from "../ei/entity_model";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";

export const accordionButton = style({
  marginTop: "-3px!important",
  padding: "6px!important",
  marginRight: "6px!important",
  marginLeft: "0px !important",
});

export const accordionContent = style({
  // background: '#dedede',
  padding: "6px 6px 6px 25px!important",
});

let entity: HierarchicEntity;

interface Props {
  active: boolean;
  collection: IObservableArray<HierarchicEntity>;
  handleClick: any;
  index: number;
  title: string;
  url: string;
  ei: Ei;

  createEntity: (e: any) => void;
  showAll: (e: any) => void;
  showSingle: (id: string, name: string) => void;
}

export const HierarchicEntityView = observer(
  ({
    active,
    collection,
    createEntity,
    handleClick,
    index,
    showAll,
    showSingle,
    title,
    url,
    ei,
  }: Props) => (
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
        <Button
          floated="right"
          icon="plus"
          compact
          color="green"
          className={accordionButton}
          onClick={createEntity}
        />
        <Button
          floated="right"
          icon="sitemap"
          compact
          color="orange"
          className={accordionButton}
          to="/roles"
          onClick={showAll}
        />
      </Accordion.Title>
      <Accordion.Content active={active} className={accordionContent}>
        <List>
          {collection.sort(entitySort).map((entity) => (
            <List.Item
              as={Link}
              to={`/${ei.Name.toUrlName()}/${
                ei.id
              }/${url}/${entity.Name.toUrlName()}/${entity.Id}`}
              action={() => showSingle(entity.Id, entity.Name)}
              key={entity.Id}
            >
              <IconView entity={entity} />
              {entity.Name || entity.Id}
            </List.Item>
          ))}
        </List>
      </Accordion.Content>
    </>
  )
);
