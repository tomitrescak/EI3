import React from "react";
import { observer } from "mobx-react";
import type { Workflow } from "../ei/workflow_model";
import { useAppContext } from "../../config/context";
import { DiagramView } from "../diagrams/diagram_view";
import { StateWidget } from "./model/workflow/widget_state";
import { TransitionJoin, TransitionSplit } from "../ei/transition_model";
import { TransitionJoinWidget } from "./model/workflow/widget_transition_join";
import { TransitionSplitWidget } from "./model/workflow/widget_transition_split";
import { ConnectionWidget } from "./model/workflow/widget_connection";
interface Props {
  workflowId: string;
}

export const WorkflowEditor = observer((props: Props) => {
  const svgRef = React.useRef<SVGSVGElement>(null);
  const context = useAppContext();
  const workflow = context.ei.Workflows.find((w) => w.Id === props.workflowId);
  // let ents = entities();

  // set offsets
  // const currentOffsetX = localStorage.getItem(
  //   `EntityDiagram.${props.type}.offsetX`
  // );
  // const currentOffsetY = localStorage.getItem(
  //   `EntityDiagram.${props.type}.offsetY`
  // );

  console.log(workflow.Connections);

  return (
    <DiagramView>
      <svg
        ref={svgRef}
        xmlns="http://www.w3.org/2000/svg"
        viewBox={`-400 0 800 800`}
        width="100%"
        height="100%"
      >
        {workflow.States.map((s) => (
          <StateWidget key={s.Id} node={s} />
        ))}
        {workflow.Transitions.filter((t) => t instanceof TransitionJoin).map(
          (p) => (
            <TransitionJoinWidget node={p as TransitionJoin} />
          )
        )}
        {workflow.Transitions.filter((t) => t instanceof TransitionSplit).map(
          (p) => (
            <TransitionSplitWidget node={p as TransitionSplit} />
          )
        )}
        {workflow.Connections.map((c) => (
          <ConnectionWidget
            svgRef={svgRef}
            key={c.Id}
            connection={c}
            workflow={workflow}
          />
        ))}
      </svg>
    </DiagramView>
  );
});
