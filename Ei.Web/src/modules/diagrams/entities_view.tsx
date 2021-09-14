import React from "react";

import { observer } from "mobx-react";

import { DiagramView } from "../diagrams/diagram_view";
import { EntityDiagramModel } from "../diagrams/model/entity/entity_diagram_model";
import { Ei } from "../ei/ei_model";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { useAppContext } from "../../config/context";
import { DiagramListener } from "@projectstorm/react-canvas-core";
import { EntityLinkModel } from "./model/entity/entity_link_model";
import { PointModel } from "@projectstorm/react-diagrams";
import { Point } from "@projectstorm/geometry";
import { useParams } from "react-router";

interface Props {
  id: string;
  type: string;
  entities: (ei: Ei) => HierarchicEntity[];
}

export const EntitiesView = observer((props: Props) => {
  function entities(customProps: Props = props) {
    return customProps.entities(context.ei);
  }

  const params = useParams<any>();
  const context = useAppContext();
  var engine = context.ei.engine;

  // const [selectedNode, setSelectedNode] = React.useState(() => {
  //   if (props.id) {
  //     const selectedNode = entities().find(
  //       (o) => o.Id.toLowerCase() === props.id.toLowerCase()
  //     ) as HierarchicEntity;
  //     // selectedNode.setSelected(true);
  //     return selectedNode;
  //   }
  // });

  var model = new EntityDiagramModel();

  // React.useEffect(() => {
  //   const id = params.roleId || params.organisationId;

  //   if (id) {
  //     // const selectedNode = entities().find(
  //     //   (o) => o.Id.toLowerCase() === id.toLowerCase()
  //     // ) as HierarchicEntity;

  //     // if (selectedNode && id === selectedNode.Id) {
  //     //   return;
  //     // }

  //     const nextNode = entities(props).find(
  //       (o) => o.Id === id
  //     ) as HierarchicEntity;
  //     // if (selectedNode) {
  //     //   selectedNode.setSelected(false);
  //     // }
  //     if (nextNode) {
  //       nextNode.setSelected(true);
  //     }
  //   }
  // });

  // let model = new DiagramModel();
  // model.version;

  let ents = entities();
  const id = params.roleId || params.organisationId;

  for (let node of entities()) {
    model.addNode(node);

    if (id && node.Id.toLowerCase() === id.toLowerCase()) {
      node.setSelected(true);
    } else {
      node.setSelected(false);
    }

    if (node.ParentId) {
      let parentLink = new EntityLinkModel();
      // create points
      if (node.points && node.points.length) {
        parentLink.setPoints(
          node.points.map(
            (p) =>
              new PointModel({
                link: parentLink,
                position: new Point(p.x, p.y),
              })
          )
        );
      }
      // add ports
      const parent = ents.find((p) => p.Id === node.ParentId);
      parentLink.setSourcePort(parent.getPort("bottom"));
      parentLink.setTargetPort(node.getPort("top"));

      model.addLink(parentLink);
    }
    node.ParentId; // subscribe
  }

  // listen and store offsets
  model.registerListener({
    offsetUpdated: ({ offsetX, offsetY }) => {
      localStorage.setItem(
        `EntityDiagram.${props.type}.offsetX`,
        offsetX.toString()
      );
      localStorage.setItem(
        `EntityDiagram.${props.type}.offsetY`,
        offsetY.toString()
      );
    },
  } as DiagramListener);

  // set offsets
  const currentOffsetX = localStorage.getItem(
    `EntityDiagram.${props.type}.offsetX`
  );
  const currentOffsetY = localStorage.getItem(
    `EntityDiagram.${props.type}.offsetY`
  );
  if (currentOffsetX) {
    model.setOffset(parseInt(currentOffsetX, 10), parseInt(currentOffsetY, 10));
  }

  // //5) load model into engine
  engine.setModel(model);
  // engine.repaintCanvas();

  return <DiagramView engine={engine} />;
});
