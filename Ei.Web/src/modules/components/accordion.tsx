import styled from "@emotion/styled";
import { Accordion } from "semantic-ui-react";

export const AccordionContent = styled(Accordion.Content)`
  // background: '#dedede',
  padding: 6px 6px 6px 25px !important;

  .item {
    border-radius: 3px;
    padding: 4px !important;
  }
  &.active {
    background: #dddddd !important;
  }
`;

export const AccordionTitle = styled(Accordion.Title)`
  border-top: solid 1px rgba(34, 36, 38, 0.15);
  padding-left: 8px !important;

  &.active {
    background: #dddddd !important;
  }
`;
