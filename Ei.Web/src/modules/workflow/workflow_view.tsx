import * as React from "react";

import { inject, observer } from "mobx-react";
import { DefaultNodeFactory, DiagramEngine } from "storm-react-diagrams";

import { DiagramView } from "../diagrams/diagram_view";
import { WorkflowDiagramModel } from "../diagrams/model/workflow/workflow_diagram_model";
import { WorkflowLinkFactory } from "../diagrams/model/workflow/workflow_link_factory";
import { WorkflowNodeFactory } from "../diagrams/model/workflow/workflow_node_factory";
import { Entity } from "../ei/entity_model";

interface Props {
  id: string;
  context?: App.Context;

  selectedState?: string;
  selectedTransition?: string;
  selectedConnection?: string;
}

@inject("context")
@observer
export class WorkflowView extends React.Component<Props> {
  static displayName = "WorkflowView";

  selectedNode: Entity;

  get ei() {
    return this.props.context.ei;
  }

  get workflow() {
    return this.ei.Workflows.find((w) => w.Id === this.props.id);
  }

  // componentWillMount() {
  //   // if (this.props.id) {
  //   //   this.selectedNode = this.entities().find(o => o.Id === this.props.id) as HierarchicEntity;
  //   //   this.selectedNode.setSelected(true);
  //   // }
  // }

  // componentWillUpdate(nextProps: Props) {
  //   // if (nextProps.id) {
  //   //   const nextNode = this.entities(nextProps).find(o => o.Id === nextProps.id) as HierarchicEntity;
  //   //   if (this.selectedNode) {
  //   //     this.selectedNode.setSelected(false);
  //   //   }
  //   //   if (nextNode) {
  //   //     nextNode.setSelected(true);
  //   //   }
  //   //   this.selectedNode = nextNode;
  //   // }
  // }

  render() {
    let model = new WorkflowDiagramModel();
    model.setGridSize(10);
    model.version;

    let workflow = this.workflow;
    if (!workflow) {
      return <div>Workflow Deleted!</div>;
    }

    // add states
    for (let node of this.workflow.States) {
      model.addNode(node);
    }

    // add transitions
    for (let node of this.workflow.Transitions) {
      model.addNode(node);
    }

    // add connections
    for (let node of this.workflow.Connections) {
      model.addLink(node.link);

      if (node.fromJoint) {
        model.addNode(node.fromJoint);
      }

      if (node.toJoint) {
        model.addNode(node.toJoint);
      }
    }

    // listen and store offsets
    model.addListener({
      offsetUpdated: ({ offsetX, offsetY }) => {
        localStorage.setItem(
          `EntityDiagram.Workflow.${this.props.id}.offsetX`,
          offsetX.toString()
        );
        localStorage.setItem(
          `EntityDiagram.Workflow.${this.props.id}.offsetY`,
          offsetY.toString()
        );
      },
      zoomUpdated: (zoom) => {
        localStorage.setItem(
          `EntityDiagram.Workflow.${this.props.id}.zoom`,
          zoom.zoom.toString()
        );
      },
    });

    // set offsets
    const currentOffsetX =
      localStorage.getItem(`EntityDiagram.Workflow.${this.props.id}.offsetX`) ||
      "200";
    const currentOffsetY =
      localStorage.getItem(`EntityDiagram.Workflow.${this.props.id}.offsetY`) ||
      "200";
    const currentZoom = localStorage.getItem(
      `EntityDiagram.Workflow.${this.props.id}.zoom`
    );
    if (currentOffsetX) {
      model.setOffset(
        parseInt(currentOffsetX, 10),
        parseInt(currentOffsetY, 10)
      );
      model.setZoomLevel(parseInt(currentZoom, 10) || 100);
    }

    // register engines
    const engine = new DiagramEngine();
    engine.registerNodeFactory(new DefaultNodeFactory());
    engine.registerNodeFactory(new WorkflowNodeFactory());
    engine.registerLinkFactory(new WorkflowLinkFactory("default"));

    return (
      <>
        <DiagramView engine={engine} diagram={model} />
      </>
    );
  }
}
