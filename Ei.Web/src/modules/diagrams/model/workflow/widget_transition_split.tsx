import { observer } from "mobx-react";
import React from "react";
import { useHistory, useLocation } from "react-router-dom";

import {
  transitionHeight,
  TransitionSplit,
  transitionWidth,
} from "../../../ei/transition_model";
import { Port } from "./widget_transition_join";

export type StateJoinNodeWidgetProps = {
  node: TransitionSplit;
  svgRef: React.MutableRefObject<SVGSVGElement>;
};

// export interface EntityNodeWidgetState {}

const height = 20;
const width = 80;

export const TransitionSplitWidget = observer(
  ({ node, svgRef }: StateJoinNodeWidgetProps) => {
    let history = useHistory();
    let location = useLocation();
    let labelSize = (node.Name || node.Id).length * 8 + 8;
    labelSize = labelSize < width ? width : labelSize;

    const selected = node.url === location.pathname + location.search;

    return (
      <svg
        width={labelSize}
        height={height}
        x={node.position.x}
        y={node.position.y}
        cursor="pointer"
        onMouseDown={(evt) => {
          node.handleDrag(evt, svgRef, history);
        }}
      >
        {/* <circle
          fill="orange"
          cx={node.ports.split1().x}
          cy={node.ports.split1().y}
          r={6}
        />
        <circle
          fill="red"
          cx={node.ports.split2().x}
          cy={node.ports.split2().y}
          r={6}
        />
        <circle
          fill="green"
          cx={node.ports.split3().x}
          cy={node.ports.split3().y}
          r={6}
        />
        <circle
          fill="blue"
          cx={node.ports.input().x}
          cy={node.ports.input().y}
          r={6}
        /> */}
        <g
          style={{
            transformOrigin: "center",
          }}
          transform={`rotate(${node.Vertical ? -90 : 0})`}
        >
          <rect
            fill={selected ? "salmon" : "black"}
            width={labelSize}
            height={transitionHeight}
            style={{ opacity: 1 }}
          />
          <text
            x={labelSize / 2}
            y={9}
            style={{
              fontFamily: "Verdana",
              fontSize: "14px",
              fill: "white",
              textAlign: "center",
              width: "200px",
              // fontWeight: node.selected ? "bold" : "normal",
            }}
            textAnchor="middle"
            dominantBaseline="central"
          >
            {node.Name || node.Id}
          </text>
        </g>
      </svg>
    );
  }
);

export let TransitionSplitWidgetFactory = React.createFactory(
  TransitionSplitWidget
);

{
  /* <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? 5 : labelSize / 2 + 7,
          top: node.Horizontal ? height - 3 : -20,
        }}
        port={node.getPort("split1")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget> */
}
{
  /* <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? labelSize / 2 - 7 : labelSize / 2 + 7,
          top: node.Horizontal ? height - 3 : 3,
        }}
        port={node.getPort("split2")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget> */
}
{
  /* <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? labelSize - 20 : labelSize / 2 + 7,
          top: node.Horizontal ? height - 3 : height + 5,
        }}
        port={node.getPort("split3")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>
      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? labelSize / 2 - 7 : labelSize / 2 - 20,
          top: node.Horizontal ? -10 : 3,
        }}
        port={node.getPort("input")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget> */
}
