import * as React from 'react';

import { observer } from 'mobx-react';
import { Accordion, Button, Icon, Label, List } from 'semantic-ui-react';

import { Link } from '../../config/router';
import { accordionButton, accordionContent } from '../components/hierarchic_entity_view';
import { nestedAccordion } from '../components/workflow_list_view';
import { Authorisation } from '../ei/authorisation_model';
import { Ei } from '../ei/ei_model';

interface Props {
  active: boolean;
  index: number;
  ei: Ei;
  handleClick: any;
}

let authorisation: Authorisation;
let autIndex: number;

@observer
export class AuthorisationList extends React.Component<Props> {
  render() {
    const { active, index, ei, handleClick } = this.props;

    return (
      <>
        <Accordion.Title active={active} index={index} onClick={handleClick}>
          <Icon name="dropdown" />
          <Label size="tiny" color="blue" circular content={ei.Authorisation.length} /> Authorisations
          <Button
            floated="right"
            icon="plus"
            compact
            color="green"
            className={accordionButton}
            onClick={ei.createAuthorisation}
          />
        </Accordion.Title>
        <Accordion.Content active={active} className={accordionContent}>
          <Accordion className={nestedAccordion}>
            <For each="authorisation" of={ei.Authorisation} index="autIndex">
              <List.Item
                as={Link}
                to={`/${ei.Name.toUrlName()}/${ei.id}/authorisation/${index}`}
                action={() => ei.store.viewStore.showAuthorisation(autIndex.toString())}
                key={index}
              >
                <Icon name={authorisation.Organisation ? 'users' : 'user'} />
                { authorisation.Organisation ? ei.organisationName(authorisation.Organisation) : (authorisation.User ? ei.roleName(authorisation.User) : '<empty>') }
              </List.Item>
            </For>
          </Accordion>
        </Accordion.Content>
      </>
    );
  }
}
