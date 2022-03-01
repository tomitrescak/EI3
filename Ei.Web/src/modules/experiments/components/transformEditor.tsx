import styled from "@emotion/styled";
import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { ExperimentPane, UniqueId } from "./experimentCommon";

// const Line = styled.div`
//   display: flex;
//   align-items: center;
//   padding: 2px 8px;
//   background: #efefef;
// `;

export const TransformEditor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <ExperimentPane>
      <div className="inline">
        <Input size="mini" name="position.x" label="X" fluid />
        <Input size="mini" name="position.y" label="Y" fluid />
        <Input size="mini" name="position.z" label="Z" fluid />
      </div>
    </ExperimentPane>
  </Formix>
);

export const transformComponent = {
  text: "Transform",
  type: "UnityEngine.Transform, Ei.Integration.Unity",
  editor: TransformEditor,
  defaultValue: () => ({
    Id: UniqueId(),
    $type: transformComponent.type,
    position: {
      x: "0",
      y: "0",
      z: "0",
    },
  }),
};
