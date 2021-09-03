import React from "react";

import { observer } from "mobx-react";
import { PortWidget } from "storm-react-diagrams";

import { HierarchicEntity } from "../../../ei/hierarchic_entity_model";

export interface Props {
  node: HierarchicEntity;
  size?: number;
}

// export interface EntityNodeWidgetState {}

const height = 60;

export const EntityNodeWidget = observer(({ size = 150, node }: Props) => {
  // componentWillUpdate() {
  // 	let { node, context } = this.props;

  // 	// create reactive links and update is necessary
  // 	let parent = node.eiEntity.Parent;
  // 	let topPort = node.getPort('top');
  // 	let topLinks = Object.keys(topPort.links);
  // 	let ei = context.ei;

  // 	// remove link that are not in model
  // 	if (!parent && topLinks.length) {
  // 		(ei.engine.diagramModel as EntityModel).safeRemoveLink(topPort.getLinks()[topLinks[0]]);

  // 		context.ei.engine.diagramModel.forceUpdate();
  // 	}
  // 	if (parent && topLinks.length === 0) {
  // 		let parentNode = ei.OrganisationDiagram.nodes[parent];
  // 		let link = new LinkModel();
  // 		link.setSourcePort(node.getPort('top'));
  // 		link.setTargetPort(parentNode.getPort('bottom'));
  // 		ei.engine.diagramModel.addLink(link);

  // 		context.ei.engine.diagramModel.forceUpdate();
  // 	}
  // }

  size = node.Name.length * 8 + 30;

  return (
    <div
      className="Entity-node"
      style={{
        position: "relative",
        width: size,
        height,
      }}
    >
      <svg
        width={size}
        height={size}
        dangerouslySetInnerHTML={{
          __html: `
					<g id="Layer_1">
					</g>
					<g id="Layer_2">
						<rect fill="${node.selected ? "salmon" : "orange"}" stroke="${
            node.selected ? "white" : "black"
          }" stroke-width="3" stroke-miterlimit="10" width="${size}" height="${height}" rx="10" ry="10" />
						<text x="${size / 2}" y="${
            height / 2
          }" style="font-family: Verdana; font-size: 14px; fill: white; text-align: center; width: 200px; font-weight: ${
            node.selected ? "bold" : "normal"
          }" text-anchor="middle" alignment-baseline="central">
							${node.Name}
						</text>
					</g>
				`,
        }}
      />
      <div
        style={{
          position: "absolute",
          zIndex: 10,
          left: size / 2 - 8,
          top: -8,
        }}
      >
        <PortWidget name="top" node={node} />
      </div>
      <div
        style={{
          position: "absolute",
          zIndex: 10,
          left: size / 2 - 8,
          top: height - 8,
        }}
      >
        <PortWidget name="bottom" node={node} />
      </div>
    </div>
  );
});

export let EntityNodeWidgetFactory = React.createFactory(EntityNodeWidget);
