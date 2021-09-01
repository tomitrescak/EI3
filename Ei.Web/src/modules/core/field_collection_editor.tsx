import * as React from "react";

import { observer } from "mobx-react";
import { FieldCollection, Input } from "semantic-ui-mobx";
import { Button } from "semantic-ui-react";
import { style } from "typestyle/lib";

interface Props {
  collection: FieldCollection<any>;
}

const ioPuts = style({ marginBottom: "3px!important" });
let index = 0;

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
        {this.props.collection.array.map((input, index) => (
          <Input
            className={ioPuts}
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
