import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";

export const TimerEditor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input
        type="number"
        name="DayLengthInSeconds"
        label="Day Length in Seconds"
        fluid
      />
    </ExperimentPane>
  </Formix>
);

export const timerComponent = {
  text: "Timer",
  type: "Ei.Simulation.Behaviours.SimulationTimer, Ei.Simulation",
  editor: TimerEditor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: timerComponent.type,
    DayLengthInSeconds: 120,
  }),
};
