import { ComponentDao } from "../experiment_model";
import { UniqueId } from "./experimentCommon";

export const Editor = ({ component }: { component: ComponentDao }) => null;

export const randomDecisionReasoner = {
  text: "Random Decision Reasoner",
  type: "Ei.Simulation.Behaviours.Reasoners.RandomDecisionReasoner, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: randomDecisionReasoner.type,
  }),
};
