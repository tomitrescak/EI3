import styled from "@emotion/styled";
import React from "react";
import { SplitPane } from "react-multi-split-pane";

import { Components } from "../components/components_view";
import { MiddleView } from "../errors/middle_view";

const ApplicationPanes = styled(SplitPane)`
  path.last {
    marker-end: url(#arrow);
  }
  .arrow {
    fill: black;
  }
  .point {
    fill: transparent !important;
  }
  .port {
    background: transparent !important;
  }
  .port:hover {
    background: #c0ff00 !important;
  }
  .point.selected {
    fill: salmon !important;
  }
  .Resizer {
    background: #000;
    opacity: 0.2;
    z-index: 1;
    box-sizing: border-box;
    background-clip: padding-box;
  }

  .Resizer:hover {
    transition: all 2s ease;
  }

  .Resizer.horizontal {
    height: 11px;
    margin: -5px 0;
    border-top: 5px solid rgba(255, 255, 255, 0);
    border-bottom: 5px solid rgba(255, 255, 255, 0);
    cursor: row-resize;
  }

  .Resizer.horizontal:hover,
  .Resizer.horizontal.resizing {
    border-top: 5px solid rgba(0, 0, 0, 0.5);
    border-bottom: 5px solid rgba(0, 0, 0, 0.5);
  }

  .Resizer.vertical {
    width: 11px;
    margin: 0 -5px;
    border-left: 5px solid rgba(255, 255, 255, 0);
    border-right: 5px solid rgba(255, 255, 255, 0);
    cursor: col-resize;
  }

  .Resizer.vertical:hover,
  .Resizer.vertical.resizing {
    border-left: 5px solid rgba(0, 0, 0, 0.5);
    border-right: 5px solid rgba(0, 0, 0, 0.5);
  }

  .DragLayer {
    z-index: 1;
    pointer-events: none;
  }

  .DragLayer.resizing {
    pointer-events: auto;
  }

  .DragLayer.horizontal {
    cursor: row-resize;
  }

  .DragLayer.vertical {
    cursor: col-resize;
  }

  label: ApplicationPanes;
`;

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

const BarePane = styled.div`
  height: 100%;
  width: 100%;
  display: flex;
  overflow: auto;

  .form {
    width: 100%;
  }

  .storm-diagrams-canvas {
    position: absolute;
    background: rgba(0, 0, 0, 0.8);
    background-image: linear-gradient(
        0deg,
        transparent 24%,
        rgba(255, 255, 255, 0.05) 25%,
        rgba(255, 255, 255, 0.05) 26%,
        transparent 27%,
        transparent 74%,
        rgba(255, 255, 255, 0.05) 75%,
        rgba(255, 255, 255, 0.05) 76%,
        transparent 77%,
        transparent
      ),
      linear-gradient(
        90deg,
        transparent 24%,
        rgba(255, 255, 255, 0.05) 25%,
        rgba(255, 255, 255, 0.05) 26%,
        transparent 27%,
        transparent 74%,
        rgba(255, 255, 255, 0.05) 75%,
        rgba(255, 255, 255, 0.05) 76%,
        transparent 77%,
        transparent
      );
    background-size: 50px 50px;
    left: 0px;
    right: 0px;
    top: 0px;
    bottom: 0px;
  }
  label: BarePane;
`;

const PropertyPane = styled.div`
  background: #efefef;
  height: 100%;
  width: 100%;
  overflow: auto;
  padding: 6px;
  label: PropertyPane;
`;

const ErrorPane = styled.div`
  max-height: 90%;
  height: 100%;
  width: 100%;
  padding: 0px;
  label: ErrorPane;
`;

const LeftPane = styled.div`
  height: 100%;
  width: 100%;
  label: LeftPane;
`;

interface Props {
  Editor?: React.FC;
  Main: React.FC;
}

const MiddleLayout = ({ Main }: Props) => (
  <SplitPane split="horizontal" defaultSizes={[900, 100]}>
    <BarePane>
      <Main />
    </BarePane>
    <ErrorPane>
      <MiddleView />
    </ErrorPane>
  </SplitPane>
);

export const EiLayout = ({ Editor, Main }: Props) => {
  return (
    <ApplicationPanes
      split="vertical"
      minSize={10}
      defaultSizes={[150, 400, Editor ? 150 : 0]}
    >
      <LeftPane>
        <Components />
      </LeftPane>
      <MiddleLayout Main={Main} />
      {Editor ? (
        <PropertyPane>
          <Editor />
        </PropertyPane>
      ) : null}
    </ApplicationPanes>
  );
};
