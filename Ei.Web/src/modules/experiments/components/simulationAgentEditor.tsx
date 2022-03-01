import { Observer } from "mobx-react";
import React from "react";
import { Button } from "semantic-ui-react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";
import { timerComponent } from "./timerEditor";

type SimulationAgentDao = ComponentDao & {
  ReasoningIntervalInMs: number;
  Groups: string[][];
};

const Editor = ({ component }: { component: SimulationAgentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input
        name="ReasoningIntervalInMs"
        type="number"
        size="mini"
        label="Reasoning Interval In Ms"
        fluid
      />

      <fieldset>
        <legend>
          Groups
          <Button
            icon="plus"
            size="mini"
            color="green"
            basic
            style={{ marginLeft: 8 }}
            compact
            onClick={() => component.Groups.push(["", ""])}
          />
        </legend>
        <table style={{ width: "100%" }}>
          <thead>
            <tr>
              <th style={{ width: "50%" }}>Organisation</th>
              <th style={{ width: "50%" }}>Role</th>
            </tr>
          </thead>
          <tbody>
            <Observer>
              {() => (
                <>
                  {component.Groups.map((a, i) => {
                    return (
                      <tr>
                        <td>
                          <Input name={`Groups.${i}.0`} size="mini" fluid />
                        </td>
                        <td>
                          <Input name={`Groups.${i}.1`} size="mini" fluid />
                        </td>
                        <td>
                          <Button
                            icon="minus"
                            basic
                            style={{ height: 30 }}
                            color="red"
                            compact
                            size="mini"
                            onClick={() => component.Groups.splice(i, 1)}
                          />
                        </td>
                      </tr>
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

export const simulationAgentComponent = {
  text: "Simulation Agent",
  globalDependencies: [],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.SimulationAgent, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: simulationAgentComponent.type,
    ReasoningIntervalInMs: 100,
    Groups: [],
  }),
};
