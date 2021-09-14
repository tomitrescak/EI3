import React from "react";

import { NodeModel, PortWidget } from "@projectstorm/react-diagrams";

import { FreeJoint } from "../../../ei/connection_model";
import styled from "@emotion/styled";

export const Port = styled.div`
  width: 16px;
  height: 16px;
  z-index: 10;
  background: rgba(0, 0, 0, 0.5);
  border-radius: 8px;
  cursor: pointer;
  &:hover {
    background: rgba(0, 0, 0, 1);
  }
`;

export interface FreeWidgetProps {
  node: FreeJoint;
}

export interface PortProps {
  name: string;
  node: NodeModel;
}

export interface PortState {
  selected: boolean;
}

// const invisiblePort = style({
//   width: '0px!important',
//   height: '0px!important'
// });

// export class PortWidget extends React.Component<PortProps, PortState> {
//   constructor(props: PortProps) {
//     super(props);
//     this.state = {
//       selected: false
//     };
//   }

//   render() {
//     return (
//       <div
//         className={'port ' + invisiblePort}
//         data-name={this.props.name}
//         data-nodeid={this.props.node.getID()}
//       />
//     );
//   }
// }

const size = 20;

export const FreeWidget = ({ node }: FreeWidgetProps) => {
  return (
    <div
      className="Entity-node"
      style={{
        position: "relative",
        width: size,
        height: size,
      }}
    >
      <svg width={size} height={size}>
        <g id="Layer_1" />
        <g id="Layer_2">
          <ellipse
            fill="silver"
            style={{ opacity: 0.4 }}
            rx={10}
            ry={10}
            cx={10}
            cy={10}
            stroke="black"
            strokeWidth={3}
            strokeDasharray={"3 3"}
          />
        </g>
      </svg>
      <PortWidget
        style={{
          position: "absolute",
          zIndex: 10,
          left: 8,
          top: 8,
        }}
        port={node.getPort("left")}
        engine={node.ei.engine}
      >
        <Port />
      </PortWidget>
    </div>
  );
};

export let FreeWidgetFactory = React.createFactory(FreeWidget);
