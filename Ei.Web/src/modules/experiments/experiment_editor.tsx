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
import { GameObjectDao } from "./experiment_model";

const ListHeader = styled(List.Header)`
  padding: 4px 16px;

  background: #cecece;

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

const componentOptions = [transformComponent, timerComponent];

export const ExperimentEditor = () => {
  const context = useAppContext();
  const { id } = useQuery();
  const experiment = context.ei.Experiments.find((e) => e.Id === id);
  const state = useLocalObservable(() => ({
    selectedGameObject: null as GameObjectDao,
  }));

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

        <Menu.Item icon="play" />
      </Menu>

      <div style={{ flex: 1, overflow: "auto" }}>
        <SplitPane>
          <div style={{ width: "100%" }}>
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
                onClick={() => (state.selectedGameObject = experiment as any)}
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
                        onClick={() => (state.selectedGameObject = go)}
                      >
                        {go.Name}
                      </ListItem>
                    ))}
                  </>
                )}
              </Observer>
            </List>
          </div>
          <div style={{ background: "blue", width: "100%", overflow: "auto" }}>
            Content <br />
            Content <br />
            Content <br />
          </div>
          <div style={{ width: "100%", background: "white" }}>
            <List className="ui form">
              <ListHeader icon="list" content="Properties" />
              <Observer>
                {() => (
                  <>
                    {state.selectedGameObject ? (
                      <Formix initialValues={state.selectedGameObject}>
                        <>
                          <ExperimentProperties />
                          {state.selectedGameObject.Components && (
                            <ListHeader icon="cog" content="Components" />
                          )}

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
                                    <div
                                      key={c.Id}
                                      style={{
                                        borderBottom: "1px dashed #dedede",
                                        paddingBottom: 4,
                                      }}
                                    >
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
                                          component={c}
                                        />
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
