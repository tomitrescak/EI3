import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";
import { timerComponent } from "./timerEditor";

const Editor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input name="Organisation" size="mini" label="Organisation" fluid />
      <Input name="Password" size="mini" label="Password" fluid />
    </ExperimentPane>
  </Formix>
);

export const simulationProjectComponent = {
  text: "Simulation Project",
  globalDependencies: [timerComponent],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.SimulationProject, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: simulationProjectComponent.type,
    Organisation: "",
    Password: null,
  }),
};
