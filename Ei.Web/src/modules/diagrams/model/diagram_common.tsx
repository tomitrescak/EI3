export type Point = {
  x: number;
  y: number;
};

export function drag(
  svg: SVGSVGElement,
  evt: React.MouseEvent<SVGGraphicsElement>,
  commit: (p: Point) => void,
  update?: (p: Point) => void,
  click?: () => void
) {
  evt.preventDefault();

  svg.setAttribute("data-mouse-down", "yes");

  let xStart = parseFloat(evt.currentTarget.getAttribute("x"));
  let yStart = parseFloat(evt.currentTarget.getAttribute("y"));
  let xPos = evt.clientX;
  let yPos = evt.clientY;
  let target = evt.currentTarget;

  let m = svg.getScreenCTM();
  let p = svg.createSVGPoint();
  let moved = false;

  function move(e: React.MouseEvent<SVGElement>) {
    moved = true;

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

    svg.setAttribute("data-mouse-down", "no");

    if (moved && commit) {
      commit(p);
    }
    if (!moved && click) {
      click();
    }
  }

  document.addEventListener("mousemove", move as any);
  document.addEventListener("mouseup", stopDrag);
}
