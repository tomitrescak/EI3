import React from "react";

import { TransitionJoin } from "../../../ei/transition_model";
import styled from "@emotion/styled";

export interface StateJoinNodeWidgetProps {
  node: TransitionJoin;
}

// export interface EntityNodeWidgetState {}

const height = 20;
const width = 80;

export const Port = styled.div`
  width: 16px;
  height: 16px;
  z-index: 10;

  cursor: pointer;
  &:hover {
    background: rgba(0, 0, 0, 0.5);
    border-radius: 8px;
  }
`;

export const TransitionJoinWidget = ({ node }: StateJoinNodeWidgetProps) => {
  let labelSize = (node.Name || node.Id).length * 8 + 8;
  labelSize = labelSize < width ? width : labelSize;

  const selected =
    node.selected || node.url === location.pathname + location.search;

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
            fill={selected ? "salmon" : "black"}
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
              fontWeight: selected ? "bold" : "normal",
            }}
            textAnchor="middle"
            dominantBaseline="central"
          >
            {node.Name || node.Id}
          </text>
        </g>
      </svg>
      {/* <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? 5 : labelSize / 2 - 20,
          top: node.Horizontal ? -10 : -20,
        }}
        port={node.getPort("join1")}
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
        port={node.getPort("join2")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>
      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: node.Horizontal ? labelSize - 20 : labelSize / 2 - 20,
          top: node.Horizontal ? -10 : height + 5,
        }}
        port={node.getPort("join3")}
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
        port={node.getPort("yield")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget> */}
    </div>
  );
};

export let TransitionJoinWidgetFactory =
  React.createFactory(TransitionJoinWidget);
