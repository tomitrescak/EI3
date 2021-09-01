import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { Form } from 'semantic-ui-mobx';

import { Message } from 'semantic-ui-react';

import { IObservableArray } from 'mobx';

import { EntityEditor } from '../core/entity_view';
import { Ei } from '../ei/ei_model';
import { HierarchicEntity } from '../ei/hierarchic_entity_model';
import { PropertyView } from '../properties/property_view';

interface Props {
  context?: App.Context;
  collection: (ei: Ei) => IObservableArray<HierarchicEntity>;
  id: string;
  name?: string;
  parentView: string;
  minCount: number;
}

// const deleteButton = style({
//   marginTop: '10px',
//   textAlign: 'center'
// });

@inject('context')
@observer
export class HierarchicEntityEditor extends React.Component<Props> {
  static displayName = 'EntityView';

  // deleteRecord = action(() => {
  //   let ei = this.props.context.store.ei;
  //   let collection = this.props.collection(ei);
  //   let entity = collection.find(o => o.Id === this.props.id);

    
  //   // remove all child link
  //   if (entity.parentLink) {
  //     entity.parentLink.safeRemove(entity.model);
  //   }

  //   // find children
  //   for (let child of collection.filter(c => c.Parent === entity.Id)) {
  //     child.parentLink.safeRemove(entity.model);
  //   }
    
  //   // remove entity
  //   collection.remove(entity);

  //   ei.store.viewStore.showView(this.props.parentView);
  // });

  // delete = async () => {
  //   if (
  //     await this.props.context.Ui.confirmDialogAsync(
  //       'Do you want to delete this record? This can break your existing references!',
  //       'Deleting record'
  //     )
  //   ) {
  //     this.deleteRecord();
  //   }
  // };

  render() {
    let ei = this.props.context.store.ei;
    let entity = this.props.collection(ei).find(o => o.Id.toLowerCase() === this.props.id.toLowerCase());

    if (!entity) {
      return <Message content="Deleted" />;
    }

    // let organisations = [{ value: '', text: 'No Parent' }]
    //   .concat(ei.Organisations.filter(o => o.Id !== organisation.Id).map(o => ({ value: o.Id, text: o.Name })));

    let parent = entity.Parent;
    if (parent) {
      parent = this.props.collection(ei).find(o => o.Id === parent).Name || '';
    }

    return (
      <>
        <Form>
          <EntityEditor entity={entity} />

          {/*<Select label="Parent" options={organisations} owner={getField(organisation, 'Parent')} placeholder="No Parent"  />*/}
          <If condition={parent}>
            <div>
              <b>Parent: </b> {parent}
            </div>
          </If>
          <PropertyView owner={entity} types={ei.types} />
        </Form>
        {/*this.props.minCount < this.props.collection(ei).length && (
          <div className={deleteButton}>
            <Header as="h5" style={{ color: 'red' }} dividing />
            <Button
              style={{ margin: 'auto' }}
              icon="trash"
              content="Delete"
              labelPosition="left"
              color="red"
              onClick={this.delete}
            />
          </div>
        )*/}
      </>
    );
  }
}
