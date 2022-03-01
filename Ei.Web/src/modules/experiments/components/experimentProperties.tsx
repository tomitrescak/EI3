import React from "react";
import { Input, Select } from "../../Form";
import { ExperimentPane } from "./experimentCommon";
import { iconOptions } from "../icon_list";
import styled from "@emotion/styled";

const IconSelect = styled(Select)`
  .text {
    width: 100%;
  }
`;

export const ExperimentProperties = () => (
  <ExperimentPane>
    <Input name="Name" label="Name" />

    <IconSelect
      name="Icon"
      label="Icon"
      options={iconOptions}
      selection
      search
    />
  </ExperimentPane>
);
