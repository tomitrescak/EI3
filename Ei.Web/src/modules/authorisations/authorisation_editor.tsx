import * as React from "react";

import { observer } from "mobx-react";
import { Form, Input, Select } from "semantic-ui-mobx";
import { Header, Message } from "semantic-ui-react";
import { style } from "typestyle";

import { GroupsEditor } from "../core/group_editor";
import { useAppContext } from "../../config/context";
import { useParams } from "react-router-dom";

interface Props {
  index: string;
}

const pane = style({
  padding: "12px",
});

export const AuthorisationEditor = () => {
  const context = useAppContext();
  const { authorisationId } = useParams<{ authorisationId: string }>();

  let ei = context.ei;

  if (!ei) {
    return <Message content="Deleted" />;
  }

  let authorisation = ei.Authorisation[parseInt(authorisationId, 10)];

  if (!authorisation) {
    return <div>Authorisation deleted</div>;
  }

  return (
    <Form className={pane}>
      <Input owner={authorisation.fields.User} label="User" />
      <Select
        owner={authorisation.fields.Organisation}
        label="Organisation"
        options={ei.removableOrganisationsOptions}
      />
      <Input owner={authorisation.fields.Password} label="Password" />

      <Header as="h4" content="Role Assignments" icon="users" dividing />

      <GroupsEditor groups={authorisation.Groups} ei={ei} />
    </Form>
  );
};
