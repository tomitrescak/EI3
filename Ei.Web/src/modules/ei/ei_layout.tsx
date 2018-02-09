import * as React from 'react';
import * as SplitPane from 'react-split-pane';

import { observer } from 'mobx-react';
import { style } from 'typestyle';
import { Components } from '../components/components_view';
import { MiddleView } from '../errors/middle_view';
import { ChildProps } from '../ws/interface';

const layoutStyle = style({
  $nest: {
    '& path.last': {
      markerEnd: 'url(#arrow)'
    } as any,
    '& .arrow': {
      fill: 'black'
    },
    '& .point': {
      fill: 'transparent!important'
    },
    '& .port': {
      background: 'transparent!important'
    },
    '& .port:hover': {
      background: '#c0ff00!important'
    },
    '& .point.selected': {
      fill: 'rgba(0, 0, 0, 0.5)!important'
    },
    '& .Resizer': {
      background: '#000',
      opacity: 0.2,
      zIndex: 1,
      boxSizing: 'border-box',
      backgroundClip: 'padding-box'
    },
    '& .Resizer:hover': {
      transition: 'all 2s ease'
    },
    '& .Resizer.horizontal': {
      height: '11px',
      margin: '-5px 0',
      borderTop: '6px solid rgba(255, 255, 255, 0)',
      borderBottom: '5px solid rgba(255, 255, 255, 0)',
      cursor: 'row-resize',
      width: '100%'
    },
    '& .Resizer.horizontal:hover': {
      borderTop: '5px solid rgba(0, 0, 0, 0.5)',
      borderBottom: '5px solid rgba(0, 0, 0, 0.5)'
    },
    '& .Resizer.vertical': {
      width: '11px',
      margin: '0 -5px',
      borderLeft: '5px solid rgba(255, 255, 255, 0)',
      borderRight: '5px solid rgba(255, 255, 255, 0)',
      cursor: 'col-resize'
    },
    '& .Resizer.vertical:hover': {
      borderLeft: '5px solid rgba(0, 0, 0, 0.5)',
      borderRight: '5px solid rgba(0, 0, 0, 0.5)'
    },
    '& .SplitPane.horizontal': {
      position: 'inherit!important' as any
    }
  }
});

// const pane = style({
//   padding: '12px',
//   overflow: 'auto',
//   height: '100%',
//   $nest: {
//     '.storm-diagrams-canvas': {
//       background: 'rgba(0, 0, 0, 0.8)',
//       backgroundImage:
//         'linear-gradient(0deg, transparent 24%, rgba(255, 255, 255, 0.05) 25%, rgba(255, 255, 255, 0.05) 26%, transparent 27%, transparent 74%, rgba(255, 255, 255, 0.05) 75%, rgba(255, 255, 255, 0.05) 76%, transparent 77%, transparent), linear-gradient(90deg, transparent 24%, rgba(255, 255, 255, 0.05) 25%, rgba(255, 255, 255, 0.05) 26%, transparent 27%, transparent 74%, rgba(255, 255, 255, 0.05) 75%, rgba(255, 255, 255, 0.05) 76%, transparent 77%, transparent)',
//       backgroundSize: '50px 50px',
//       height: '100%',
//       minHeight: '400px',
//       margin: '-12px'
//     }
//   }
// });

const barePane = style({
  height: '100%',
  $nest: {
    '.storm-diagrams-canvas': {
      position: 'absolute',
      background: 'rgba(0, 0, 0, 0.8)',
      backgroundImage:
        'linear-gradient(0deg, transparent 24%, rgba(255, 255, 255, 0.05) 25%, rgba(255, 255, 255, 0.05) 26%, transparent 27%, transparent 74%, rgba(255, 255, 255, 0.05) 75%, rgba(255, 255, 255, 0.05) 76%, transparent 77%, transparent), linear-gradient(90deg, transparent 24%, rgba(255, 255, 255, 0.05) 25%, rgba(255, 255, 255, 0.05) 26%, transparent 27%, transparent 74%, rgba(255, 255, 255, 0.05) 75%, rgba(255, 255, 255, 0.05) 76%, transparent 77%, transparent)',
      backgroundSize: '50px 50px',
      left: '0px',
      right: '0px',
      top: '0px',
      bottom: '0px'
    }
  }
});

const propertyPane = style({
  background: '#efefef',
  height: '100%',
  overflow: 'auto',
  padding: '6px'
});

const errorPane = style({
  maxHeight: '90%',
  height: '100%',
  width: '100%',
  padding: '0px'
});

const componentsType = style({
  height: '100%'
});

interface Props {
  children: any;
  context: App.Context;
  views: { [index: string]: JSX.Element };
}

interface Query {
  LoadInstitution: string;
}

const MiddleLayout = ({ views }: any) => (
  <SplitPane split="horizontal" defaultSize={100} primary="second">
    <div className={barePane}>{views.graph}</div>
    <div className={errorPane}><MiddleView /></div>
  </SplitPane>
);

export const EiLayout = observer(({ views }: ChildProps<Props, Query>) => {
  return (
    <SplitPane split="vertical" minSize={100} defaultSize={250} className={layoutStyle}>
      <div className={componentsType}>
        <Components />
      </div>
      {views.editor ? (
        <SplitPane split="vertical" defaultSize={400} primary="second">
          <MiddleLayout views={views} />
          <div className={propertyPane}>{views.editor}</div>
        </SplitPane>
      ) : (
        <MiddleLayout views={views} />
      )}
    </SplitPane>
  );
});

EiLayout.displayName = 'EiLayout';
