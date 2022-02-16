import styled from "@emotion/styled";
import React from "react";
import { Formix, Input } from "../../Form";
import { ComponentDao } from "../experiment_model";
import { UniqueId } from "./experimentCommon";

const Line = styled.div`
  display: flex;
  align-items: center;
  padding: 2px 8px;
  background: #efefef;
  .field {
    width: 60px;
    margin-right: 30px !important;
    margin-bottom: 0px !important;

    :last-of-type {
      margin-right: 0px;
    }
  }
`;

export const TransformEditor = ({ component }: { component: ComponentDao }) => (
  <Formix initialValues={component}>
    <Line>
      <Input type="number" name="position.x" label="X" fluid />
      <Input type="number" name="position.y" label="Y" />
      <Input type="number" name="position.z" label="Z" />
    </Line>
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
      x: 0,
      y: 0,
      z: 0,
    },
  }),
};
