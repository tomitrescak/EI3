import { Entity } from "../../ei/entity_model";
import { HierarchicEntity } from "../../ei/hierarchic_entity_model";

export type Point = {
  x: number;
  y: number;
};

export function drag(
  svg: SVGSVGElement,
  entity: Entity,
  evt: React.MouseEvent<SVGGraphicsElement>,
  entities?: any[]
) {
  // evt.preventDefault();

  let xStart = parseFloat(evt.currentTarget.getAttribute("x"));
  let yStart = parseFloat(evt.currentTarget.getAttribute("y"));
  let xPos = evt.clientX;
  let yPos = evt.clientY;
  let target = evt.currentTarget;

  let m = svg.getScreenCTM();
  let p = svg.createSVGPoint();

  function move(e: React.MouseEvent<SVGElement>) {
    p.x = xStart + (e.clientX - xPos) / m.a;
    p.y = yStart + (e.clientY - yPos) / m.a;

    // p = p.matrixTransform(m.inverse());

    target.setAttribute("x", p.x.toString());
    target.setAttribute("y", p.y.toString());

    if (entity instanceof HierarchicEntity) {
      entity.updateConnection(entities, p);
    }
  }

  function stopDrag() {
    document.removeEventListener("mousemove", move as any);
    document.removeEventListener("mouseup", stopDrag);

    entity.position.x = parseFloat(target.getAttribute("x"));
    entity.position.y = parseFloat(target.getAttribute("y"));
  }

  document.addEventListener("mousemove", move as any);
  document.addEventListener("mouseup", stopDrag);
}
