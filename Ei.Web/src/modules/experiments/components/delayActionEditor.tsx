import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";

export const Editor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <Input size="mini" type="number" name="Index" label="Order" fluid />
      <Input
        size="mini"
        type="number"
        name="DelayMs"
        label="Delay (Ms)"
        fluid
      />
    </ExperimentPane>
  </Formix>
);

export const delayActionEditor = {
  text: "Delay Action",
  type: "Ei.Simulation.Behaviours.Environment.Objects.DelayAction, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: delayActionEditor.type,
    Index: 0,
    DelayMs: 0,
  }),
};
