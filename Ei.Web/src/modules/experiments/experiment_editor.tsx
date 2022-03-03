import styled from "@emotion/styled";
import { Observer, useLocalObservable } from "mobx-react";
import React from "react";
import { SplitPane } from "react-multi-split-pane";
import { Button, Icon, List, Menu, Message, Select } from "semantic-ui-react";
import { useAppContext } from "../../config/context";
import { Ui, useQuery } from "../../helpers/client_helpers";
import { Formix } from "../Form";
import { ExperimentProperties } from "./components/experimentProperties";
import { timerComponent } from "./components/timerEditor";
import { transformComponent } from "./components/transformEditor";
import { ComponentDao, GameObjectDao } from "./experiment_model";
import { simulationProjectComponent } from "./components/simulationProject";
import { agentEnvironmentEditor } from "./components/agentEnvironmentEditor";
import { environmentalObjectEditor } from "./components/environmentalObjectEditor";
import { delayActionEditor } from "./components/delayActionEditor";
import { simulationAgentComponent } from "./components/simulationAgentEditor";
import { randomDecisionReasoner } from "./components/randomDecisionReasoner";
import { environmentalActuator } from "./components/environmentalActuatorEditor";
import { sensorEditor } from "./components/sensorEditor";
import { linearNavigationEditor } from "./components/linearNavigationEditor";
import { toJS } from "mobx";
import { ExperimentCanvas } from "./components/experiment_canvas";
import { LogMessage } from "./components/experimentCommon";
import { WorkflowEditor } from "../diagrams/workflow_view";

const ListHeader = styled(List.Header)`
  padding: 4px 16px;

  background: #aeaeae;

  &.secondary {
    background: #dedede;
  }
  font-weight: bold;
  width: 100%;

  :first-of-type {
    margin-top: 0px;
  }
`;

const ListItem = styled(List.Item)`
  padding: 4px 16px !important;
  cursor: pointer;

  :hover {
    background: #efefef;
  }

  &.active {
    background: #f5f5f5;
  }

  label: ListItem;
`;

type ComponentOption = {
  text: string;
  globalDependencies?: ComponentOption[];
  componentDependencies?: ComponentOption[];
  type: string;
  editor: React.FC<any>;
  defaultValue: () => Any;
};

const componentOptions: ComponentOption[] = [
  transformComponent,
  timerComponent,
  simulationProjectComponent,
  agentEnvironmentEditor,
  environmentalObjectEditor,
  delayActionEditor,
  simulationAgentComponent,
  environmentalActuator,
  randomDecisionReasoner,
  sensorEditor,
  linearNavigationEditor,
].sort((a, b) => a.text.localeCompare(b.text));

function typeFirst(obj: any) {
  if (obj == null || typeof obj !== "object") {
    return obj;
  }

  if (Array.isArray(obj)) {
    return obj.map((m) => typeFirst(m));
  }

  let result: any = {};

  // type must be first
  if (obj.$type) {
    result.$type = obj.$type;
  }

  for (let key of Object.keys(obj)) {
    if (key === "$type") {
      continue;
    }
    result[key] = typeFirst(obj[key]);
  }

  return result;
}

const MAX_MESSAGES = 200;

export const ExperimentEditor = () => {
  const context = useAppContext();
  const { id } = useQuery();
  const experiment = context.ei.Experiments.find((e) => e.Id === id);
  const allMessages = React.useRef([]);

  const state = useLocalObservable(() => ({
    selectedGameObject: null as GameObjectDao,
    isAgent: false,
    agentPositions: {},
    select(go: GameObjectDao) {
      this.selectedGameObject = go;
      if (
        go.Components &&
        go.Components.some((c) => c.$type === simulationAgentComponent.type)
      ) {
        this.isAgent = true;
        context.messages
          .replace(allMessages.current.filter((m) => m.agent === go.Name))
          .slice(-MAX_MESSAGES);
      } else {
        this.isAgent = false;
        context.messages.replace(allMessages.current.slice(-MAX_MESSAGES));
      }
    },
  }));

  // this thing monitors the institution
  React.useEffect(() => {
    const id = context.client.send({
      query: "MonitorInstitution",
      isSubscription: true,
      receiver: (data: LogMessage) => {
        if (data.agent != null) {
          let agent = experiment.GameObjects.find(
            (go) => go.Name === data.agent
          );
          if (data.component === "Navigation") {
            let position = data.message.match(/\[(.*),(.*)\]/);
            agent.Components[0].position.x = position[1];
            agent.Components[0].position.y = position[2];
          }
          if (data.component === "Institution") {
            let match = data.message.match(
              /moved to '(\w+)' in workflow '(\w+)'/
            );
            if (match) {
              state.agentPositions[agent.Name] = {
                state: match[1],
                workflow: match[2],
              };
              // mark all states as not visited
              let workflow = context.ei.Workflows.find(
                (w) => w.Id === match[2]
              );
              workflow.States.forEach((s) => (s.active = false));
              workflow.States.find((s) => s.Id === match[1]).active = true;
            } else {
              match = data.message.match(
                /performed action '(\w+)' in connection '(\w+)'/
              );
              if (match) {
                if (state.agentPositions[agent.Name]) {
                  let workflow = context.ei.Workflows.find(
                    (w) => w.Id === state.agentPositions[agent.Name].workflow
                  );
                  workflow.Connections.forEach((s) => (s.active = false));
                  workflow.Connections.find((s) => s.Id === match[2]).active =
                    true;
                }
              }
            }
          }
        }

        allMessages.current.push(data);

        if (!state.isAgent || data.agent === state.selectedGameObject.Name) {
          context.messages.push(data);
          if (context.messages.length > MAX_MESSAGES) {
            context.messages.unshift();
          }
        }
      },
    });
    return () => {
      context.client.unsubscribe("MonitorInstitution", id);
    };
  }, []);

  if (experiment.GameObjects == null) {
    experiment.GameObjects = [];
  }

  if (experiment == null) {
    return (
      <div style={{ padding: 16 }}>
        <Message header="Experiment not found" error icon="ban" />
      </div>
    );
  }

  return (
    <div
      style={{
        width: "calc(100% - 1px)",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Menu
        inverted
        attached="top"
        color="grey"
        style={{ borderRadius: "0px", height: 44 }}
      >
        <Observer>{() => <Menu.Item content={experiment.Name} />}</Observer>

        <Menu.Item
          icon="play"
          onClick={() => {
            allMessages.current = [];
            context.messages.clear();

            // console.log(typeFirst(toJS(experiment)));
            context.ei.run(
              context.client,
              JSON.stringify(typeFirst(toJS(experiment)), null, 2)
            );
          }}
        />
      </Menu>

      <div style={{ flex: 1, position: "relative" }}>
        <SplitPane>
          <div style={{ width: "100%", overflow: "auto" }}>
            <List>
              <ListHeader content="Administration" />
              <ListItem
                icon="cog"
                content="Properties"
                className={
                  state.selectedGameObject === (experiment as any)
                    ? "active"
                    : ""
                }
                onClick={() => state.select(experiment as any)}
              />
              <ListHeader
                style={{
                  display: "flex",
                  alignItems: "center",
                  paddingRight: 4,
                }}
              >
                <span style={{ flex: 1 }}>Game Objects</span>
                <Button
                  color="green"
                  icon="plus"
                  compact
                  size="tiny"
                  onClick={async () => {
                    const modal = await Ui.promptText(
                      "Name of the Game Object"
                    );
                    if (modal.isConfirmed) {
                      experiment.GameObjects.push({
                        Id: Date.now().toString(),
                        Name: modal.value,
                        Icon: null,
                        Components: [transformComponent.defaultValue()],
                      });
                    }
                  }}
                />
              </ListHeader>
              <Observer>
                {() => (
                  <>
                    {experiment.GameObjects.map((go) => (
                      <ListItem
                        key={go.Id}
                        className={
                          state.selectedGameObject === go ? "active" : ""
                        }
                        onClick={() => state.select(go)}
                      >
                        {go.Name}
                      </ListItem>
                    ))}
                  </>
                )}
              </Observer>
            </List>
          </div>
          <div
            style={{ width: "100%", overflow: "auto", position: "relative" }}
          >
            <SplitPane split="horizontal">
              <div style={{ width: "100%" }}>
                <Menu
                  inverted
                  color="blue"
                  style={{ borderRadius: 0, margin: 0 }}
                >
                  <Menu.Item content="Simulation" />
                </Menu>
                <ExperimentCanvas experiment={experiment} state={state} />
              </div>
              <div style={{ width: "100%" }}>
                <Menu
                  color="blue"
                  inverted
                  style={{ borderRadius: 0, margin: 0 }}
                >
                  <Menu.Item content="Workflow" />
                </Menu>
                <div>
                  <Observer>
                    {() => {
                      let activeAgent = state.isAgent
                        ? state.selectedGameObject
                        : null;
                      if (!activeAgent) {
                        return (
                          <Message
                            style={{ margin: 16 }}
                            icon="info"
                            positive
                            content="Please select an agent to see its position within
                          the workflow"
                          />
                        );
                      }
                      let position = state.agentPositions[activeAgent.Name];
                      if (!position) {
                        return (
                          <Message
                            style={{ margin: 16 }}
                            icon="info"
                            content="Agent did not join any workflow"
                            positive
                          />
                        );
                      }
                      return <WorkflowEditor workflowId={position.workflow} />;
                    }}
                  </Observer>
                </div>
              </div>
            </SplitPane>
          </div>
          <div style={{ width: "100%", background: "white", overflow: "auto" }}>
            <List className="ui form">
              <Observer>
                {() => (
                  <>
                    {state.selectedGameObject ? (
                      <Formix initialValues={state.selectedGameObject}>
                        <>
                          <ListHeader icon="cog" content="Components" />
                          <ExperimentProperties />

                          {state.selectedGameObject.Components && (
                            <>
                              {state.selectedGameObject.Components.map(
                                (c, index) => {
                                  const currentComponent =
                                    componentOptions.find(
                                      (o) => o.type === c.$type
                                    );
                                  if (currentComponent == null) {
                                    return (
                                      <div key={c.Id}>
                                        Component does not exist! {c.$type}
                                      </div>
                                    );
                                  }
                                  return (
                                    <div key={c.Id}>
                                      <ListHeader
                                        className="secondary"
                                        style={{ paddingRight: 8 }}
                                      >
                                        <div style={{ display: "flex" }}>
                                          <span style={{ flex: 1 }}>
                                            {currentComponent.text}
                                          </span>
                                          {index > 0 && (
                                            <Icon
                                              name="close"
                                              title="Remove Component"
                                              style={{
                                                color: "#999",
                                                cursor: "pointer",
                                              }}
                                              onClick={() =>
                                                state.selectedGameObject.Components.splice(
                                                  index,
                                                  1
                                                )
                                              }
                                            />
                                          )}
                                        </div>
                                      </ListHeader>
                                      <List.Item>
                                        <currentComponent.editor
                                          component={c as any}
                                        />
                                        {/* Check all dependencies */}
                                        {(
                                          currentComponent.globalDependencies ||
                                          []
                                        )
                                          .filter((f) =>
                                            experiment.GameObjects.every((g) =>
                                              g.Components.every(
                                                (c) => c.$type !== f.type
                                              )
                                            )
                                          )
                                          .map((t, i) => (
                                            <div style={{ padding: 8 }}>
                                              <Message
                                                visible
                                                error
                                                header="Missing Global Dependency"
                                                content={t.text}
                                                key={i}
                                              />
                                            </div>
                                          ))}
                                        {(
                                          currentComponent.componentDependencies ||
                                          []
                                        )
                                          .filter((f) =>
                                            state.selectedGameObject.Components.every(
                                              (c) => c.$type !== f.type
                                            )
                                          )
                                          .map((t, i) => (
                                            <Message
                                              error
                                              header="Missing Component"
                                              content={t.text}
                                              key={i}
                                            />
                                          ))}
                                      </List.Item>
                                    </div>
                                  );
                                }
                              )}

                              <div style={{ padding: 8 }}>
                                <Select
                                  text="Add Component"
                                  value=""
                                  fluid
                                  selection
                                  search
                                  options={componentOptions.map((c) => ({
                                    value: c.type,
                                    text: c.text,
                                  }))}
                                  onChange={(e, v) => {
                                    const cmp = componentOptions.find(
                                      (c) => c.type === v.value
                                    );

                                    // we do not allow two transforms
                                    if (
                                      v.value == "transform" &&
                                      state.selectedGameObject.Components.some(
                                        (c) => c.$type === cmp.type
                                      )
                                    ) {
                                      return;
                                    }

                                    state.selectedGameObject.Components.push(
                                      cmp.defaultValue()
                                    );
                                  }}
                                />
                                <Button
                                  style={{ marginTop: 8 }}
                                  color="red"
                                  icon="trash"
                                  content="Delete Game Object"
                                  onClick={() =>
                                    Ui.confirmDialogAsync(
                                      "Do you want to delete this Game Object",
                                      "Removing Game Object",
                                      "Delete"
                                    ).then((v) => {
                                      if (v) {
                                        experiment.GameObjects.splice(
                                          experiment.GameObjects.indexOf(
                                            state.selectedGameObject
                                          ),
                                          1
                                        );
                                        state.selectedGameObject = null;
                                      }
                                    })
                                  }
                                />
                              </div>
                            </>
                          )}
                        </>
                      </Formix>
                    ) : (
                      <ListItem>Nothing selected</ListItem>
                    )}
                  </>
                )}
              </Observer>
            </List>
          </div>
        </SplitPane>
      </div>
    </div>
  );
};
