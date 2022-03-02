import React from "react";
import { observer } from "mobx-react";
import type { Workflow } from "../ei/workflow_model";
import { useAppContext } from "../../config/context";
import { DiagramView } from "../diagrams/diagram_view";
import { StateWidget } from "./model/workflow/widget_state";
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

  return (
    <DiagramView>
      <svg
        ref={svgRef}
        xmlns="http://www.w3.org/2000/svg"
        viewBox={`0 0 800 800`}
        width="100%"
        height="100%"
      >
        {workflow.States.map((s) => (
          <StateWidget key={s.Id} node={s} />
        ))}
        {workflow.Connections.map((c) => {})}
      </svg>
    </DiagramView>
  );
});
