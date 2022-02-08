import styled from "@emotion/styled";
import { Accordion } from "semantic-ui-react";

export const AccordionContent = styled(Accordion.Content)`
  // background: '#dedede',
  padding: 6px 6px 6px 25px !important;

  .item {
    border-radius: 4px;
    padding: 4px !important;
  }
  &.active {
    background: #ddd !important;
  }

  &.secondary,
  &.active.secondary {
    background: #ccc !important;
  }
`;

export const AccordionTitle = styled(Accordion.Title)`
  border-top: solid 1px rgba(34, 36, 38, 0.15);
  padding: 8px 8px !important;

  &.active {
    background: #dddddd !important;
  }

  :hover {
    background: #d0d0d0 !important;
  }
`;

export const SecondaryAccordionTitle = styled(Accordion.Title)`
  border-top: solid 1px rgba(34, 36, 38, 0.15);
  border-bottom: solid 1px rgba(34, 36, 38, 0.15);
  padding-left: 24px !important;

  &.active {
    background: #c0c0c0 !important;
  }

  :hover {
    background: #c9c9c9 !important;
  }
`;
