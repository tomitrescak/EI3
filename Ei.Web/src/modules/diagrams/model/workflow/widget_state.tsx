import React from "react";

import { observer } from "mobx-react";
import { State } from "../../../ei/state_model";
import styled from "@emotion/styled";
import { useHistory, useLocation } from "react-router-dom";
import { action } from "mobx";
import { drag } from "../diagram_common";
import { adjust, createPath } from "./widget_action";

export interface StateNodeWidgetProps {
  node: State;
  svgRef: React.MutableRefObject<SVGSVGElement>;
}

// export interface EntityNodeWidgetState {}

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

export const StateWidget = observer(
  ({ node, svgRef }: StateNodeWidgetProps) => {
    // const history = useHistory();

    let stroke = node.IsEnd ? 8 : 2;
    let { currentSize, width, height, text, labelSize } = node;

    let labelX = node.labelSize < currentSize * 2 ? (width - labelSize) / 2 : 0;

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
        cursor="pointer"
        onMouseDown={(evt) => {
          evt.preventDefault();

          drag(
            svgRef.current,
            evt,
            (p) => {
              node.position = p;
            },
            (p) => {
              let fromStateConnections = node.workflow.Connections.filter(
                (c) => c.From === node.Id
              );
              for (let connection of fromStateConnections) {
                if (connection.fromRef.current) {
                  connection.fromRef.current.setAttribute(
                    "d",
                    createPath(
                      p,
                      node.ports[connection.SourcePort](),
                      connection.ActionPosition,
                      connection.ports[
                        connection.ActionConnection == "LeftRight"
                          ? "left"
                          : "top"
                      ](connection.actionWidth, connection.actionHeight),
                      false
                    )
                  );
                }
              }
              let toStateConnections = node.workflow.Connections.filter(
                (c) => c.To === node.Id
              );

              for (let connection of toStateConnections) {
                const point = adjust(p, node.ports[connection.TargetPort]());

                if (connection.arrowRef?.current) {
                  connection.arrowRef.current.setAttribute(
                    "transform",
                    `translate(${point.x},${point.y})`
                  );
                }
                if (connection.toRef.current) {
                  connection.toRef.current.setAttribute(
                    "d",
                    createPath(
                      connection.ActionPosition,
                      connection.ports[
                        connection.ActionConnection == "LeftRight"
                          ? "right"
                          : "bottom"
                      ](connection.actionWidth, connection.actionHeight),
                      p,
                      node.ports[connection.TargetPort]()
                    )
                  );
                }
              }
            },
            () => history.push(node.url)
          );
        }}
      >
        {/* <circle
        fill="gold"
        cx={node.ports.east().x}
        cy={node.ports.east().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.west().x}
        cy={node.ports.west().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.north().x}
        cy={node.ports.north().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.south().x}
        cy={node.ports.south().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.southEast().x}
        cy={node.ports.southEast().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.southWest().x}
        cy={node.ports.southWest().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.northEast().x}
        cy={node.ports.northEast().y}
        r={6}
      />
      <circle
        fill="gold"
        cx={node.ports.northWest().x}
        cy={node.ports.northWest().y}
        r={6}
      /> */}

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
  }
);

export let StateWidgetFactory = React.createFactory(StateWidget);
