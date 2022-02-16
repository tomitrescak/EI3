import React from "react";

import { Accordion, Loader, Menu } from "semantic-ui-react";

import { observer } from "mobx-react";
import styled from "@emotion/styled";
import { AuthorisationList } from "../authorisations/authorisation_list";
import { HierarchicEntityView } from "./hierarchic_entity_view";
import { WorkflowList } from "./workflow_list_view";
import { Link, useHistory, useParams } from "react-router-dom";
import { useAppContext } from "../../config/context";
import { ExperimentList } from "../experiments/experiment_list";
import { useQuery } from "../../helpers/client_helpers";

const ComponentsType = styled.div`
  height: 100%;
  overflow: auto;
  padding-bottom: 40px;
`;

export const Components = observer(() => {
  const history = useHistory();
  const context = useAppContext();
  // const handler = React.useMemo(
  //   () => context.createAccordionHandler("root"),
  //   []
  // );
  const [activeIndex, setActiveIndex] = React.useState(
    localStorage.getItem("list.top")
      ? parseInt(localStorage.getItem("list.top"))
      : 0
  );
  const ei = context.ei;
  const store = context;

  const { eiName } = useParams<{ eiName: string }>();
  const { ei: eiId } = useQuery<{ ei: string }>();

  const compile = () => {
    context.ei.compile(context.client);
  };

  function titleProps(index: number) {
    return {
      active: index === activeIndex,
      index,
      handleClick() {
        localStorage.setItem("list.top", index.toString());
        setActiveIndex(index);
      },
    };
  }

  return (
    <>
      <Menu
        inverted
        attached="top"
        color="blue"
        style={{ borderRadius: "0px" }}
      >
        <Menu.Item>
          <Link to={`/ei/${eiName}?ei=${eiId}`}>{ei.Name}</Link>
        </Menu.Item>
        <Menu.Menu position="right">
          {/* <Menu.Item
            icon="play"
            title="Compile and Run the Institution"
            onClick={() => {
              context.ei.run(context.client);
              history.push(`/ei/${ei.Name.toUrlName()}/execution?ei=${ei.Id}`);
            }}
          /> */}
          {/* <Menu.Item
            icon="reply"
            onClick={context.Ui.history.undo}
            title="Undo"
          />
          <Menu.Item
            icon="mail forward"
            onClick={context.Ui.history.redo}
            title="Redo"
          /> */}
          {store.compiling ? (
            <Menu.Item title="Compiling">
              <Loader active inline size="tiny" />
            </Menu.Item>
          ) : (
            <Menu.Item icon="cogs" onClick={compile} title="Compile Solution" />
          )}
          <Menu.Item icon="save" onClick={ei.save} />
        </Menu.Menu>
      </Menu>
      <ComponentsType>
        <Accordion>
          <HierarchicEntityView
            {...titleProps(0)}
            collection={ei.Roles}
            createEntity={ei.createRole}
            url="roles"
            title="Roles"
            ei={ei}
          />

          <HierarchicEntityView
            {...titleProps(1)}
            collection={ei.Organisations}
            createEntity={ei.createOrganisation}
            url="organisations"
            title="Organisations"
            ei={ei}
          />

          <HierarchicEntityView
            {...titleProps(2)}
            collection={ei.Types}
            createEntity={ei.createType}
            url="types"
            title="Types"
            ei={ei}
          />

          <WorkflowList {...titleProps(3)} ei={ei} />

          <AuthorisationList {...titleProps(4)} ei={ei} />

          <ExperimentList {...titleProps(5)} ei={ei} />
        </Accordion>
      </ComponentsType>
    </>
  );
});
