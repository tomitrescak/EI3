

import { Formix, Input } from '../../Form';
import { ComponentDao } from '../experiment_model';
import { agentEnvironmentEditor } from './agentEnvironmentEditor';
import { ExperimentPane, UniqueId } from './experimentCommon';

type EnvironmentObjectDao = ComponentDao & {
  Icon: string;
  Actions: Array<{
    Id: string;
    Parameters: Array<{
      Name: string;
      Value: string;
    }>;
  }>;
};

const Editor = ({ component }: { component: EnvironmentObjectDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane style={{ paddingBottom: 2 }}>
      <Input name="Icon" size="mini" label="Icon" fluid />
    </ExperimentPane>
  </Formix>
);

export const environmentalObjectEditor = {
  text: "Environmental Object",
  globalDependencies: [agentEnvironmentEditor],
  componentDependencies: [],
  type: "Ei.Simulation.Behaviours.Environment.Objects.EnvironmentObject, Ei.Simulation",
  editor: Editor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: environmentalObjectEditor.type,
    Icon: "",
    Actions: [],
  }),
};
