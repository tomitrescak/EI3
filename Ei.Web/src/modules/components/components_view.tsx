import * as React from 'react';

import { Accordion, Icon, Label, List, Loader, Menu } from 'semantic-ui-react';

import { inject, observer } from 'mobx-react';
import { style } from 'typestyle';
import { Link } from '../../config/router';
import { AccordionHandler } from '../../config/store';
import { AuthorisationList } from '../authorisations/authorisation_list';
import { SocketClient } from '../ws/socket_client';
import { accordionContent, HierarchicEntityView } from './hierarchic_entity_view';
import { WorkflowList } from './workflow_list_view';

const componentsType = style({
  height: '100%',
  overflow: 'auto',
  paddingBottom: '40px'
});

interface Props {
  context?: App.Context;
  client?: SocketClient;
}

interface State {
  activeIndices: number[];
}

@inject('context', 'client')
@observer
export class Components extends React.Component<Props, State> {
  static displayName = 'ComponentView';

  handler: AccordionHandler;

  componentWillMount() {
    this.handler = this.props.context.store.createAccordionHandler('root');
  }

  showRoles = (e: any) => {
    e.stopPropagation();
    this.props.context.store.viewStore.showView('roles');
  };
  showRole = (id: string, name: string) => this.props.context.store.viewStore.showRole(id, name);

  showOrganisations = (e: any) => {
    e.stopPropagation();
    this.props.context.store.viewStore.showView('organisations');
  };
  showOrganisation = (id: string, name: string) =>
    this.props.context.store.viewStore.showOrganisation(id, name);

  showTypes = (e: any) => {
    e.stopPropagation();
    this.props.context.store.viewStore.showView('types');
  };
  showType = (id: string, name: string) => this.props.context.store.viewStore.showType(id, name);

  compile = () => {
    this.props.context.store.ei.compile(this.props.client);
  };

  render() {
    const context = this.props.context;
    const ei = this.props.context.store.ei;
    const store = this.props.context.store;

    return (
      <>
        <Menu inverted attached="top" color="blue" style={{ borderRadius: '0px' }}>
          <Menu.Item>
            <Link to="/" action={() => store.viewStore.showView('ei')}>
              Ei
            </Link>
          </Menu.Item>
          <Menu.Menu position="right">
            <Menu.Item icon="reply" onClick={context.Ui.history.undo} title="Undo" />
            <Menu.Item icon="mail forward" onClick={context.Ui.history.redo} title="Redo" />
            {store.compiling ? (
              <Menu.Item title="Compiling">
                <Loader active inline size="tiny" />
              </Menu.Item>
            ) : (
              <Menu.Item icon="cogs" onClick={this.compile} title="Compile Solution" />
            )}
            <Menu.Item icon="save" onClick={ei.save} />
          </Menu.Menu>
        </Menu>
        <div className={componentsType}>
          <Accordion>
            <HierarchicEntityView
              active={this.handler.isActive(0)}
              collection={ei.Roles}
              createEntity={ei.createRole}
              handleClick={this.handler.handleClick}
              index={0}
              showAll={this.showRoles}
              showSingle={this.showRole}
              url="roles"
              title="Roles"
              ei={ei}
            />

            <HierarchicEntityView
              active={this.handler.isActive(1)}
              collection={ei.Organisations}
              createEntity={ei.createOrganisation}
              handleClick={this.handler.handleClick}
              index={1}
              showAll={this.showOrganisations}
              showSingle={this.showOrganisation}
              url="organisations"
              title="Organisations"
              ei={ei}
            />

            <HierarchicEntityView
              active={this.handler.isActive(2)}
              collection={ei.Types}
              createEntity={ei.createType}
              handleClick={this.handler.handleClick}
              index={2}
              showAll={this.showTypes}
              showSingle={this.showType}
              url="types"
              title="Types"
              ei={ei}
            />

            <WorkflowList
              active={this.handler.isActive(3)}
              index={3}
              handleClick={this.handler.handleClick}
              ei={ei}
            />

            <AuthorisationList
              active={this.handler.isActive(4)}
              index={4}
              handleClick={this.handler.handleClick}
              ei={ei}
            />

            <Accordion.Title
              active={this.handler.isActive(5)}
              index={5}
              onClick={this.handler.handleClick}
            >
              <Icon name="dropdown" />
              <Label size="tiny" color="blue" circular content="1" /> Execution
            </Accordion.Title>

            <Accordion.Content active={this.handler.isActive(5)} className={accordionContent}>
              <List>
                <List.Item
                  as={Link}
                  to={`/${ei.Name.toUrlName()}/${ei.id}/experiment/default/general/1`}
                  action={() => context.store.viewStore.showExecution(ei.id, ei.Name.toUrlName(), '1', 'default', 'experimentGeneral')}
                >
                  General
                </List.Item>
                <List.Item
                  as={Link}
                  to={`/${ei.Name.toUrlName()}/${ei.id}/experiment/default/agents/1`}
                  action={() => context.store.viewStore.showExecution(ei.id, ei.Name.toUrlName(), '1', 'default', 'experimentAgents')}
                >
                  Agents
                </List.Item>
                <List.Item
                  as={Link}
                  to={`/${ei.Name.toUrlName()}/${ei.id}/experiment/default/environment/1`}
                  action={() => context.store.viewStore.showExecution(ei.id, ei.Name.toUrlName(), '1', 'default', 'experimentEnvironment')}
                >
                  Environment
                </List.Item>
              </List>
            </Accordion.Content>
          </Accordion>
        </div>
      </>
    );
  }
}
