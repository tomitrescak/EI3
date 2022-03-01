import { Observer } from "mobx-react";
import React from "react";
import { Button } from "semantic-ui-react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { agentEnvironmentEditor } from "./agentEnvironmentEditor";
import { ExperimentPane, UniqueId } from "./experimentCommon";

type EnvironmentObjectDao = ComponentDao & {
  Icon: string;
  Actions: Array<{
    Id: string;
    Parameters: Array<{
      Name: string;
      Value: string;
    }>;
  }>;
};

const Editor = ({ component }: { component: EnvironmentObjectDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane style={{ paddingBottom: 2 }}>
      <Input name="Icon" size="mini" label="Icon" fluid />

      <fieldset>
        <legend>
          Actions
          <Button
            icon="plus"
            size="mini"
            color="green"
            basic
            compact
            style={{ marginLeft: 4 }}
            onClick={() =>
              component.Actions.push({
                Id: "",
                Parameters: [],
              })
            }
          />
        </legend>
        <table style={{ width: "100%" }}>
          <thead>
            <tr>
              <th style={{ width: "30%" }}>Id</th>
              <th>Parameters</th>
            </tr>
          </thead>
          <tbody>
            <Observer>
              {() => (
                <>
                  {component.Actions.map((a, i) => {
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
                            <div style={{ display: "flex" }}>
                              <div style={{ flex: 1 }}>
                                <Observer>
                                  {() => (
                                    <>
                                      {a.Parameters?.map((p, pi) => (
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
                                            style={{
                                              height: 30,
                                              marginLeft: 1,
                                            }}
                                            basic
                                            color="red"
                                            compact
                                            onClick={() =>
                                              a.Parameters.splice(pi, 1)
                                            }
                                          />
                                        </div>
                                      )) || null}
                                    </>
                                  )}
                                </Observer>
                              </div>
                              <Button
                                icon="plus"
                                size="mini"
                                style={{ height: 30, marginLeft: 1 }}
                                basic
                                compact
                                color="green"
                                onClick={() =>
                                  a.Parameters.push({
                                    Name: "",
                                    Value: "",
                                  })
                                }
                              />
                            </div>
                          </td>
                        </tr>
                      </Formix>
                    );
                  })}
                </>
              )}
            </Observer>
          </tbody>
        </table>
      </fieldset>
    </ExperimentPane>
  </Formix>
);

export const environmentalObjectEditor = {
  text: "Environmental Object",
  globalDependencies: [agentEnvironmentEditor],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.Environment.Objects.EnvironmentObject, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: environmentalObjectEditor.type,
    Icon: "",
    Actions: [],
  }),
};
