import * as React from "react";

import { observer } from "mobx-react";
import { Button } from "semantic-ui-react";
import styled from "@emotion/styled";
import { Input } from "../Form";

interface Props {
  collection: Array<any>;
  collectionName: string;
}

const IoPuts = styled(Input)`
  margin-bottom: 3px !important;
`;

export let FieldCollectionEditor = (props: Props) => {
  return (
    <>
      {props.collection.map((_input, index) => (
        <IoPuts
          name={`${props.collectionName}[${index}]`}
          action={{
            color: "red",
            icon: "trash",
            onClick: (e: React.MouseEvent<HTMLDivElement>) => {
              const idx = parseInt(
                e.currentTarget.getAttribute("data-index"),
                10
              );
              props.collection.splice(idx, 1);
            },
            "data-index": index,
            type: "button",
            name: "removeInput",
          }}
          key={index}
        />
      ))}

      <Button
        type="button"
        name="addInput"
        primary
        onClick={() => {
          props.collection.push("");
        }}
        icon="plus"
        content={`Add`}
      />
    </>
  );
};

FieldCollectionEditor = observer(FieldCollectionEditor);
