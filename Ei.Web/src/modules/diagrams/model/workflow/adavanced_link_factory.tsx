import { observer, Observer } from "mobx-react";

import { ActionDisplayType, Connection } from "../../../ei/connection_model";

// class Point {
//   x: number;
//   y: number;

//   constructor(x: number, y: number) {
//     this.x = x;
//     this.y = y;
//   }

//   vector(b: Point) {
//     return new Point(b.x - this.x, b.y - this.y);
//   }

//   normalised() {
//     let distance = this.distance();
//     return new Point(this.x / distance, this.y / distance);
//   }

//   distance() {
//     return Math.sqrt(this.x * this.x + this.y * this.y);
//   }

//   polyDistance(points: Point[]) {
//     let distance = 0;
//     for (let i = 0; i < points.length - 1; i++) {
//       distance += points[i].vector(points[i + 1]).distance();
//     }
//     return distance;
//   }

//   midPoint(points: Point[]) {
//     let mid = this.polyDistance(points) * 0.5;

//     let totalDistance = 0;
//     for (let i = 0; i < points.length - 1; i++) {
//       const distance = points[i].vector(points[i + 1]).distance();
//       if (totalDistance + distance < mid) {
//         totalDistance += distance;
//       } else {
//         const remaining = mid - totalDistance;
//         const proportional = remaining / distance;
//         const vector = points[i].vector(points[i + 1]);
//         return {
//           mid: points[i].add(vector.multiply(proportional)),
//           vector,
//         };
//       }
//     }
//   }

//   add(b: Point) {
//     return new Point(this.x + b.x, this.y + b.y);
//   }

//   multiply(scalar: number) {
//     return new Point(this.x * scalar, this.y * scalar);
//   }
// }

// const CustomLinkArrowWidget = (props: {
//   point: any;
//   previousPoint: any;
//   color: string;
//   colorSelected: string;
// }) => {
//   const { point, previousPoint } = props;

//   const angle =
//     90 +
//     (Math.atan2(
//       point.getPosition().y - previousPoint.getPosition().y,
//       point.getPosition().x - previousPoint.getPosition().x
//     ) *
//       180) /
//       Math.PI;

//   //translate(50, -10),
//   return (
//     <g
//       className="arrow"
//       transform={
//         "translate(" +
//         point.getPosition().x +
//         ", " +
//         point.getPosition().y +
//         ")"
//       }
//     >
//       <g style={{ transform: "rotate(" + angle + "deg)" }}>
//         <g transform={"translate(0, -12)"}>
//           <polygon
//             points="0,10 8,30 -8,30"
//             fill={props.color}
//             data-id={point.getID()}
//             data-linkid={point.getLink().getID()}
//           />
//         </g>
//       </g>
//     </g>
//   );
// };

// const PreConditionLabels = ({ link }: { link: Connection }) => {
//   const connection = link;

//   // collect roles

//   return (
//     <Observer>
//       {() => {
//         let allowedRoles = [];
//         for (let access of connection.Access) {
//           if (access.Precondition) {
//             allowedRoles.push(
//               link.workflow.ei.Roles.find((r) => r.Id === access.Role).Icon
//             );
//           }
//         }
//         if (allowedRoles.length === 0) {
//           return null;
//         }

//         let points = link.getPoints().map((p) => new Point(p.getX(), p.getY()));
//         let vector = points[0].vector(points[1]);
//         let position = vector.normalised().multiply(30).add(points[0]);
//         let angle = 0;
//         // if (connection.RotateLabel) {
//         //   angle = Math.atan(vector.y / vector.x) * (180 / Math.PI);
//         // }

//         return (
//           <g
//             id="Layer_3"
//             transform={`rotate(${angle} ${position.x} ${position.y})`}
//           >
//             {allowedRoles.map((r, i) => (
//               <text
//                 key={i}
//                 x={position.x + i * 22}
//                 y={position.y + 12}
//                 fill="white"
//                 textAnchor="middle"
//                 dominantBaseline="central"
//                 style={{ fontSize: "20px" }}
//               >
//                 {r}
//               </text>
//             ))}
//           </g>
//         );
//       }}
//     </Observer>
//   );
// };

// const PostConditionLabels = ({ link }: { link: Connection }) => {
//   const connection = link;

//   // collect roles

//   return (
//     <Observer>
//       {() => {
//         let allowedRoles = [];
//         for (let access of connection.Access) {
//           if (access.Postconditions.length) {
//             let role = link.workflow.ei.Roles.find((r) => r.Id === access.Role);
//             if (role) {
//               allowedRoles.push(role.Icon);
//             } else {
//               link.workflow.ei.context.warn(
//                 `Role '${access.Role}' does not exist in precondition for link '${link.connection.Id}'`
//               );
//             }
//           }
//         }
//         if (allowedRoles.length === 0) {
//           return null;
//         }

//         let points = link.getPoints().map((p) => new Point(p.getX(), p.getY()));
//         let vector = points[points.length - 1].vector(
//           points[points.length - 2]
//         );
//         let position = vector
//           .normalised()
//           .multiply(30)
//           .add(points[points.length - 1]);
//         let angle = 0;
//         // if (link.connection.RotateLabel) {
//         //   angle = this.lastAngle(points);
//         // }
//         return (
//           <g
//             id="Layer_3"
//             transform={`rotate(${angle} ${position.x} ${position.y})`}
//           >
//             {allowedRoles.map((r, i) => (
//               <text
//                 key={i}
//                 x={position.x + i * 22}
//                 y={position.y + 12}
//                 fill="white"
//                 textAnchor="middle"
//                 dominantBaseline="central"
//                 style={{ fontSize: "20px" }}
//               >
//                 {r}
//               </text>
//             ))}
//           </g>
//         );
//       }}
//     </Observer>
//   );
// };

// const maxLength = 200;
// const size = 5;

// const ActionView = ({ link }: { link: Connection }) => {
//   return (
//     <Observer>
//       {() => {
//         const action = link.action;
//         if (!action) {
//           return null;
//         }
//         let points = link.getPoints().map((p) => new Point(p.getX(), p.getY()));
//         const connection = link.connection;

//         const result = points[0].midPoint(points);
//         const position = result.mid;
//         const vector = result.vector;
//         const name =
//           connection.ActionDisplay === ActionDisplayType.IconOnly
//             ? action.Icon
//             : action.Icon +
//               "\u00A0\u00A0\u00A0\u00A0" +
//               (action.Name || action.Id);
//         const labelSize = (name.length - 3) * 8 + 10;

//         let angle = 0;
//         if (link.connection.RotateLabel) {
//           angle = Math.atan(vector.y / vector.x) * (180 / Math.PI);
//         }

//         if (connection.ActionDisplay === ActionDisplayType.Full) {
//           // create precondition and postcondition text
//           let texts = [name];

//           // add preconditions
//           for (let access of connection.Access) {
//             if (access.Precondition) {
//               let preconditionTexts: string[] = [];
//               let role = connection.workflow.ei.Roles.find(
//                 (r) => r.Id === access.Role
//               );
//               if (role) {
//                 let parts = access.Precondition.split("\n");
//                 for (let i = 0; i < parts.length; i++) {
//                   preconditionTexts.push(
//                     (i === 0
//                       ? `❓ ${role.Icon} `
//                       : "\u00A0\u00A0\u00A0\u00A0") +
//                       parts[i].substring(0, maxLength)
//                   );
//                 }
//                 // texts.push(`❓ ${role.Icon} ${access.Precondition.substring(0, maxLength)}`);
//               } else {
//                 connection.workflow.ei.context.warn(
//                   `Role in precondition does not exist: ` + role
//                 );
//               }
//               texts.push(...preconditionTexts);
//             }
//           }

//           // add postconditions
//           for (let access of connection.Access) {
//             if (access.Postconditions.length) {
//               let role = connection.workflow.ei.Roles.find(
//                 (r) => r.Id === access.Role
//               );
//               if (role) {
//                 for (let pc of access.Postconditions) {
//                   let pcText = [];
//                   if (pc.Condition) {
//                     let parts = pc.Condition.split("\n");
//                     for (let i = 0; i < parts.length; i++) {
//                       pcText.push(
//                         (i === 0
//                           ? `⚡️${role.Icon} ❓\u00A0`
//                           : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0") +
//                           parts[i].substring(0, maxLength)
//                       );
//                     }
//                   }
//                   if (pc.Action) {
//                     let parts = pc.Action.split("\n");
//                     for (let i = 0; i < parts.length; i++) {
//                       pcText.push(
//                         (i === 0
//                           ? pcText.length === 0
//                             ? `⚡️${role.Icon}❗️\u00A0`
//                             : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0❗️\u00A0"
//                           : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0") +
//                           parts[i].substring(0, maxLength)
//                       );
//                     }
//                     texts.push(...pcText);
//                   }

//                   // texts.push(
//                   //   `⚡️ ${role.Icon}\u00A0\u00A0 ${
//                   //     pc.Condition ? pc.Condition.substring(0, maxLength / 2) + ' ?: ' : ''
//                   //   } ${pc.Action.substring(0, maxLength / 2)}`
//                   // );
//                 }
//               } else {
//                 connection.workflow.ei.context.warn(
//                   `Role in precondition does not exist: ` + role
//                 );
//               }
//             }
//           }

//           let longest =
//             texts.reduce(
//               (prev, next) => (prev = prev < next.length ? next.length : prev),
//               0
//             ) *
//               6 +
//             10;
//           let height = texts.length * 19 + 4;

//           longest = longest < labelSize ? labelSize : longest;

//           return (
//             <>
//               <g id="Layer_2">
//                 <rect
//                   fill="grey"
//                   rx={size}
//                   ry={size}
//                   width={longest}
//                   height={height}
//                   transform={`rotate(${angle} ${position.x} ${position.y})`}
//                   x={position.x - longest / 2}
//                   y={position.y - height / 2}
//                 />

//                 <text
//                   x={0}
//                   y={position.y - height / 2}
//                   fill="white"
//                   style={{
//                     fontFamily: "Verdana",
//                     fontSize: "12px",
//                     textAlign: "left",
//                   }}
//                   transform={`rotate(${angle} ${position.x} ${position.y})`}
//                 >
//                   {texts.map((r, i) => (
//                     <tspan
//                       style={{ fontWeight: i === 0 ? "bold" : "normal" }}
//                       key={i}
//                       x={position.x - longest / 2 + 5}
//                       dy="18px"
//                     >
//                       {r}
//                     </tspan>
//                   ))}
//                 </text>
//                 {link.connection.AllowLoops && (
//                   <>
//                     <circle
//                       cx={position.x - longest / 2}
//                       cy={position.y - 22}
//                       r="10"
//                       fill="silver"
//                     />
//                     <text x={position.x - longest / 2 - 4} y={position.y - 18}>
//                       {link.connection.AllowLoops}
//                     </text>
//                   </>
//                 )}
//               </g>
//             </>
//           );
//         }

//         return (
//           <>
//             <g id="Layer_2">
//               {connection.ActionDisplay === ActionDisplayType.IconAndText && (
//                 <rect
//                   fill="grey"
//                   rx={size}
//                   ry={size}
//                   width={labelSize}
//                   height={20}
//                   transform={`rotate(${angle} ${position.x} ${position.y})`}
//                   x={position.x - labelSize / 2}
//                   y={position.y - 15}
//                 />
//               )}
//               <text
//                 x={position.x}
//                 y={position.y - 5}
//                 fill="white"
//                 textAnchor="middle"
//                 dominantBaseline="central"
//                 transform={`rotate(${angle} ${position.x} ${position.y})`}
//               >
//                 {name}
//               </text>
//               {link.connection.AllowLoops && (
//                 <>
//                   <circle
//                     cx={position.x - labelSize / 2}
//                     cy={position.y - 16}
//                     r="10"
//                     fill="silver"
//                   />
//                   <text x={position.x - labelSize / 2 - 4} y={position.y - 12}>
//                     {link.connection.AllowLoops}
//                   </text>
//                 </>
//               )}
//             </g>
//           </>
//         );
//       }}
//     </Observer>
//   );
// };

// export const AdvancedLinkWidget = (props: any) => {
//   // const location = useLocation();
//   const link = props.link as Connection;
//   const selected = link.url === window.location.pathname;

//   return <AdvancedLinkWidget1 {...props} selected={selected} />;
// };

// export class AdvancedLinkWidget1 {
//   generateArrow(point: PointModel, previousPoint: PointModel): JSX.Element {
//     return (
//       <CustomLinkArrowWidget
//         key={point.getID()}
//         point={point}
//         previousPoint={previousPoint}
//         colorSelected={this.props.link.getOptions().selectedColor}
//         color={this.props.link.getOptions().color}
//       />
//     );
//   }

//   render() {
//     // const history = useLocation();

//     const link = this.props.link as Connection;

//     //ensure id is present for all points on the path
//     var points = this.props.link.getPoints();
//     var paths = [];
//     this.refPaths = [];

//     //draw the multiple anchors and complex line instead
//     for (let j = 0; j < points.length - 1; j++) {
//       paths.push(
//         this.generateLink(
//           LinkWidget.generateLinePath(points[j], points[j + 1]),
//           {
//             "data-linkid": this.props.link.getID(),
//             "data-point": j,
//             onMouseDown: (event: MouseEvent) => {
//               if (event.altKey) {
//                 this.addPointToLink(event as any, j + 1);
//               }
//             },
//           },
//           j
//         )
//       );
//     }

//     //render the circles
//     for (let i = 1; i < points.length - 1; i++) {
//       paths.push(this.generatePoint(points[i]));
//     }

//     if (this.props.link.getTargetPort() !== null) {
//       paths.push(
//         this.generateArrow(points[points.length - 1], points[points.length - 2])
//       );
//     } else {
//       paths.push(this.generatePoint(points[points.length - 1]));
//     }

//     return (
//       <>
//         <g
//           data-default-link-test={this.props.link.getOptions().testName}
//           className={this.props.selected ? "linkSelected" : ""}
//         >
//           {paths}
//         </g>

//         <ActionView link={link} />
//         {link.connection.ActionDisplay !== ActionDisplayType.Full && (
//           <PreConditionLabels link={link} />
//         )}

//         {link.connection.ActionDisplay !== ActionDisplayType.Full && (
//           <PostConditionLabels link={link} />
//         )}
//       </>
//     );
//   }
// }
