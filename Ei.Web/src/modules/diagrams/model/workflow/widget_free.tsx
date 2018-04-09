import * as React from 'react';

import { inject, observer } from 'mobx-react';
import { NodeModel } from 'storm-react-diagrams';

import { style } from 'typestyle/lib';
import { FreeJoint } from '../../../ei/connection_model';

export interface FreeWidgetProps {
  context?: App.Context;
  node: FreeJoint;
}

export interface PortProps {
  name: string;
  node: NodeModel;
}

export interface PortState {
  selected: boolean;
}

const invisiblePort = style({
  width: '0px!important',
  height: '0px!important'
});

export class PortWidget extends React.Component<PortProps, PortState> {
  constructor(props: PortProps) {
    super(props);
    this.state = {
      selected: false
    };
  }

  render() {
    return (
      <div
        className={'port ' + invisiblePort}
        data-name={this.props.name}
        data-nodeid={this.props.node.getID()}
      />
    );
  }
}

const size = 10;

@inject('context')
@observer
export class FreeWidget extends React.Component<FreeWidgetProps> {
  constructor(props: FreeWidgetProps) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <div
        className="Entity-node"
        style={{
          position: 'relative',
          width: size,
          height: size
        }}
      >
        <svg width={size} height={size}>
          <g id="Layer_1" />
          <g id="Layer_2">
            <ellipse
              fill="silver"
              style={{opacity: 0.4}}
              rx={size}
              ry={size}
              cx={8}
              cy={8}
              stroke="black"
              strokeWidth={3}
              strokeDasharray={'3 3'}
            />
          </g>
        </svg>
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            left: 8,
            top: 8
          }}
        >
          <PortWidget name="left" node={this.props.node} />
        </div>
      </div>
    );
  }
}

export let FreeWidgetFactory = React.createFactory(FreeWidget);
