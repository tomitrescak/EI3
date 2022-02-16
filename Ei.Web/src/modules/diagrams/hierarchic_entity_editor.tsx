import React from "react";

import { observer } from "mobx-react";

import { Button, Message } from "semantic-ui-react";

import { IObservableArray } from "mobx";

import { EntityEditor } from "../core/entity_view";
import { Ei } from "../ei/ei_model";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { PropertyView } from "../properties/property_view";
import { useAppContext } from "../../config/context";
import { useQuery } from "../../helpers/client_helpers";

interface Props {
  collection: (ei: Ei) => IObservableArray<HierarchicEntity>;
}

// const deleteButton = style({
//   marginTop: '10px',
//   textAlign: 'center'
// });

export const HierarchicEntityEditor = observer((props: Props) => {
  // deleteRecord = action(() => {
  //   let ei = this.props.context.ei;
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

  let context = useAppContext();
  const { id } = useQuery();

  console.log(id);

  if (!id) {
    return null;
  }

  let ei = context.ei;
  let entity = props
    .collection(ei)
    .find((o) => o.Id.toLowerCase() === id.toLowerCase());

  // React.useEffect(() => {
  //   if (
  //     entity &&
  //     ei.context.selectedEntity &&
  //     ei.context.selectedEntity.Id != entity.Id &&
  //     ei.context.selectedEntity.selected
  //   ) {
  //     ei.context.selectedEntity.setSelected(false);
  //   }
  //   if (entity && !entity.selected) {
  //     entity.setSelected();
  //     ei.context.selectedEntity = entity;
  //   }
  // });

  if (!entity) {
    return <Message content="Deleted" />;
  }

  // let organisations = [{ value: '', text: 'No Parent' }]
  //   .concat(ei.Organisations.filter(o => o.Id !== organisation.Id).map(o => ({ value: o.Id, text: o.Name })));

  let parent = entity.ParentId;
  if (parent) {
    parent = props.collection(ei).find((o) => o.Id === parent).Name || "";
  }

  return (
    <>
      <EntityEditor entity={entity} />

      {/*<Select label="Parent" options={organisations} owner={getField(organisation, 'Parent')} placeholder="No Parent"  />*/}
      {parent && (
        <div>
          <b>Parent: </b> {parent}
        </div>
      )}
      <PropertyView owner={entity} types={ei.types} />

      <Button
        style={{ margin: "auto", marginTop: 8 }}
        icon="trash"
        content="Delete"
        labelPosition="left"
        color="red"
        onClick={async () => {
          if (
            await context.Ui.confirmDialogAsync(
              "Do you wish to delete this record?"
            )
          ) {
            props
              .collection(ei)
              .splice(props.collection(ei).indexOf(entity), 1);
          }
        }}
      />
    </>
  );
});
