import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";

export const Editor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input
        size="mini"
        type="number"
        name="SpeedKmH"
        step="0.5"
        label="Speed Km/H"
        fluid
      />
    </ExperimentPane>
  </Formix>
);

export const linearNavigationEditor = {
  text: "Linear Navigation",
  type: "Ei.Simulation.Behaviours.LinearNavigation, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: linearNavigationEditor.type,
    SpeedKmH: 6,
  }),
};
