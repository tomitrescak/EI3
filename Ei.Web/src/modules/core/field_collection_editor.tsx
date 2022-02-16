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

@observer
export class FieldCollectionEditor extends React.Component<Props> {
  remove = (e: React.MouseEvent<HTMLDivElement>) => {
    const idx = parseInt(e.currentTarget.getAttribute("data-index"), 10);
    this.props.collection.splice(idx, 1);
  };

  add = () => {
    this.props.collection.push("");
  };

  render() {
    return (
      <>
        {this.props.collection.map((_input, index) => (
          <IoPuts
            name={`${this.props.collectionName}[${index}]`}
            action={{
              color: "red",
              icon: "trash",
              onClick: this.remove,
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
          onClick={this.add}
          icon="plus"
          content={`Add`}
        />
      </>
    );
  }
}
