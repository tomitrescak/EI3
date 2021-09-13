import * as React from "react";

import { inject, observer } from "mobx-react";
import { PortWidget } from "@projectstorm/react-diagrams";
import { State } from "../../../ei/state_model";

export interface StateNodeWidgetProps {
  context?: App.Context;
  node: State;
}

// export interface EntityNodeWidgetState {}

const size = 15;

@inject("context")
@observer
export class StateWidget extends React.Component<StateNodeWidgetProps> {
  public static defaultProps: StateNodeWidgetProps = {
    node: null,
  };

  constructor(props: StateNodeWidgetProps) {
    super(props);
    this.state = {};
  }

  render() {
    let { node } = this.props;
    let currentSize = node.IsEnd ? size + 2 : size;
    let stroke = node.IsEnd ? 8 : 2;
    let text = (node.Timeout ? "⏱ " : "") + (node.Name || node.Id);
    let labelSize = text.length * 8 + 8;
    let width = labelSize < currentSize * 2 ? currentSize * 2 : labelSize;
    let height = size * 2;
    let labelX = labelSize < currentSize * 2 ? (width - labelSize) / 2 : 0;

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
      ) * 7;

    return (
      <div
        className="Entity-node"
        style={{
          position: "relative",
          width: width,
          height: height,
        }}
      >
        <svg width={width} height={height}>
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
              />
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
                alignmentBaseline="central"
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
              fill={
                node.IsStart ? "black" : node.selected ? "salmon" : "silver"
              }
              stroke={node.selected ? "salmon" : "black"}
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
                fontWeight: node.selected ? "bold" : "normal",
              }}
              textAnchor="middle"
              alignmentBaseline="central"
            >
              {text}
            </text>
          </g>
        </svg>
        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: -13,
            top: height / 2 - 6,
          }}
        >
          <PortWidget name="west" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: 3 * (width / 4), // simplified equation
            top: (3 * height) / 4,
          }}
        >
          <PortWidget name="southeast" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: 3 * (width / 4), // simplified equation
            top: height / 4 - 13,
          }}
        >
          <PortWidget name="northeast" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: width - 4,
            top: height / 2 - 6,
          }}
        >
          <PortWidget name="east" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: width / 2 - 7,
            top: -13,
          }}
        >
          <PortWidget name="north" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: (width + 12) / 4 - 15, // simplified equation
            top: height / 4 - 13,
          }}
        >
          <PortWidget name="northwest" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: width / 2 - 7,
            top: height - 4,
          }}
        >
          <PortWidget name="south" node={this.props.node} />
        </div>

        <div
          style={{
            position: "absolute",
            zIndex: 10,
            left: (width + 12) / 4 - 15, // simplified equation
            top: (3 * height) / 4,
          }}
        >
          <PortWidget name="southwest" node={this.props.node} />
        </div>
      </div>
    );
  }
}

export let StateWidgetFactory = React.createFactory(StateWidget);
