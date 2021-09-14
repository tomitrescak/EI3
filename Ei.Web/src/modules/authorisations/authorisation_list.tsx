import React from "react";

import { observer } from "mobx-react";
import { Accordion, Icon, Label, List } from "semantic-ui-react";

import { Link } from "react-router-dom";
import {
  AccordionButton,
  AccordionContent,
} from "../components/hierarchic_entity_view";
import { NestedAccordion } from "../components/workflow_list_view";
import { Ei } from "../ei/ei_model";

interface Props {
  active: boolean;
  index: number;
  ei: Ei;
  handleClick: any;
}

@observer
export class AuthorisationList extends React.Component<Props> {
  render() {
    const { active, index, ei, handleClick } = this.props;

    return (
      <>
        <Accordion.Title active={active} index={index} onClick={handleClick}>
          <Icon name="dropdown" />
          <Label
            size="tiny"
            color="blue"
            circular
            content={ei.Authorisation.length}
          />{" "}
          Authorisations
          <AccordionButton
            floated="right"
            icon="plus"
            compact
            color="green"
            onClick={ei.createAuthorisation}
          />
        </Accordion.Title>
        <AccordionContent active={active}>
          <NestedAccordion>
            {ei.Authorisation.map((authorisation, authIndex) => (
              <List.Item
                as={Link}
                to={`/ei/${ei.Name.toUrlName()}/${
                  ei.Id
                }/authorisation/${authIndex}`}
                key={index}
              >
                <Icon name={authorisation.Organisation ? "users" : "user"} />
                {authorisation.Organisation
                  ? ei.organisationName(authorisation.Organisation)
                  : authorisation.User
                  ? ei.roleName(authorisation.User)
                  : "<empty>"}
              </List.Item>
            ))}
          </NestedAccordion>
        </AccordionContent>
      </>
    );
  }
}
