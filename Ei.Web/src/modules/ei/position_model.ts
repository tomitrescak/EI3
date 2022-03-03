import { drag } from "../diagrams/model/diagram_common";
import { adjust, createPath } from "../diagrams/model/workflow/widget_action";
import { Ei } from "./ei_model";
import { Entity, EntityDao } from "./entity_model";
import { Workflow } from "./workflow_model";

export abstract class PositionModel extends Entity {
  ei: Ei;
  workflow: Workflow;

  ports = {};

  constructor(entity: Partial<EntityDao>, workflow: Workflow, ei: Ei) {
    super(entity);

    this.ei = ei;
    this.workflow = workflow;

    // add listeners
    // this.registerListener({
    //   selectionChanged: ({ isSelected }) => {
    //     isSelected ? this.select() : this.deselect();
    //   },
    // } as NodeModelListener);
  }

  handleDrag(evt, svgRef, history) {
    const node = this;
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
                  connection.ActionConnection == "LeftRight" ? "left" : "top"
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
  }

  // abstract select(): void;
}
