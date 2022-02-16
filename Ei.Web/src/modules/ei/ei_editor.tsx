import React from "react";

import { observer } from "mobx-react";
import { Header, Message } from "semantic-ui-react";

import { EntityEditor } from "../core/entity_view";
import { CodeEditor } from "../core/monaco_editor";
import { PropertyView } from "../properties/property_view";
import styled from "@emotion/styled";
import { useAppContext } from "../../config/context";
import { Form, Formix, Select } from "../Form";

const EditorForm = styled.div`
  padding: 12px;
  width: 100%;
  label: EditorForm;
`;

export const EiEditor = observer(() => {
  const context = useAppContext();
  const update = (value: any) => {
    context.ei.Expressions = value;
  };

  const value = () => context.ei.Expressions;

  let ei = context.ei;

  if (!ei) {
    return <Message content="Deleted" />;
  }

  return (
    <div className="ui form">
      <Formix initialValues={ei}>
        <EditorForm>
          <EntityEditor entity={ei} />

          <Select
            label="Main Workflow"
            options={ei.workflowOptions}
            name={"MainWorkflow"}
          />

          <Header as="h5" content="Expressions" dividing icon="code" />

          <CodeEditor update={update} value={value} i={ei.Properties} />

          <PropertyView owner={ei} types={ei.types} />
        </EditorForm>
      </Formix>
    </div>
  );
});
