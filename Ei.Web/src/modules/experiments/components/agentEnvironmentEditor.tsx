import { Observer } from "mobx-react";
import React from "react";
import { Button } from "semantic-ui-react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";

type NoLocationAction = {};

export type AgentEnvironment = ComponentDao & {
  Definition: {
    Width: number;
    Height: number;
    MetersPerPixel: number;
    ActionsWithNoLocation: NoLocationAction[];
  };
};

const Editor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input
        name="Definition.MetersPerPixel"
        size="mini"
        label="Meters Per Pixel"
        fluid
      />

      <Input name="Definition.Width" size="mini" label="Width" fluid />
      <Input name="Definition.Height" size="mini" label="Height" fluid />

      <fieldset>
        <legend>
          Actions With No Location{" "}
          <Button
            icon="plus"
            size="mini"
            color="green"
            basic
            compact
            onClick={() => component.Definition.ActionsWithNoLocation.push({})}
          />
        </legend>
        <table style={{ width: "100%" }}>
          <thead>
            <tr>
              <th style={{ width: "30%" }}>Id</th>
              <th style={{ width: "20%" }}>Dur. (ms)</th>
              <th>Parameters</th>
            </tr>
          </thead>
          <tbody>
            <Observer>
              {() =>
                component.Definition.ActionsWithNoLocation.map((a, i) => {
                  if (a.Parameters == null) {
                    a.Parameters = [];
                  }
                  return (
                    <Formix key={i} initialValues={a}>
                      <tr>
                        <td>
                          <Input name="Id" size="mini" fluid />
                        </td>
                        <td>
                          <Input name="Duration" size="mini" fluid />
                        </td>
                        <td>
                          <div style={{ display: "flex" }}>
                            <div style={{ flex: 1 }}>
                              <Observer>
                                {() =>
                                  a.Parameters?.map((p, pi) => (
                                    <div key={pi} className="inline">
                                      <Input
                                        name={`Parameters.${pi}.Name`}
                                        placeholder="Name"
                                        size="mini"
                                        fluid
                                      />
                                      <Input
                                        name={`Parameters.${pi}.Value`}
                                        placeholder="Value"
                                        size="mini"
                                        fluid
                                      />
                                      <Button
                                        icon="minus"
                                        size="mini"
                                        style={{ height: 30, marginLeft: 1 }}
                                        basic
                                        color="red"
                                        compact
                                        onClick={() =>
                                          a.Parameters.splice(pi, 1)
                                        }
                                      />
                                    </div>
                                  )) || null
                                }
                              </Observer>
                            </div>
                            <Button
                              icon="plus"
                              size="mini"
                              style={{ height: 30, marginLeft: 1 }}
                              basic
                              compact
                              color="green"
                              onClick={() => a.Parameters.push({})}
                            />
                          </div>
                        </td>
                      </tr>
                    </Formix>
                  );
                })
              }
            </Observer>
          </tbody>
        </table>
      </fieldset>
    </ExperimentPane>
  </Formix>
);

export const agentEnvironmentEditor = {
  text: "Agent Environment",
  globalDependencies: [],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.AgentEnvironment, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: agentEnvironmentEditor.type,
    Definition: {
      Width: 0,
      Height: 0,
      MetersPerPixel: 1,
      ActionsWithNoLocation: [],
    },
  }),
};
