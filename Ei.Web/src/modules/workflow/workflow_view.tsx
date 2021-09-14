import React from "react";

import { observer } from "mobx-react";
import createEngine from "@projectstorm/react-diagrams";

import { DiagramListener } from "@projectstorm/react-canvas-core";

import { DiagramView } from "../diagrams/diagram_view";
import { WorkflowDiagramModel } from "../diagrams/model/workflow/workflow_diagram_model";
import { WorkflowLinkFactory } from "../diagrams/model/workflow/workflow_link_factory";
import { WorkflowNodeFactory } from "../diagrams/model/workflow/workflow_node_factory";

import { useAppContext } from "../../config/context";
import { useParams } from "react-router";

export const WorkflowView = () => {
  const { workflowId } = useParams<{ workflowId: string }>();
  return <WorkflowContent workflowId={workflowId} />;
};

export const WorkflowContent = observer(
  ({ workflowId }: { workflowId: string }) => {
    const context = useAppContext();
    const workflow = context.ei.Workflows.find((w) => w.Id === workflowId);
    let model = new WorkflowDiagramModel(context);
    model.setGridSize(10);
    model.version;

    if (!workflow) {
      return <div>Workflow Deleted!</div>;
    }
    // add states
    for (let node of workflow.States) {
      model.addNode(node);
    }
    // // add transitions
    for (let node of workflow.Transitions) {
      model.addNode(node);
    }
    // add connections
    for (let node of workflow.Connections) {
      model.addLink(node.link);
      if (node.fromJoint) {
        model.addNode(node.fromJoint);
      }
      if (node.toJoint) {
        model.addNode(node.toJoint);
      }
    }
    // listen and store offsets
    model.registerListener({
      offsetUpdated: ({ offsetX, offsetY }) => {
        if (!isNaN(offsetX)) {
          localStorage.setItem(
            `EntityDiagram.Workflow.${workflowId}.offsetX`,
            offsetX.toString()
          );
        }
        if (!isNaN(offsetY)) {
          localStorage.setItem(
            `EntityDiagram.Workflow.${workflowId}.offsetY`,
            offsetY.toString()
          );
        }
      },
      zoomUpdated: (zoom) => {
        localStorage.setItem(
          `EntityDiagram.Workflow.${workflowId}.zoom`,
          zoom.zoom.toString()
        );
      },
    } as DiagramListener);
    // set offsets

    const currentOffsetX =
      localStorage.getItem(`EntityDiagram.Workflow.${workflowId}.offsetX`) ||
      "200";
    const currentOffsetY =
      localStorage.getItem(`EntityDiagram.Workflow.${workflowId}.offsetY`) ||
      "200";
    const currentZoom = localStorage.getItem(
      `EntityDiagram.Workflow.${workflowId}.zoom`
    );
    if (currentOffsetX) {
      model.setOffset(
        parseInt(currentOffsetX, 10),
        parseInt(currentOffsetY, 10)
      );

      model.setZoomLevel(parseInt(currentZoom, 10) || 100);
      model.setLocked(false);
    }

    // register engines
    const engine = createEngine();
    // const state = engine.getStateMachine().getCurrentState();
    // state.dragCanvas.config.allowDrag = true;
    let nodeFactories = engine.getNodeFactories();
    let linkFactories = engine.getLinkFactories();
    // nodeFactories.registerFactory(new DefaultNodeFactory());
    nodeFactories.registerFactory(new WorkflowNodeFactory());
    linkFactories.registerFactory(new WorkflowLinkFactory("default"));
    engine.setModel(model);
    return <DiagramView key={workflow.Id} engine={engine} />;
  }
);
