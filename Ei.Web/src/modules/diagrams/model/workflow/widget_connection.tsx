import { ActionDisplayType, Connection } from "../../../ei/connection_model";
import { Workflow } from "../../../ei/workflow_model";
import { ActionView } from "./widget_action";

const ActionWidget = () => {};

export const ConnectionWidget = ({
  connection,
  workflow,
  svgRef,
}: {
  connection: Connection;
  workflow: Workflow;
  svgRef: React.MutableRefObject<SVGSVGElement>;
}) => {
  // const location = useLocation();
  // const link = props.link as Connection;
  // const selected = link.url === window.location.pathname;

  return (
    <>
      <ActionView link={connection} workflow={workflow} svgRef={svgRef} />
    </>
  );
};
