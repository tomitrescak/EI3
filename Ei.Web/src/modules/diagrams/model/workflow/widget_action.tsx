import { observer, Observer } from "mobx-react";
import React from "react";
import { ActionDisplayType, Connection } from "../../../ei/connection_model";
import { Entity } from "../../../ei/entity_model";
import { PointDao } from "../../../ei/hierarchic_entity_model";
import { Workflow } from "../../../ei/workflow_model";
import { drag } from "../diagram_common";

const maxLength = 200;
const size = 8;

type Port = {
  x: number;
  y: number;
  orientation: string;
};

function orientation(port: Port) {
  switch (port.orientation) {
    case "north":
      return { x: port.x, y: port.y - 50 };
    case "south":
      return { x: port.x, y: port.y + 50 };
    case "east":
      return { x: port.x + 50, y: port.y };
    case "west":
      return { x: port.x - 50, y: port.y };
    case "northEast":
      return { x: port.x + 30, y: port.y - 30 };
    case "northWest":
      return { x: port.x - 30, y: port.y - 30 };
    case "southEast":
      return { x: port.x + 30, y: port.y + 30 };
    case "southWest":
      return { x: port.x - 30, y: port.y + 30 };
  }
}

function adjust(point: PointDao | null, position: Port, offset?: PointDao) {
  if (point == null) {
    return position;
  }
  return {
    x: point.x + position.x + (offset ? offset.x : 0),
    y: point.y + position.y + (offset ? offset.y : 0),
    orientation: position.orientation,
  };
}

function offset(port: Port) {
  switch (port.orientation) {
    case "north":
      return { x: 0, y: -10 };
    case "south":
      return { x: 0, y: port.y + 50 };
    case "east":
      return { x: 10, y: 0 };
    case "west":
      return { x: -10, y: 0 };
    case "northEast":
      return { x: 5, y: -5 };
    case "northWest":
      return { x: -5, y: -5 };
    case "southEast":
      return { x: 5, y: 5 };
    case "southWest":
      return { x: -5, y: 5 };
  }
}

function createPath(
  positionFrom: PointDao | null,
  portFrom: Port,
  positionTo: PointDao | null,
  portTo: Port,
  withOffset = true
) {
  const from = adjust(positionFrom, portFrom);
  const to = adjust(positionTo, portTo, withOffset ? offset(portTo) : null);

  return `M ${from.x} ${from.y} C ${orientation(from).x} ${
    orientation(from).y
  }, ${orientation(to).x} ${orientation(to).y}, ${to.x} ${to.y}`;
}

function createPathObject(
  positionFrom: PointDao | null,
  portFrom: Port,
  positionTo: PointDao | null,
  portTo: Port
) {
  const from = adjust(positionFrom, portFrom);
  const to = adjust(positionTo, portTo);

  return {
    fromX: from.x,
    fromY: from.y,
    startHandleX: orientation(from).x,
    startHandleY: orientation(from).y,
    endHandleX: orientation(to).x,
    endHandleY: orientation(to).y,
    toX: to.x,
    toY: to.y,
  };
}

function createAnchors(connection: Connection, width: number, height: number) {
  return {
    from: connection.fromPosition
      ? createPathObject(
          connection.fromPosition.position,
          connection.fromPosition.ports[connection.SourcePort](),
          connection.ActionPosition,
          connection.ports[
            connection.ActionConnection == "LeftRight" ? "left" : "top"
          ](width, height)
        )
      : null,
    to: connection.toPosition
      ? createPathObject(
          connection.toPosition.position,
          connection.toPosition.ports[connection.TargetPort](),
          connection.ActionPosition,
          connection.ports[
            connection.ActionConnection == "LeftRight" ? "right" : "bottom"
          ](width, height)
        )
      : null,
  };
}

const CustomLinkArrowWidget = (props: {
  point: PointDao;
  previousPoint: PointDao;
  color: string;
}) => {
  const { point, previousPoint } = props;

  const angle =
    90 +
    (Math.atan2(point.y - previousPoint.y, point.x - previousPoint.x) * 180) /
      Math.PI;

  //translate(50, -10),
  return (
    <g
      className="arrow"
      transform={"translate(" + point.x + ", " + point.y + ")"}
    >
      <g style={{ transform: "rotate(" + angle + "deg)" }}>
        <g transform={"translate(0, -12)"}>
          <polygon points="0,10 8,30 -8,30" fill={props.color} />
        </g>
      </g>
    </g>
  );
};

const PreConditionLabels = ({
  link,
  width,
  height,
}: {
  link: Connection;
  width: number;
  height: number;
}) => {
  const connection = link;

  const position = connection.fromPosition
    ? adjust(
        connection.fromPosition.position,
        connection.fromPosition.ports[connection.SourcePort]()
      )
    : adjust(
        connection.ActionPosition,
        connection.ports[
          connection.ActionConnection == "LeftRight" ? "left" : "top"
        ](width, height)
      );

  // collect roles

  return (
    <Observer>
      {() => {
        let allowedRoles = [];
        for (let access of connection.Access) {
          if (access.Precondition) {
            allowedRoles.push(
              link.workflow.ei.Roles.find((r) => r.Id === access.Role).Icon
            );
          }
        }
        if (allowedRoles.length === 0) {
          return null;
        }

        return (
          <g>
            {allowedRoles.map((r, i) => (
              <text
                key={i}
                x={position.x}
                y={position.y}
                fill="white"
                textAnchor="middle"
                dominantBaseline="central"
                style={{ fontSize: "20px" }}
              >
                {r}
              </text>
            ))}
          </g>
        );
      }}
    </Observer>
  );
};

const PostConditionLabels = ({ link }: { link: Connection }) => {
  const connection = link;

  // collect roles

  return (
    <Observer>
      {() => {
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
          return null;
        }

        let points = link.getPoints().map((p) => new Point(p.getX(), p.getY()));
        let vector = points[points.length - 1].vector(
          points[points.length - 2]
        );
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
                dominantBaseline="central"
                style={{ fontSize: "20px" }}
              >
                {r}
              </text>
            ))}
          </g>
        );
      }}
    </Observer>
  );
};

const ActionRect = observer(
  ({
    children,
    angle,
    x,
    y,
    width,
    height,
    connection,
    svgRef,
  }: React.PropsWithChildren<{
    angle: number;
    x: number;
    y: number;
    width: number;
    height: number;
    connection: Connection;
    svgRef: React.MutableRefObject<SVGSVGElement>;
  }>) => {
    const fromRef = React.useRef<SVGPathElement>(null);
    const toRef = React.useRef<SVGPathElement>(null);

    const firstArrowPosition = connection.toPosition
      ? adjust(
          connection.ActionPosition,
          connection.ports[
            connection.ActionConnection == "LeftRight" ? "left" : "top"
          ](width, height)
        )
      : null;

    const secondArrowPosition = connection.toPosition
      ? adjust(
          connection.toPosition.position,
          connection.toPosition.ports[connection.TargetPort]()
        )
      : null;

    // const partsFrom = createPathObject(
    //   connection.fromPosition,
    //   connection.fromPosition.ports[connection.SourcePort](),
    //   null,
    //   connection.ports[
    //     connection.ActionConnection == "LeftRight" ? "left" : "top"
    //   ](width, height)
    // );
    // const partsTo = createPathObject(
    //   connection.toPosition,
    //   connection.toPosition.ports[connection.TargetPort](),
    //   null,
    //   connection.ports[
    //     connection.ActionConnection == "LeftRight" ? "right" : "bottom"
    //   ](width, height)
    // );

    return (
      <>
        <svg
          width={width}
          height={height}
          transform={`rotate(${angle} ${x} ${y})`}
          cursor="pointer"
          x={x}
          y={y}
          onMouseDown={(evt) =>
            drag(
              svgRef.current,
              evt,
              (p) => {
                connection.ActionPosition = p;
              },
              (p) => {
                if (fromRef.current) {
                  fromRef.current.setAttribute(
                    "d",
                    createPath(
                      connection.fromPosition.position,
                      connection.fromPosition.ports[connection.SourcePort](),
                      p,
                      connection.ports[
                        connection.ActionConnection == "LeftRight"
                          ? "left"
                          : "top"
                      ](width, height),
                      false
                    )
                  );
                }
                if (toRef.current) {
                  toRef.current.setAttribute(
                    "d",
                    createPath(
                      p,
                      connection.ports[
                        connection.ActionConnection == "LeftRight"
                          ? "right"
                          : "bottom"
                      ](width, height),
                      connection.toPosition.position,
                      connection.toPosition.ports[connection.TargetPort]()
                    )
                  );
                }
              }
            )
          }
          // onClick={() => {
          //   // ents.forEach((e) => (e.selected = false));
          //   // e.selected = true;
          //   history.push(e.url);
          // }}
        >
          <rect fill="grey" rx={size} ry={size} width={width} height={height} />
          {children}
        </svg>
        {connection.From && (
          <>
            {/* <circle
              fill="gold"
              cx={partsFrom.fromX}
              cy={partsFrom.fromY}
              r={6}
            />
            <circle
              fill="blue"
              cx={partsFrom.startHandleX}
              cy={partsFrom.startHandleY}
              r={6}
            />

            <circle fill="red" cx={partsFrom.toX} cy={partsFrom.toY} r={6} />
            <circle
              fill="orange"
              cx={partsFrom.endHandleX}
              cy={partsFrom.endHandleY}
              r={6}
            /> */}

            <path
              ref={fromRef}
              stroke="silver"
              strokeWidth={3}
              fill="transparent"
              d={createPath(
                connection.fromPosition.position,
                connection.fromPosition.ports[connection.SourcePort](),
                connection.ActionPosition,
                connection.ports[
                  connection.ActionConnection == "LeftRight" ? "left" : "top"
                ](width, height),
                false
              )}
            />

            {/* <CustomLinkArrowWidget
              point={firstArrowPosition}
              previousPoint={orientation(firstArrowPosition)}
              color="silver"
            /> */}
          </>
        )}

        {connection.toPosition && (
          <>
            {/* <circle fill="gold" cx={partsTo.fromX} cy={partsTo.fromY} r={6} />
            <circle
              fill="blue"
              cx={partsTo.startHandleX}
              cy={partsTo.startHandleY}
              r={6}
            />

            <circle fill="red" cx={partsTo.toX} cy={partsTo.toY} r={6} />
            <circle
              fill="orange"
              cx={partsTo.endHandleX}
              cy={partsTo.endHandleY}
              r={6}
            /> */}

            <path
              ref={toRef}
              stroke="silver"
              strokeWidth={3}
              fill="transparent"
              d={createPath(
                connection.ActionPosition,
                connection.ports[
                  connection.ActionConnection == "LeftRight"
                    ? "right"
                    : "bottom"
                ](width, height),
                connection.toPosition.position,
                connection.toPosition.ports[connection.TargetPort]()
              )}
            />
            <CustomLinkArrowWidget
              point={secondArrowPosition}
              previousPoint={orientation(secondArrowPosition)}
              color="silver"
            />
          </>
        )}
      </>
    );
  }
);

//

export const ActionView = ({
  link,
  workflow,
  svgRef,
}: {
  link: Connection;
  workflow: Workflow;
  svgRef: React.MutableRefObject<SVGSVGElement>;
}) => {
  return (
    <Observer>
      {() => {
        let action = link.ActionId
          ? workflow.Actions.find((a) => a.Id === link.ActionId)
          : null;
        if (!action) {
          return (
            <ActionRect
              x={link.ActionPosition.x}
              y={link.ActionPosition.y}
              width={100}
              height={35}
              angle={0}
              connection={link}
              svgRef={svgRef}
            >
              <text
                x={20}
                y={20}
                fill="white"
                style={{
                  fontFamily: "Verdana",
                  fontSize: "12px",
                  textAlign: "left",
                }}
              >
                No Action
              </text>
            </ActionRect>
          );
        }

        // const result = points[0].midPoint(points);
        const position = link.ActionPosition;
        // const vector = result.vector;
        const name =
          action.Icon + "\u00A0\u00A0\u00A0\u00A0" + (action.Name || action.Id);
        const labelSize = (name.length - 3) * 8 + 10;

        let fromPosition = link.fromPosition?.position;
        let toPosition = link.toPosition?.position;

        let angle = 0;
        if (link.RotateLabel && fromPosition && toPosition) {
          let vector = {
            x: toPosition.x - fromPosition.x,
            y: toPosition.y - fromPosition.y,
          };
          angle = Math.atan(vector.y / vector.x) * (180 / Math.PI);
        }

        if (link.ActionDisplay === ActionDisplayType.Full) {
          // create precondition and postcondition text
          let texts = [name];

          // add preconditions
          for (let access of link.Access) {
            if (access.Precondition) {
              let preconditionTexts: string[] = [];
              let role = link.workflow.ei.Roles.find(
                (r) => r.Id === access.Role
              );
              if (role) {
                let parts = access.Precondition.split("\n");
                for (let i = 0; i < parts.length; i++) {
                  preconditionTexts.push(
                    (i === 0
                      ? `❓ ${role.Icon} `
                      : "\u00A0\u00A0\u00A0\u00A0") +
                      parts[i].substring(0, maxLength)
                  );
                }
                // texts.push(`❓ ${role.Icon} ${access.Precondition.substring(0, maxLength)}`);
              } else {
                link.workflow.ei.context.warn(
                  `Role in precondition does not exist: ` + role
                );
              }
              texts.push(...preconditionTexts);
            }
          }

          // add postconditions
          for (let access of link.Access) {
            if (access.Postconditions.length) {
              let role = link.workflow.ei.Roles.find(
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
                link.workflow.ei.context.warn(
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
            <ActionRect
              x={link.ActionPosition.x}
              y={link.ActionPosition.y}
              width={longest}
              height={height}
              angle={angle}
              connection={link}
              svgRef={svgRef}
            >
              <text
                x={0}
                y={0}
                fill="white"
                style={{
                  fontFamily: "Verdana",
                  fontSize: "12px",
                }}
              >
                {texts.map((r, i) => (
                  <tspan
                    style={{ fontWeight: i === 0 ? "bold" : "normal" }}
                    key={i}
                    x={8}
                    dy="18px"
                  >
                    {r}
                  </tspan>
                ))}
              </text>
              {link.AllowLoops && (
                <>
                  <circle
                    cx={position.x - longest / 2}
                    cy={position.y - 22}
                    r="10"
                    fill="silver"
                  />
                  <text x={position.x - longest / 2 - 4} y={position.y - 18}>
                    {link.AllowLoops}
                  </text>
                </>
              )}
            </ActionRect>
          );
        }

        return (
          <ActionRect
            x={link.ActionPosition.x}
            y={link.ActionPosition.y}
            width={labelSize + 40}
            height={30}
            angle={angle}
            connection={link}
            svgRef={svgRef}
          >
            <text
              x={(labelSize + 40) / 2}
              y={18}
              width={labelSize}
              fill="white"
              style={{
                fontFamily: "Verdana",
                fontSize: "14px",
              }}
              textAnchor="middle"
              // dominantBaseline="central"
            >
              {name}
            </text>
            {link.AllowLoops && (
              <>
                <circle
                  cx={position.x - labelSize / 2}
                  cy={position.y - 16}
                  r="10"
                  fill="silver"
                />
                <text x={position.x - labelSize / 2 - 4} y={position.y - 12}>
                  {link.AllowLoops}
                </text>
              </>
            )}
          </ActionRect>
        );
      }}
    </Observer>
  );
};
