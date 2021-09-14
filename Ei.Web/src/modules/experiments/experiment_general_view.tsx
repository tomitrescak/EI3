import React from "react";

import { observer } from "mobx-react";
import { Header } from "semantic-ui-react";

@observer
export class ExperimentGeneral extends React.Component {
  render() {
    // let experiment = this.props.context.ei.Experiments[0];

    return (
      <div>
        <Header>General</Header>
        Here you write code for general
      </div>
    );
  }
}
