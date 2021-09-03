import React from "react";

import { Accordion, Icon, Label, List, Loader, Menu } from "semantic-ui-react";

import { observer } from "mobx-react";
import styled from "@emotion/styled";
import { AuthorisationList } from "../authorisations/authorisation_list";
import {
  AccordionContent,
  HierarchicEntityView,
} from "./hierarchic_entity_view";
import { WorkflowList } from "./workflow_list_view";
import { Link, useParams } from "react-router-dom";
import { useAppContext } from "../../config/context";

const ComponentsType = styled.div`
  height: 100%;
  overflow: auto;
  padding-bottom: 40px;
`;

export const Components = observer(() => {
  const context = useAppContext();
  const handler = React.useMemo(
    () => context.createAccordionHandler("root"),
    []
  );
  const ei = context.ei;
  const store = context;
  const { eiId, eiName } =
    useParams<{ eiId: string; eiName: string; editor: string }>();

  const compile = () => {
    context.ei.compile(context.client);
  };

  return (
    <>
      <Menu
        inverted
        attached="top"
        color="blue"
        style={{ borderRadius: "0px" }}
      >
        <Menu.Item>
          <Link to={`/${eiId}/${eiName}`}>Ei</Link>
        </Menu.Item>
        <Menu.Menu position="right">
          <Menu.Item
            icon="play"
            as={Link}
            to={`/${ei.Name.toUrlName()}/${ei.id}/execution`}
          />
          <Menu.Item
            icon="reply"
            onClick={context.Ui.history.undo}
            title="Undo"
          />
          <Menu.Item
            icon="mail forward"
            onClick={context.Ui.history.redo}
            title="Redo"
          />
          {store.compiling ? (
            <Menu.Item title="Compiling">
              <Loader active inline size="tiny" />
            </Menu.Item>
          ) : (
            <Menu.Item icon="cogs" onClick={compile} title="Compile Solution" />
          )}
          <Menu.Item icon="save" onClick={ei.save} />
        </Menu.Menu>
      </Menu>
      <ComponentsType>
        <Accordion>
          <HierarchicEntityView
            active={handler.isActive(0)}
            handleClick={handler.handleClick}
            collection={ei.Roles}
            createEntity={ei.createRole}
            index={0}
            url="roles"
            title="Roles"
            ei={ei}
          />

          <HierarchicEntityView
            active={handler.isActive(1)}
            handleClick={handler.handleClick}
            collection={ei.Organisations}
            createEntity={ei.createOrganisation}
            index={1}
            url="organisations"
            title="Organisations"
            ei={ei}
          />

          <HierarchicEntityView
            active={handler.isActive(2)}
            collection={ei.Types}
            createEntity={ei.createType}
            handleClick={handler.handleClick}
            index={2}
            url="types"
            title="Types"
            ei={ei}
          />

          <WorkflowList
            active={handler.isActive(3)}
            index={3}
            handleClick={handler.handleClick}
            ei={ei}
          />

          <AuthorisationList
            active={handler.isActive(4)}
            index={4}
            handleClick={handler.handleClick}
            ei={ei}
          />

          <Accordion.Title
            active={handler.isActive(5)}
            index={5}
            onClick={handler.handleClick}
          >
            <Icon name="dropdown" />
            <Label size="tiny" color="blue" circular content="1" /> Experiments
          </Accordion.Title>

          <AccordionContent active={handler.isActive(5)}>
            <List>
              <List.Item
                as={Link}
                to={`/${ei.Name.toUrlName()}/${
                  ei.id
                }/experiment/default/general/1`}
              >
                General
              </List.Item>
              <List.Item
                as={Link}
                to={`/${ei.Name.toUrlName()}/${
                  ei.id
                }/experiment/default/agents/1`}
              >
                Agents
              </List.Item>
              <List.Item
                as={Link}
                to={`/${ei.Name.toUrlName()}/${
                  ei.id
                }/experiment/default/environment/1`}
              >
                Environment
              </List.Item>
            </List>
          </AccordionContent>
        </Accordion>
      </ComponentsType>
    </>
  );
});
