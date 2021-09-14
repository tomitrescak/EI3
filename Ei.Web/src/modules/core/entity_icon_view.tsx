import * as React from "react";

import { observer } from "mobx-react";

import { Entity } from "../ei/entity_model";
import styled from "@emotion/styled";

interface Props {
  entity: Entity;
}

const Icon = styled.span`
  padding-right: 8px;
`;

export const IconView = observer(({ entity }: Props) => (
  <Icon>{entity.Icon}</Icon>
));
