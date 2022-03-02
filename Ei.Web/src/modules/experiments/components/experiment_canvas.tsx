import { Observer } from "mobx-react";
import { Message } from "semantic-ui-react";
import { Container } from "../../diagrams/diagram_view";
import type { ExperimentDao, GameObjectDao } from "../experiment_model";
import {
  agentEnvironmentEditor,
  AgentEnvironment,
} from "./agentEnvironmentEditor";

export const ExperimentCanvas = ({
  experiment,
  state,
}: {
  experiment: ExperimentDao;
  state: {
    select(go: GameObjectDao): void;
    selectedGameObject: GameObjectDao;
  };
}) => {
  // find environment definition
  let go = experiment.GameObjects.find((g) =>
    g.Components.some((c) => c.$type === agentEnvironmentEditor.type)
  );
  if (go == null) {
    return (
      <Message
        icon="warning"
        warning
        header="Please add 'Agent Environment' component to your project"
      />
    );
  }
  let env = go.Components.find(
    (c) => c.$type === agentEnvironmentEditor.type
  ) as AgentEnvironment;

  return (
    <Container background={"rgb(60, 60, 60)"} color={"rgba(255,255,255, 0.05)"}>
      <Observer>
        {() => (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox={`0 0 ${env.Definition.Width} ${env.Definition.Height}`}
            width="100%"
            height="100%"
          >
            {experiment.GameObjects.map((go, i) => (
              <Observer key={go.Id}>
                {() => (
                  <image
                    style={{
                      cursor: "pointer",
                      transition: "all 0.1s",
                      outline:
                        state.selectedGameObject === go
                          ? "dashed 4px salmon"
                          : undefined,
                      filter:
                        state.selectedGameObject === go
                          ? "invert(100%)"
                          : "invert(70%)",
                    }}
                    href={go.Icon ? `/icons/${go.Icon}` : `icons/cube.svg`}
                    x={go.Components[0].position.x}
                    y={go.Components[0].position.y}
                    color="white"
                    height="32"
                    width="32"
                    onClick={() => state.select(go)}
                  />
                )}
              </Observer>
            ))}
          </svg>
        )}
      </Observer>
    </Container>
  );
};
