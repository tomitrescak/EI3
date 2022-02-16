import styled from "@emotion/styled";

export const ExperimentPane = styled.div`
  padding: 4px 8px;
  background: #efefef;
`;

let id = 0;
export const UniqueId = () => (Date.now() + id++).toString();
