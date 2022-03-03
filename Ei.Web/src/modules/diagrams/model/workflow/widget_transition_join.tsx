import React from "react";

import {
  transitionHeight,
  TransitionJoin,
  transitionWidth,
} from "../../../ei/transition_model";
import styled from "@emotion/styled";
import { observer } from "mobx-react";
import { useHistory, useLocation } from "react-router-dom";

export interface StateJoinNodeWidgetProps {
  node: TransitionJoin;
  svgRef: React.MutableRefObject<SVGSVGElement>;
}

// export interface EntityNodeWidgetState {}

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

export const TransitionJoinWidget = observer(
  ({ node, svgRef }: StateJoinNodeWidgetProps) => {
    let history = useHistory();
    let location = useLocation();
    let labelSize = (node.Name || node.Id).length * 8 + 8;
    labelSize = labelSize < transitionWidth ? transitionWidth : labelSize;

    const selected = node.url === location.pathname + location.search;

    return (
      <svg
        width={labelSize}
        height={transitionHeight}
        x={node.position.x}
        y={node.position.y}
        cursor="pointer"
        onMouseDown={(evt) => {
          node.handleDrag(evt, svgRef, history);
        }}
      >
        {/* <circle
          fill="gold"
          cx={node.ports.join1().x}
          cy={node.ports.join1().y}
          r={6}
        />
        <circle
          fill="gold"
          cx={node.ports.join2().x}
          cy={node.ports.join2().y}
          r={6}
        />
        <circle
          fill="gold"
          cx={node.ports.join3().x}
          cy={node.ports.join3().y}
          r={6}
        />
        <circle
          fill="gold"
          cx={node.ports.yield().x}
          cy={node.ports.yield().y}
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
    );
  }
);

export let TransitionJoinWidgetFactory =
  React.createFactory(TransitionJoinWidget);

{
  /* <PortWidget
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
      </PortWidget> */
}
