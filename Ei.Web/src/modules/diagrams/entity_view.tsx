import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form } from 'semantic-ui-mobx';

import { Message } from 'semantic-ui-react';
import { EntityEditor } from '../core/entity_view';
import { Ei } from '../ei/ei_model';
import { HierarchicEntity } from '../ei/hierarchic_entity_model';
import { PropertyView } from '../properties/property_view';

interface Props {
  context?: App.Context;
  collection: (ei: Ei) => HierarchicEntity[];
  id: string;
  name?: string;
}

@inject('context')
@observer
export class EntityView extends React.Component<Props> {
  static displayName = 'EntityView';

  render() {
    let ei = this.props.context.store.ei;
    let entity = this.props.collection(ei).find(o => o.Id === this.props.id);

    if (!entity) {
      return <Message content="Deleted" />
    }

    // let organisations = [{ value: '', text: 'No Parent' }]
    //   .concat(ei.Organisations.filter(o => o.Id !== organisation.Id).map(o => ({ value: o.Id, text: o.Name })));

    let parent = entity.Parent;
    if (parent) {
      parent = this.props.collection(ei).find(o => o.Id === parent).Name || '';
    }

    return (
      <Form>
        <EntityEditor entity={entity} />

        {/*<Select label="Parent" options={organisations} owner={getField(organisation, 'Parent')} placeholder="No Parent"  />*/}
        <If condition={parent}>
          <div><b>Parent: </b> { parent }</div>
        </If>
        <PropertyView owner={entity} types={ei.types} /> 
      </Form>
    );
  }
}
