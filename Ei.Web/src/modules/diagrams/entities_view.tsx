import React from "react";

import { observer } from "mobx-react";

import { DiagramView } from "../diagrams/diagram_view";
import { EntityDiagramModel } from "../diagrams/model/entity/entity_diagram_model";
import { Ei } from "../ei/ei_model";
import { HierarchicEntity } from "../ei/hierarchic_entity_model";
import { useAppContext } from "../../config/context";

interface Props {
  id: string;
  type: string;
  entities: (ei: Ei) => HierarchicEntity[];
}

export const EntitiesView = observer((props: Props) => {
  function entities(customProps: Props = props) {
    return customProps.entities(context.ei);
  }

  const [selectedNode, setSelectedNode] = React.useState(() => {
    if (props.id) {
      const selectedNode = entities().find(
        (o) => o.Id.toLowerCase() === props.id.toLowerCase()
      ) as HierarchicEntity;
      // selectedNode.setSelected(true);
      return selectedNode;
    }
  });
  const context = useAppContext();

  React.useEffect(() => {
    if (props.id) {
      if (selectedNode && props.id === selectedNode.id) {
        return;
      }

      const nextNode = entities(props).find(
        (o) => o.Id === props.id
      ) as HierarchicEntity;
      if (selectedNode) {
        selectedNode.setSelected(false);
      }
      if (nextNode) {
        nextNode.setSelected(true);
      }
      setSelectedNode(nextNode);
    }
  });

  let model = new EntityDiagramModel();
  model.version;

  for (let node of entities()) {
    model.addNode(node);
    if (node.parentLink) {
      model.addLink(node.parentLink);
    }
    node.Parent; // subscribe
  }

  // listen and store offsets
  model.addListener({
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
  });

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

  return <DiagramView engine={context.ei.engine} diagram={model} />;
});
