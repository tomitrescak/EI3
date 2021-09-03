import * as React from "react";

import { inject, observer } from "mobx-react";
import { Header } from "semantic-ui-react";

interface Props {
  context?: App.Context;
}

@inject("context")
@observer
export class ExperimentGeneral extends React.Component<Props> {
  render() {
    let experiment = this.props.context.ei.Experiments[0];

    return (
      <div>
        <Header>General</Header>
        Here you write code for general
      </div>
    );
  }
}
