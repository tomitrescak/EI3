import React from "react";

import { Observer, observer } from "mobx-react";
import { action } from "mobx";

import { DiagramView } from "../diagrams/diagram_view";
import { Ei } from "../ei/ei_model";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { useAppContext } from "../../config/context";
import { useHistory } from "react-router-dom";
import { drag } from "./model/diagram_common";

interface Props {
  entities: (ei: Ei) => HierarchicEntity[];
}

export const EntitiesView = observer((props: Props) => {
  // function entities(customProps: Props = props) {
  //   return customProps.entities(context.ei);
  // }

  const svgRef = React.useRef<SVGSVGElement>(null);
  const context = useAppContext();
  let entities = props.entities(context.ei);

  // set offsets
  // const currentOffsetX = localStorage.getItem(
  //   `EntityDiagram.${props.type}.offsetX`
  // );
  // const currentOffsetY = localStorage.getItem(
  //   `EntityDiagram.${props.type}.offsetY`
  // );

  return (
    <DiagramView>
      <svg
        ref={svgRef}
        xmlns="http://www.w3.org/2000/svg"
        viewBox={`0 0 800 800`}
        width="100%"
        height="100%"
      >
        {entities.map((e, i) => (
          <EntityView
            key={e.Id}
            svgRef={svgRef}
            e={e}
            ents={entities}
            drag={drag}
          />
        ))}
      </svg>
    </DiagramView>
  );
});

function EntityView({
  svgRef,
  e,
  ents,
  drag,
}: {
  svgRef: React.MutableRefObject<SVGSVGElement>;
  e: HierarchicEntity;
  ents: HierarchicEntity[];
  drag: Function;
}) {
  const lineRef = React.useRef<SVGLineElement>(null);
  const history = useHistory();
  let parent = e.ParentId ? ents.find((p) => p.Id === e.ParentId) : null;

  const selected = e.url === location.pathname + location.search;

  e.connection = lineRef;

  return (
    <>
      <Observer>
        {() => (
          <svg
            cursor="pointer"
            id={`Layer_${e.Id}`}
            key={e.Id}
            x={e.position.x}
            y={e.position.y}
            onMouseDown={(evt) => drag(svgRef.current, e, evt, ents)}
            onClick={action(() => {
              // ents.forEach((e) => (e.selected = false));
              // e.selected = true;
              history.push(e.url);
            })}
          >
            <rect
              fill={selected ? "blue" : "orange"}
              strokeWidth="0"
              width={e.size}
              height={e.height}
              rx="10"
              ry="10"
            />
            <text
              x={e.size / 2}
              y={e.height / 2}
              style={{
                fontFamily: "Verdana",
                fontSize: "14px",
                fill: "white",
                textAlign: "center",
                width: "200px",
                fontWeight: selected ? "bold" : "normal",
              }}
              textAnchor="middle"
              dominantBaseline="middle"
            >
              {e.Name}
            </text>
            <circle cx={e.size / 2 - 2} cy={0} r="5" fill="red" />
            <circle cx={e.size / 2 - 2} cy={e.height} r="5" fill="red" />
          </svg>
        )}
      </Observer>

      {parent && (
        // <line
        //   ref={lineRef}
        //   x1={parent.bottomPort().x}
        //   y1={parent.bottomPort().y}
        //   x2={e.topPort().x}
        //   y2={e.topPort().y}
        //   stroke="#222"
        //   strokeWidth={3}
        // />
        <path
          ref={lineRef}
          stroke="grey"
          strokeWidth={5}
          fill="transparent"
          d={`M ${parent.bottomPort().x} ${parent.bottomPort().y} C ${
            parent.bottomPort().x
          } ${parent.bottomPort().y + 50}, ${e.topPort().x} ${
            e.topPort().y - 50
          }, ${e.topPort().x} ${e.topPort().y}`}
        />
      )}
    </>
  );
}
