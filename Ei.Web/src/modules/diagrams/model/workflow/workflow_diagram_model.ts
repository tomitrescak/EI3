import { DiagramListener, DiagramModel } from "@projectstorm/react-diagrams";

import { makeObservable, observable } from "mobx";
import { AppContext } from "../../../../config/context";
import { Ui } from "../../../../helpers/client_helpers";
import { WorkflowLinkModel } from "./workflow_link_model";
import { WorkflowPortModel } from "./workflow_port_model";

export class WorkflowDiagramModel extends DiagramModel {
  @observable version = 0;

  constructor(context: AppContext) {
    super();

    const listener = {
      // nodesUpdated: (_e) => {
      //   // node was deleted, remove it from the collection
      //   // let node = e.node as HierarchicEntity;
      //   // if (!e.isCreated) {
      //   //   node.remove();
      //   // }
      // },
      linksUpdated: (e) => {
        const link = e.link as WorkflowLinkModel;
        // link deleted
        if (!e.isCreated) {
          link.safeRemoveLink();
        }
        if (e.isCreated) {
          // create handles for empty connections
          let checkConnection = () => {
            setTimeout(() => {
              // we may be modifying exiting connection
              if (
                link.workflow.Connections.some(
                  (c) => c.Id === link.connection.Id
                )
              ) {
                return;
              }

              const fromPort = e.link.getSourcePort() as WorkflowPortModel;
              const toPort = e.link.getTargetPort() as WorkflowPortModel;

              // add connection
              let connection = link.connection;
              connection.From = fromPort.getNode().getID();

              if (toPort) {
                connection.To = toPort.getNode().getID();
              }

              connection.update();

              link.workflow.Connections.push(link.connection);

              context.Router.push(
                context.ei.createWorkflowUrl(
                  link.workflow,
                  "connection",
                  link.connection.Id
                )
              );

              Ui.history.step();
            }, 50);
            document.removeEventListener("mouseup", checkConnection);
          };
          document.addEventListener("mouseup", checkConnection);

          e.link.registerListener({
            targetPortChanged: () => {
              // // we may be modifying exiting connection
              // if (link.workflow.Connections.some(c => c.Id === link.connection.Id)) {
              //   return;
              // }
              // /////////////////////////////////////////////
              // // links can only be created parent to child
              // // if (e.link.sourcePort.name === 'left') {
              // //   let sourcePort = e.link.sourcePort;
              // //   e.link.sourcePort = e.link.targetPort;
              // //   e.link.targetPort = sourcePort;
              // // }
              // // if (e.link.sourcePort.name === 'top') {
              // //   let sourcePort = e.link.sourcePort;
              // //   e.link.sourcePort = e.link.targetPort;
              // //   e.link.targetPort = sourcePort;
              // // }
              // const fromPort = e.link.sourcePort as WorkflowPortModel;
              // const toPort = e.link.targetPort as WorkflowPortModel;
              // // /////////////////////////////////////
              // // // we cannot join top or bottoms
              // // if (fromPort.position === toPort.position ||
              // //     fromPort.position === 'left' || fromPort.position === 'top' ||
              // //     toPort.position === 'right' || toPort.position === 'bottom'
              // //   ) {
              // //   link.safeRemove(this);
              // //   return;
              // // }
              // // add connection
              // let connection = link.connection;
              // connection.From = fromPort.parentNode.id;
              // connection.To = toPort.parentNode.id;
              // link.workflow.Connections.push(link.connection);
              // link.workflow.ei.store.viewStore.showConnection(link.workflow.Id, link.workflow.Name, link.connection.Id);
              // connection.update();
              ///////////////////////////////////
              // we cannot create loops
              // let parentNode = fromPort.getParent() as State;
              // let parentNodeTopPort = parentNode.getPort('left') as WorkflowPortModel;
              // let childNode = toPort.getParent() as State;
              // let childNodeBottomPort = childNode.getPort('bottom') as WorkflowPortModel;
              // let parentLinks = parentNodeTopPort.linkArray;
            },
          } as any);
        }
      },
    } as DiagramListener;

    this.registerListener(listener);

    makeObservable(this);
  }
}
