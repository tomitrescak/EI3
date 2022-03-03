import { Entity } from "../../ei/entity_model";
import { HierarchicEntity } from "../../ei/hierarchic_entity_model";

export type Point = {
  x: number;
  y: number;
};

export function drag(
  svg: SVGSVGElement,
  evt: React.MouseEvent<SVGGraphicsElement>,
  commit: (p: Point) => void,
  update?: (p: Point) => void
) {
  evt.preventDefault();

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

    if (update) {
      update(p);
    }
  }

  function stopDrag() {
    document.removeEventListener("mousemove", move as any);
    document.removeEventListener("mouseup", stopDrag);

    if (commit) {
      commit(p);
    }
  }

  document.addEventListener("mousemove", move as any);
  document.addEventListener("mouseup", stopDrag);
}
