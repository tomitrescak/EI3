import { ComponentDao } from "../experiment_model";
import { UniqueId } from "./experimentCommon";

export const Editor = ({ component }: { component: ComponentDao }) => null;

export const sensorEditor = {
  text: "Sensor",
  type: "Ei.Simulation.Behaviours.Sensors.Sensor, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: sensorEditor.type,
  }),
};
