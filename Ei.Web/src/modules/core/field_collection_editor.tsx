import * as React from "react";

import { observer } from "mobx-react";
import { FieldCollection, Input } from "semantic-ui-mobx";
import { Button } from "semantic-ui-react";
import styled from "@emotion/styled";

interface Props {
  collection: FieldCollection<any>;
}

const IoPuts = styled(Input)`
  margin-bottom: 3px !important;
`;

@observer
export class FieldCollectionEditor extends React.Component<Props> {
  remove = (e: React.MouseEvent<HTMLDivElement>) => {
    const idx = parseInt(e.currentTarget.getAttribute("data-index"), 10);
    this.props.collection.removeAt(idx);
  };

  add = () => {
    this.props.collection.add("");
  };

  render() {
    return (
      <>
        {this.props.collection.array.map((_input, index) => (
          <IoPuts
            owner={this.props.collection.fields[index]}
            name="input"
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
