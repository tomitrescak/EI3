import { Observer } from 'mobx-react';
import { Button, Dropdown } from 'semantic-ui-react';

import { Formix, Input } from '../../Form';
import { ComponentEditor } from '../componentEditor';
import { ComponentDao, ExperimentDao, GameObjectDao } from '../experiment_model';
import { agentEnvironmentEditor } from './agentEnvironmentEditor';
import { delayActionEditor } from './delayActionEditor';
import { ExperimentPane, UniqueId } from './experimentCommon';

export type ActionsProviderDao = ComponentDao & {
  Actions: Array<{
    Id: string;
    Parameters: Array<{
      Name: string;
      Value: string;
    }>;
    Plan: ComponentDao[];
  }>;
};

export type ComponentOption = {
  text: string;
  globalDependencies?: ComponentOption[];
  componentDependencies?: ComponentOption[];
  type: string;
  editor: React.FC<any>;
  defaultValue: () => any;
};

const componentOptions: ComponentOption[] = [delayActionEditor].sort((a, b) =>
  a.text.localeCompare(b.text)
);

const Editor = ({
  component,
  experiment,
  gameObject,
}: {
  component: ActionsProviderDao;
  experiment: ExperimentDao;
  gameObject: GameObjectDao;
}) => (
  <Formix initialValues={component}>
    <ExperimentPane style={{ paddingBottom: 2 }}>
      <fieldset>
        <legend>
          Actions
          <Button
            icon="plus"
            size="mini"
            color="green"
            basic
            compact
            style={{ marginLeft: 4 }}
            onClick={() =>
              component.Actions.push({
                Id: "",
                Parameters: [],
                Plan: [],
              })
            }
          />
        </legend>
        <Observer>
          {() => (
            <>
              {component.Actions.map((a, i) => {
                if (a.Parameters == null) {
                  a.Parameters = [];
                }
                return (
                  <div key={i} style={{ borderBottom: "dashed 1px #ccc" }}>
                    <Formix key={i} initialValues={a}>
                      <table style={{ width: "100%" }}>
                        <tbody>
                          <tr>
                            <th style={{ width: "30%" }}>Id</th>
                            <th>Parameters</th>
                          </tr>
                          <tr>
                            <td>
                              <Input name="Id" size="mini" fluid />
                            </td>

                            <td>
                              <div style={{ display: "flex" }}>
                                <div style={{ flex: 1 }}>
                                  {a.Parameters.length == 0 && (
                                    <Input
                                      name="Nothing"
                                      size="mini"
                                      fluid
                                      style={{ marginRight: 4 }}
                                      disabled
                                    />
                                  )}
                                  <Observer>
                                    {() => (
                                      <>
                                        {a.Parameters?.map((p, pi) => (
                                          <div key={pi} className="inline">
                                            <Input
                                              name={`Parameters.${pi}.Name`}
                                              placeholder="Name"
                                              size="mini"
                                              fluid
                                            />
                                            <Input
                                              name={`Parameters.${pi}.Value`}
                                              placeholder="Value"
                                              size="mini"
                                              fluid
                                            />
                                            <Button
                                              icon="minus"
                                              size="mini"
                                              style={{
                                                height: 30,
                                                marginLeft: 1,
                                              }}
                                              basic
                                              color="red"
                                              compact
                                              onClick={() =>
                                                a.Parameters.splice(pi, 1)
                                              }
                                            />
                                          </div>
                                        )) || null}
                                      </>
                                    )}
                                  </Observer>
                                </div>
                                <Button
                                  icon="plus"
                                  size="mini"
                                  style={{
                                    height: 30,
                                    marginLeft: 1,
                                  }}
                                  basic
                                  compact
                                  color="green"
                                  onClick={() =>
                                    a.Parameters.push({
                                      Name: "",
                                      Value: "",
                                    })
                                  }
                                />
                              </div>
                            </td>
                          </tr>

                          <tr>
                            <td colSpan={2}>
                              <fieldset>
                                <legend>Plan</legend>

                                {a.Plan.map((p, i) => {
                                  const currentComponent =
                                    componentOptions.find(
                                      (o) => o.type === p.$type
                                    );

                                  return (
                                    <ComponentEditor
                                      owner={a.Plan}
                                      reorder={true}
                                      index={i}
                                      componentDefinition={p}
                                      currentComponent={currentComponent}
                                      gameObject={gameObject}
                                      experiment={experiment}
                                    />
                                  );
                                })}

                                <Dropdown
                                  text="Add Action"
                                  compact
                                  value=""
                                  fluid
                                  selection
                                  style={{ marginTop: 4 }}
                                  options={[{ text: "None", value: "" }].concat(
                                    componentOptions.map((c) => ({
                                      value: c.type,
                                      text: c.text,
                                    }))
                                  )}
                                  onChange={(e, v) => {
                                    if (!v.value) {
                                      return;
                                    }
                                    const cmp = componentOptions.find(
                                      (c) => c.type === v.value
                                    );

                                    a.Plan.push(cmp.defaultValue());
                                  }}
                                />
                              </fieldset>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </Formix>
                  </div>
                );
              })}
            </>
          )}
        </Observer>
      </fieldset>
    </ExperimentPane>
  </Formix>
);

export const actionsProviderEditor = {
  text: "Actions Provider",
  globalDependencies: [agentEnvironmentEditor],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.Environment.ActionsProvider, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: actionsProviderEditor.type,
    Icon: "",
    Actions: [],
  }),
};
