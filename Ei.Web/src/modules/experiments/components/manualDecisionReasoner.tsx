import { observable } from 'mobx';
import { Observer } from 'mobx-react';
import { Button, Input, Message } from 'semantic-ui-react';

import styled from '@emotion/styled';

import { useAppContext } from '../../../config/context';
import { Property } from '../../ei/property_model';
import { ComponentDao, ExperimentDao, GameObjectDao } from '../experiment_model';
import { ActionsProviderDao, actionsProviderEditor } from './actionsProviderEditor';
import { ExperimentPane, UniqueId } from './experimentCommon';

const Table = styled.table`
  th,
  td {
    padding: 2px;
  }
`;

export const Editor = ({
  component,
  gameObject,
  experiment,
}: {
  component: ComponentDao;
  gameObject: GameObjectDao;
  experiment: ExperimentDao;
}) => {
  const context = useAppContext();
  if (context.agentConnections[gameObject.Id] == null) {
    context.agentConnections[gameObject.Id] = [];
  }

  return (
    <Observer>
      {() => {
        let params = context.agentConnections[gameObject.Id];
        if (params.length === 0) {
          return <Message content="0 possible actions" />;
        }
        let values: Array<{
          workflow: string;
          connection: string;
          action: string;
          parameters: Property[];
        }> = [];
        for (let i = 0; i < params.length / 3; i++) {
          values.push({
            workflow: params[i * 2],
            connection: params[i * 2 + 1],
            action: params[i * 2 + 2],
            parameters: context.ei.Workflows.find(
              (w) => w.Id === params[i * 2]
            ).Actions.find((a) => a.Id === params[i * 2 + 2]).Properties,
          });
        }
        return (
          <ExperimentPane>
            <Table style={{ width: "100%" }}>
              <thead>
                <tr>
                  <th>Conn.</th>
                  <th>Action.</th>
                  <th>Type</th>
                  <th style={{ width: "100%" }}>Parameters</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {values.map((v, i) => {
                  let state = observable({ type: "ui" });
                  let parameterValues = observable(v.parameters.map((p) => ""));
                  let parameterNames = observable(
                    v.parameters.map((p) => p.Name)
                  );
                  let objects = experiment.GameObjects.filter((g) =>
                    (
                      g.Components.find(
                        (c) => c.$type === actionsProviderEditor.type
                      ) as ActionsProviderDao
                    )?.Actions.some((a) => a.Id === v.action)
                  );
                  if (objects.length > 0) {
                    parameterValues[0] = objects[0].Name;
                  }
                  return (
                    <Observer>
                      {() => (
                        <tr key={i}>
                          <td>{v.connection}</td>
                          <td>{v.action}</td>
                          <td style={{ padding: 0 }}>
                            <select
                              onChange={(e) =>
                                (state.type = e.currentTarget.value)
                              }
                              value={state.type}
                              style={{ padding: 2, height: 24, width: 40 }}
                            >
                              <option value="ui" selected>
                                UI
                              </option>
                              <option value="ei">EI</option>
                            </select>
                          </td>
                          <td style={{ padding: "0px 4px" }}>
                            {state.type === "ei" ? (
                              <>
                                {v.parameters.map((p, pi) => (
                                  <Input
                                    value={parameterValues[pi]}
                                    placeholder={`${p.Name} <${p.Type}>`}
                                    fluid
                                    onChange={(_, v) =>
                                      (parameterValues[pi] = v.value)
                                    }
                                  />
                                ))}
                              </>
                            ) : (
                              <select
                                style={{ padding: 2, height: 24 }}
                                onChange={(e) =>
                                  (parameterValues[0] = e.currentTarget.value)
                                }
                              >
                                {objects.map((s, i) => (
                                  <option key={i} value={s.Name}>
                                    {s.Name}
                                  </option>
                                ))}
                              </select>
                            )}
                          </td>
                          <td colSpan={3} style={{ padding: "0px 4px" }}>
                            <Button
                              size="tiny"
                              compact
                              color="blue"
                              content="Run"
                              style={{ height: 24 }}
                              onClick={() => {
                                context.client.send({
                                  query:
                                    state.type == "ei"
                                      ? "AgentAction"
                                      : "UiAction",
                                  variables: [
                                    gameObject.Name,
                                    v.workflow,
                                    v.connection,
                                    v.action,
                                    parameterNames,
                                    parameterValues,
                                  ],
                                });
                              }}
                            />
                          </td>
                        </tr>
                      )}
                    </Observer>
                  );
                })}
              </tbody>
            </Table>
          </ExperimentPane>
        );
      }}
    </Observer>
  );
};

export const manualDecisionReasoner = {
  text: "Manual Decision Reasoner",
  type: "Ei.Simulation.Behaviours.Reasoners.ManualDecisionReasoner, Ei.Simulation",
  editor: Editor,

  defaultValue: () => ({
    Id: UniqueId(),
    $type: manualDecisionReasoner.type,
  }),
};
