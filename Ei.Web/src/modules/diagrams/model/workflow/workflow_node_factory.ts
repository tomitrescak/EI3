import * as SRD from "@projectstorm/react-diagrams";
import { DefaultNodeWidget } from "@projectstorm/react-diagrams";
import React from "react";
import { FreeJoint } from "../../../ei/connection_model";

import { State } from "../../../ei/state_model";
import { TransitionJoin, TransitionSplit } from "../../../ei/transition_model";
import { FreeWidgetFactory } from "./widget_free";
import { StateWidgetFactory } from "./widget_state";
import { TransitionJoinWidgetFactory } from "./widget_transition_join";
import { TransitionSplitWidgetFactory } from "./widget_transition_split";

export class WorkflowNodeFactory extends SRD.DefaultNodeFactory {
  constructor() {
    super();
  }

  generateReactWidget({ model }: { model: SRD.NodeModel }): JSX.Element {
    if (model.constructor.name === "State") {
      return StateWidgetFactory({ node: model as State });
    } else if (model.constructor.name === "TransitionJoin") {
      return TransitionJoinWidgetFactory({ node: model as TransitionJoin });
    } else if (model.constructor.name === "TransitionSplit") {
      return TransitionSplitWidgetFactory({ node: model as TransitionSplit });
    } else if (model.constructor.name === "FreeJoint") {
      return FreeWidgetFactory({ node: model as FreeJoint });
    } else {
      return React.createElement(DefaultNodeWidget, {
        node: model as any,
        engine: this.engine,
      });
    }
  }

  getNewInstance(): null {
    return null;
  }
}
