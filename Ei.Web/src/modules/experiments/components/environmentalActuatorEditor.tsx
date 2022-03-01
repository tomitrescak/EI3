import { ComponentDao } from "../experiment_model";
import { UniqueId } from "./experimentCommon";

export const Editor = ({ component }: { component: ComponentDao }) => null;

export const environmentalActuator = {
  text: "Environmental Actuator",
  type: "Ei.Simulation.Behaviours.Actuators.EnvironmentalActuator, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: environmentalActuator.type,
  }),
};
