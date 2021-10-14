import React from "react";

import { PortWidget } from "@projectstorm/react-diagrams";
import { TransitionJoin } from "../../../ei/transition_model";
import { Port } from "./widget_transition_join";

export interface StateJoinNodeWidgetProps {
  node: TransitionJoin;
}

// export interface EntityNodeWidgetState {}

const height = 20;
const width = 80;

export const TransitionSplitWidget = ({ node }: StateJoinNodeWidgetProps) => {
  let labelSize = (node.Name || node.Id).length * 8 + 8;
  labelSize = labelSize < width ? width : labelSize;

  return (
    <div
      className="Entity-node"
      style={{
        position: "relative",
        width: labelSize,
        height,
      }}
    >
      <svg
        width={labelSize}
        height={height}
        transform={`rotate(${node.Horizontal ? 0 : 90} ${width / 2} ${
          height / 2
        })`}
      >
        <g id="Layer_1" />
        <g id="Layer_2">
          <rect
            fill={node.isSelected() ? "salmon" : "black"}
            width={labelSize}
            height={height}
            y={0}
            style={{ opacity: 1 }}
            rx={5}
            ry={5}
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
              fontWeight: node.isSelected() ? "bold" : "normal",
            }}
            textAnchor="middle"
            dominantBaseline="central"
          >
            {node.Name || node.Id}
          </text>
        </g>
      </svg>
      <PortWidget
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
      </PortWidget>
      <PortWidget
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
      </PortWidget>
      <PortWidget
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
      </PortWidget>
    </div>
  );
};

export let TransitionSplitWidgetFactory = React.createFactory(
  TransitionSplitWidget
);
