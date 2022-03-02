import React from "react";

import { observer } from "mobx-react";
import { PortWidget } from "@projectstorm/react-diagrams";
import { State } from "../../../ei/state_model";
import styled from "@emotion/styled";
import { useHistory, useLocation } from "react-router-dom";
import { action } from "mobx";

export interface StateNodeWidgetProps {
  node: State;
}

// export interface EntityNodeWidgetState {}

const size = 15;

export const Port = styled.div`
  width: 12px;
  height: 12px;
  z-index: 10;

  &:hover {
    background: rgba(0, 0, 0, 0.5);
    border-radius: 4px;
  }
  cursor: pointer;
`;

export const StateWidget = observer(({ node }: StateNodeWidgetProps) => {
  // const history = useHistory();

  let currentSize = node.IsEnd ? size + 2 : size;
  let stroke = node.IsEnd ? 8 : 2;
  let text = (node.Timeout ? "⏱ " : "") + (node.Name || node.Id);
  let labelSize = text.length * 8 + 8;
  let width = labelSize < currentSize * 2 ? currentSize * 2 : labelSize;
  let height = size * 2;
  let labelX = labelSize < currentSize * 2 ? (width - labelSize) / 2 : 0;

  const location = useLocation();
  const history = useHistory();
  const selected = node.url === location.pathname + location.search;

  let rules = node.EntryRules.map((r) => {
    let role = node.workflow.ei.Roles.find((i) => i.Id === r.Role);
    if (!role) {
      return "+ <deleted> " + r.Role;
    }
    return " + " + role.Icon + " " + role.Name;
  }).concat(
    node.ExitRules.map((r) => {
      let role = node.workflow.ei.Roles.find((i) => i.Id === r.Role);
      if (!role) {
        return "- <deleted> " + r.Role;
      }
      return " - " + role.Icon + " " + role.Name;
    })
  );

  // find the longest name
  let longest =
    rules.reduce(
      (prev, next) => (prev = prev < next.length ? next.length : prev),
      0
    ) * 9;

  return (
    <svg
      width={width}
      height={height}
      x={node.position.x}
      y={node.position.y}
      onClick={action(() => {
        // ents.forEach((e) => (e.selected = false));
        // e.selected = true;
        history.push(node.url);
      })}
    >
      {node.ShowRules && rules.length && (
        <g id="Layer_1">
          <rect
            fill="white"
            stroke="black"
            strokeWidth="2"
            width={longest}
            height={rules.length * 17 + 4}
            x={-(longest - width) / 2}
            y={-rules.length * 20 - 10}
            style={{ minHeight: "40px" }}
            rx={5}
            ry={5}
          ></rect>

          <text
            x={0}
            y={-rules.length * 20 - 10}
            style={{
              fontFamily: "Verdana",
              fontSize: "12px",
              textAlign: "center",
              width: longest + "px",
            }}
            textAnchor="middle"
            // alignmentBaseline="central"
          >
            {rules.map((r, i) => (
              <tspan key={i} x="13" dy="16px">
                {r}
              </tspan>
            ))}
          </text>
        </g>
      )}
      <g id="Layer_2">
        <ellipse
          fill={node.IsStart ? "black" : selected ? "salmon" : "silver"}
          stroke={selected ? "salmon" : "black"}
          strokeWidth={stroke}
          strokeDasharray={node.IsOpen ? "3 3" : null}
          strokeMiterlimit="10"
          rx={currentSize}
          ry={currentSize}
          cx={width / 2}
          cy={height / 2}
        />
        <rect
          fill={node.IsStart ? "black" : "#e0e0e0"}
          width={labelSize}
          height={20}
          x={labelX}
          y={height / 2 - 10}
          style={{ opacity: 1 }}
          rx={5}
          ry={5}
        />
        <text
          x={labelSize / 2 + labelX}
          y={height / 2}
          style={{
            fontFamily: "Verdana",
            fontSize: "14px",
            fill: node.IsStart ? "white" : "black",
            textAlign: "center",
            width: "200px",
            // fontWeight: selected ? "bold" : "normal",
          }}
          textAnchor="middle"
          dominantBaseline="central"
        >
          {text}
        </text>
      </g>

      {/* <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: 3 * (width / 4), // simplified equation
          top: (3 * height) / 4 - 8,
        }}
        port={node.getPort("southeast")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: 3 * (width / 4), // simplified equation
          top: height / 4 - 8,
        }}
        port={node.getPort("northeast")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: width - 8,
          top: height / 2 - 8,
        }}
        port={node.getPort("east")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: width / 2 - 8,
          top: -4,
        }}
        port={node.getPort("north")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: (width + 12) / 4 - 16, // simplified equation
          top: height / 4 - 8,
        }}
        port={node.getPort("northwest")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: width / 2 - 8,
          top: height - 8,
        }}
        port={node.getPort("south")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: (width + 12) / 4 - 16, // simplified equation
          top: (3 * height) / 4 - 8,
        }}
        port={node.getPort("southwest")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>

      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: -8,
          top: height / 2 - 8,
        }}
        port={node.getPort("west")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget> */}
    </svg>
  );
});

export let StateWidgetFactory = React.createFactory(StateWidget);
