import * as React from "react";

import { observer } from "mobx-react";
import { DefaultLinkWidget, PointModel } from "storm-react-diagrams";
import { Ui } from "../../../../helpers/client_helpers";
import { ActionDisplayType } from "../../../ei/connection_model";
import { WorkflowLinkModel } from "./workflow_link_model";

const maxLength = 200;

class Point {
  x: number;
  y: number;

  constructor(x: number, y: number) {
    this.x = x;
    this.y = y;
  }

  vector(b: Point) {
    return new Point(b.x - this.x, b.y - this.y);
  }

  normalised() {
    let distance = this.distance();
    return new Point(this.x / distance, this.y / distance);
  }

  distance() {
    return Math.sqrt(this.x * this.x + this.y * this.y);
  }

  polyDistance(points: Point[]) {
    let distance = 0;
    for (let i = 0; i < points.length - 1; i++) {
      distance += points[i].vector(points[i + 1]).distance();
    }
    return distance;
  }

  midPoint(points: Point[]) {
    let mid = this.polyDistance(points) * 0.5;
    let totalDistance = 0;
    for (let i = 0; i < points.length - 1; i++) {
      const distance = points[i].vector(points[i + 1]).distance();
      if (totalDistance + distance < mid) {
        totalDistance += distance;
      } else {
        const remaining = mid - totalDistance;
        const proportional = remaining / distance;
        const vector = points[i].vector(points[i + 1]);
        return {
          mid: points[i].add(vector.multiply(proportional)),
          vector,
        };
      }
    }
  }

  add(b: Point) {
    return new Point(this.x + b.x, this.y + b.y);
  }

  multiply(scalar: number) {
    return new Point(this.x * scalar, this.y * scalar);
  }
}

const size = 5;

@observer
export class WorkflowLink extends DefaultLinkWidget {
  // generateLink(extraProps: any, id: string | number): JSX.Element {
  //   if (parseInt(id.toString(), 10) === this.props.link.points.length - 2) {
  //     extraProps.className = 'last'
  //   }
  //   return super.generateLink(extraProps, id);
  // }

  renderPreconditionRoleLabels(link: WorkflowLinkModel) {
    const connection = link.connection;

    // collect roles

    let allowedRoles = [];
    for (let access of connection.Access) {
      if (access.Precondition) {
        allowedRoles.push(
          link.workflow.ei.Roles.find((r) => r.Id === access.Role).Icon
        );
      }
    }
    if (allowedRoles.length === 0) {
      return false;
    }

    let points = link.points.map((p) => new Point(p.x, p.y));
    let vector = points[0].vector(points[1]);
    let position = vector.normalised().multiply(30).add(points[0]);
    let angle = 0;
    // if (connection.RotateLabel) {
    //   angle = Math.atan(vector.y / vector.x) * (180 / Math.PI);
    // }

    return (
      <g
        id="Layer_3"
        transform={`rotate(${angle} ${position.x} ${position.y})`}
      >
        {allowedRoles.map((r, i) => (
          <text
            key={i}
            x={position.x + i * 22}
            y={position.y + 12}
            fill="white"
            textAnchor="middle"
            alignmentBaseline="central"
            style={{ fontSize: "20px" }}
            onClick={() => {
              this.setState({ selected: true });
            }}
          >
            {r}
          </text>
        ))}
      </g>
    );
  }

  // override to add points with alt
  addPointToLink = (event: MouseEvent, index: number): void => {
    if (
      event.altKey &&
      !this.props.diagramEngine.isModelLocked(this.props.link) &&
      this.props.link.points.length - 1 <=
        this.props.diagramEngine.getMaxNumberPointsPerLink()
    ) {
      const point = new PointModel(
        this.props.link,
        this.props.diagramEngine.getRelativeMousePoint(event)
      );
      point.setSelected(true);
      this.forceUpdate();
      this.props.link.addPoint(point, index);
      this.props.pointAdded(point, event);
    }

    Ui.history.step();
  };

  lastAngle(points: Point[]) {
    let vector = points[points.length - 1].vector(points[points.length - 2]);
    return Math.atan(vector.y / vector.x) * (180 / Math.PI);
  }

  renderPostconditionRoleLabels(link: WorkflowLinkModel) {
    const connection = link.connection;

    // collect roles

    let allowedRoles = [];
    for (let access of connection.Access) {
      if (access.Postconditions.length) {
        let role = link.workflow.ei.Roles.find((r) => r.Id === access.Role);
        if (role) {
          allowedRoles.push(role.Icon);
        } else {
          link.workflow.ei.context.warn(
            `Role '${access.Role}' does not exist in precondition for link '${link.connection.Id}'`
          );
        }
      }
    }
    if (allowedRoles.length === 0) {
      return false;
    }

    let points = link.points.map((p) => new Point(p.x, p.y));
    let vector = points[points.length - 1].vector(points[points.length - 2]);
    let position = vector
      .normalised()
      .multiply(30)
      .add(points[points.length - 1]);
    let angle = 0;
    // if (link.connection.RotateLabel) {
    //   angle = this.lastAngle(points);
    // }

    return (
      <g
        id="Layer_3"
        transform={`rotate(${angle} ${position.x} ${position.y})`}
      >
        {allowedRoles.map((r, i) => (
          <text
            key={i}
            x={position.x + i * 22}
            y={position.y + 12}
            fill="white"
            textAnchor="middle"
            alignmentBaseline="central"
            style={{ fontSize: "20px" }}
            onClick={() => {
              this.setState({ selected: true });
            }}
          >
            {r}
          </text>
        ))}
      </g>
    );
  }

  select = () => (this.props.link as WorkflowLinkModel).select();

  renderArrow(link: WorkflowLinkModel) {
    let points = link.points.map((p) => new Point(p.x, p.y));
    let pLast = link.getLastPoint();
    let pPrevious = link.points[link.points.length - 2];

    if (pPrevious == null) {
      return false;
    }

    let flip = pPrevious.x > pLast.x;
    let path = flip
      ? `M ${pLast.x - 5} ${pLast.y} L ${pLast.x + 10} ${pLast.y + 5} L ${
          pLast.x + 10
        } ${pLast.y - 5} z`
      : `M ${pLast.x - 10} ${pLast.y + 5} L ${pLast.x + 5} ${pLast.y} L ${
          pLast.x - 10
        } ${pLast.y - 5} z`;
    let angle = this.lastAngle(points);

    // TODO: Adjust for bezier

    return (
      <path
        transform={`rotate(${angle} ${pLast.x} ${pLast.y})`}
        className="arrow"
        d={path}
        fill="black"
        stroke="black"
      />
    );
  }

  renderAction(link: WorkflowLinkModel) {
    const action = link.action;
    if (!action) {
      return false;
    }
    let points = link.points.map((p) => new Point(p.x, p.y));
    const connection = link.connection;

    const result = points[0].midPoint(points);
    const position = result.mid;
    const vector = result.vector;
    const name =
      connection.ActionDisplay === ActionDisplayType.IconOnly
        ? action.Icon
        : action.Icon + "\u00A0\u00A0\u00A0\u00A0" + (action.Name || action.Id);
    const labelSize = (name.length - 3) * 8 + 10;

    let angle = 0;
    if (link.connection.RotateLabel) {
      angle = Math.atan(vector.y / vector.x) * (180 / Math.PI);
    }

    if (connection.ActionDisplay === ActionDisplayType.Full) {
      // create precondition and postcondition text
      let texts = [name];

      // add preconditions
      for (let access of connection.Access) {
        if (access.Precondition) {
          let preconditionTexts: string[] = [];
          let role = connection.workflow.ei.Roles.find(
            (r) => r.Id === access.Role
          );
          if (role) {
            let parts = access.Precondition.split("\n");
            for (let i = 0; i < parts.length; i++) {
              preconditionTexts.push(
                (i === 0 ? `❓ ${role.Icon} ` : "\u00A0\u00A0\u00A0\u00A0") +
                  parts[i].substring(0, maxLength)
              );
            }
            // texts.push(`❓ ${role.Icon} ${access.Precondition.substring(0, maxLength)}`);
          } else {
            connection.workflow.ei.context.warn(
              `Role in precondition does not exist: ` + role
            );
          }
          texts.push(...preconditionTexts);
        }
      }

      // add postconditions
      for (let access of connection.Access) {
        if (access.Postconditions.length) {
          let role = connection.workflow.ei.Roles.find(
            (r) => r.Id === access.Role
          );
          if (role) {
            for (let pc of access.Postconditions) {
              let pcText = [];
              if (pc.Condition) {
                let parts = pc.Condition.split("\n");
                for (let i = 0; i < parts.length; i++) {
                  pcText.push(
                    (i === 0
                      ? `⚡️${role.Icon} ❓\u00A0`
                      : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0") +
                      parts[i].substring(0, maxLength)
                  );
                }
              }
              if (pc.Action) {
                let parts = pc.Action.split("\n");
                for (let i = 0; i < parts.length; i++) {
                  pcText.push(
                    (i === 0
                      ? pcText.length === 0
                        ? `⚡️${role.Icon}❗️\u00A0`
                        : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0❗️\u00A0"
                      : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0") +
                      parts[i].substring(0, maxLength)
                  );
                }
                texts.push(...pcText);
              }

              // texts.push(
              //   `⚡️ ${role.Icon}\u00A0\u00A0 ${
              //     pc.Condition ? pc.Condition.substring(0, maxLength / 2) + ' ?: ' : ''
              //   } ${pc.Action.substring(0, maxLength / 2)}`
              // );
            }
          } else {
            connection.workflow.ei.context.warn(
              `Role in precondition does not exist: ` + role
            );
          }
        }
      }

      let longest =
        texts.reduce(
          (prev, next) => (prev = prev < next.length ? next.length : prev),
          0
        ) *
          6 +
        10;
      let height = texts.length * 19 + 4;

      longest = longest < labelSize ? labelSize : longest;

      return (
        <>
          <g id="Layer_2">
            <rect
              fill="grey"
              rx={size}
              ry={size}
              width={longest}
              height={height}
              transform={`rotate(${angle} ${position.x} ${position.y})`}
              x={position.x - longest / 2}
              y={position.y - height / 2}
              onClick={() => {
                this.setState({ selected: true });
              }}
            />

            <text
              x={0}
              y={position.y - height / 2}
              fill="white"
              style={{
                fontFamily: "Verdana",
                fontSize: "12px",
                textAlign: "left",
              }}
              transform={`rotate(${angle} ${position.x} ${position.y})`}
              onClick={() => {
                this.setState({ selected: true });
              }}
            >
              {texts.map((r, i) => (
                <tspan
                  style={{ fontWeight: i === 0 ? "bold" : "normal" }}
                  key={i}
                  x={position.x - longest / 2 + 5}
                  dy="18px"
                >
                  {r}
                </tspan>
              ))}
            </text>
            {link.connection.AllowLoops && (
              <>
                <circle
                  cx={position.x - longest / 2}
                  cy={position.y - 22}
                  r="10"
                  fill="silver"
                />
                <text x={position.x - longest / 2 - 4} y={position.y - 18}>
                  {link.connection.AllowLoops}
                </text>
              </>
            )}
          </g>
        </>
      );
    }

    // simple view
    return (
      <>
        <g id="Layer_2">
          {connection.ActionDisplay === ActionDisplayType.IconAndText && (
            <rect
              fill="grey"
              rx={size}
              ry={size}
              width={labelSize}
              height={20}
              transform={`rotate(${angle} ${position.x} ${position.y})`}
              x={position.x - labelSize / 2}
              y={position.y - 15}
              onClick={this.select}
            />
          )}
          <text
            x={position.x}
            y={position.y - 5}
            fill="white"
            textAnchor="middle"
            alignmentBaseline="central"
            transform={`rotate(${angle} ${position.x} ${position.y})`}
            onClick={this.select}
          >
            {name}
          </text>
          {link.connection.AllowLoops && (
            <>
              <circle
                cx={position.x - labelSize / 2}
                cy={position.y - 16}
                r="10"
                fill="silver"
              />
              <text x={position.x - labelSize / 2 - 4} y={position.y - 12}>
                {link.connection.AllowLoops}
              </text>
            </>
          )}
        </g>
      </>
    );
  }

  render() {
    const link = this.props.link as WorkflowLinkModel;

    link.getLastPoint().id = "last";

    // subscribe
    link.selected;

    return (
      <>
        {super.render()}
        {this.renderAction(link)}
        {link.connection.ActionDisplay !== ActionDisplayType.Full &&
          this.renderPreconditionRoleLabels(link)}
        {link.connection.ActionDisplay !== ActionDisplayType.Full &&
          this.renderPostconditionRoleLabels(link)}
        {this.renderArrow(link)}
      </>
    );
  }
}
