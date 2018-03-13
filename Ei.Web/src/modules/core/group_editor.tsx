import * as React from 'react';

import { IObservableArray } from 'mobx';
import { observer } from 'mobx-react';
import { Form, Select } from 'semantic-ui-mobx';

import { Button } from 'semantic-ui-react';
import { Ei } from '../ei/ei_model';
import { Group } from '../ei/group_model';

interface GroupProps {
  group: Group;
  ei: Ei;
  remove: any;
}

export const GroupEditor = observer(({ group, ei, remove }: GroupProps) => (
  <Form.Group>
    <Select width={6} options={ei.organisationsOptions} owner={group.fields.OrganisationId} fluid />
    <Select width={6} options={ei.roleOptions} owner={group.fields.RoleId} fluid />
    <Button
      width={1}
      type="button"
      name="addInput"
      color="red"
      onClick={remove}
      icon="trash"
    />
  </Form.Group>
));

interface GroupsProps {
  groups: IObservableArray<Group>;
  ei: Ei;
}

export const GroupsEditor = observer(({ groups, ei }: GroupsProps) => (
  <>
    { groups.map((g, i) => <GroupEditor group={g} ei={ei} key={i} remove={() => groups.splice(i, 1)} />) }

    <Button
      type="button"
      name="addInput"
      primary
      onClick={() =>
        groups.push(new Group({ OrganisationId: ei.Organisations[0].Id, RoleId: ei.Roles[0].Id }))
      }
      icon="plus"
      content={`Add`}
    />
  </>
));
