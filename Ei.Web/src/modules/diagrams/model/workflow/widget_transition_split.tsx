import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { PortWidget } from 'storm-react-diagrams';
import { TransitionJoin } from '../../../ei/transition_model';

export interface StateJoinNodeWidgetProps {
  context?: App.Context;
  node: TransitionJoin;
}

// export interface EntityNodeWidgetState {}

const height = 20;
const width = 80;

@inject('context')
@observer
export class TransitionSplitWidget extends React.Component<StateJoinNodeWidgetProps> {
  public static defaultProps: StateJoinNodeWidgetProps = {
    node: null
  };

  constructor(props: StateJoinNodeWidgetProps) {
    super(props);
    this.state = {};
  }

  render() {
    let { node } = this.props;
    let labelSize = (node.Name || node.Id).length * 8 + 8;
    labelSize = labelSize < width ? width : labelSize;

    
    return (
      <div
        className="Entity-node"
        style={{
          position: 'relative',
          width: labelSize,
          height
        }}
      >
        <svg width={labelSize} height={height} transform={`rotate(${node.Horizontal ? 0 : 90} ${width / 2} ${height / 2})`}>
          <g id="Layer_1" />
          <g id="Layer_2">
						<rect
              fill={node.selected ? 'salmon' : 'black'}
							width={labelSize}
							height={height}
							y={0}
							style={{opacity: 1}}
							rx={5}
							ry={5}
            />
            <text
              x={labelSize / 2}
              y={9}
              style={{
                fontFamily: 'Verdana',
                fontSize: '14px',
                fill: 'white',
                textAlign: 'center',
                width: '200px',
                fontWeight: node.selected ? 'bold' : 'normal'
              }}
              textAnchor="middle"
              alignmentBaseline="central"
            >
              {this.props.node.Name || this.props.node.Id}
            </text>
          </g>
        </svg>
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            left: node.Horizontal ? 5 : ((labelSize / 2) + 7),
            top: node.Horizontal ? height -3 : - 20
          }}
        >
          <PortWidget name="split1" node={this.props.node} />
        </div>
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            left: node.Horizontal ? ((labelSize / 2) - 7) : ((labelSize / 2) + 7),
            top: node.Horizontal ? height - 3 : 3
          }}
        >
          <PortWidget name="split2" node={this.props.node} />
        </div>
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            left: node.Horizontal ? (labelSize - 20) : ((labelSize / 2) + 7),
            top: node.Horizontal ? height -3 : height + 5
          }}
        >
          <PortWidget name="split3" node={this.props.node} />
        </div>
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            left: node.Horizontal ? ((labelSize / 2) - 7) : ((labelSize / 2) - 20),
            top: node.Horizontal ? -10 : 3
          }}
        >
          <PortWidget name="input" node={this.props.node} />
        </div>
      </div>
    );
  }
}

export let TransitionSplitWidgetFactory = React.createFactory(TransitionSplitWidget);
