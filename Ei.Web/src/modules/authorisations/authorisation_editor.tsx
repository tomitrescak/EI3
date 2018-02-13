import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form, Input, Select } from 'semantic-ui-mobx';
import { Header, Message } from 'semantic-ui-react';
import { style } from 'typestyle';

import { GroupsEditor } from '../core/group_editor';

interface Props {
  context?: App.Context;
  index: string;
}

const pane = style({
  padding: '12px'
});

@inject('context')
@observer
export class AuthorisationEditor extends React.Component<Props> {
  static displayName = 'EiEditor';

  render() {
    let ei = this.props.context.store.ei;

    if (!ei) {
      return <Message content="Deleted" />;
    }

    let authorisation = ei.Authorisation[parseInt(this.props.index, 10)];

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
  }
}
