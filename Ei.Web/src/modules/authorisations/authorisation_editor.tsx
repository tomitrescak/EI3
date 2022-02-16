import React from "react";

import { Header, Message } from "semantic-ui-react";

import { GroupsEditor } from "../core/group_editor";
import { useAppContext } from "../../config/context";
import styled from "@emotion/styled";
import { useQuery } from "../../helpers/client_helpers";
import { Formix, Input, Select } from "../Form";

const Pane = styled.div`
  padding: 12px;
  width: 100%;
  label: AuthorisationPane;
`;

export const AuthorisationEditor = () => {
  const context = useAppContext();
  const { id } = useQuery<{ id: string }>();

  let ei = context.ei;

  if (!ei) {
    return <Message content="Deleted" />;
  }

  let authorisation = ei.Authorisation[parseInt(id, 10)];

  if (!authorisation) {
    return <div>Authorisation deleted</div>;
  }

  return (
    <Formix initialValues={authorisation}>
      <Pane>
        <Input name="User" label="User" />
        <Select
          name="Organisation"
          label="Organisation"
          selection
          options={ei.removableOrganisationsOptions}
        />
        <Input name="Password" label="Password" />

        <Header as="h4" content="Role Assignments" icon="users" dividing />

        <GroupsEditor groups={authorisation.Groups} ei={ei} />
      </Pane>
    </Formix>
  );
};
