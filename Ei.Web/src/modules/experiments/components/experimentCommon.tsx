import styled from "@emotion/styled";

export const ExperimentPane = styled.div`
  padding: 0px 8px;
  padding-top: 3px !important;
  background: #efefef;

  fieldset {
    border: 1px solid #bbb;
    background-color: #ddd;
    border-radius: 6px;
    margin: 4px 0px;
    padding: 4px;
    overflow: hidden;
    width: 100%auto;
  }

  th {
    text-align: left;
  }

  legend {
    background-color: #efefef;
    padding: 2px 8px;
    border-radius: 6px;
    font-weight: bold;
  }

  td {
    vertical-align: top;
  }

  .inline {
    display: flex;

    .field {
      flex: 1;
      label {
        min-width: 10px;
        width: inherit;
      }

      > div {
        flex: 1;
      }
    }

    label {
      padding: 0px 4px;
    }

    .field:first-of-type {
      label {
        padding-left: 0px;
      }
    }
  }

  .field {
    margin-bottom: 0px !important;
    padding-bottom: 3px !important;
    display: flex;
    align-items: center;

    > div {
      flex: 1;
      width: 100%;
      min-width: 60px;
    }

    label {
      min-width: 10px;
      width: 100px;
      overflow: hidden;
      text-overflow: ellipsis;

      :after {
        content: ":";
      }
    }
  }

  label: ExperimentPane;
`;

let id = 0;
export const UniqueId = () => (Date.now() + id++).toString();

export type LogMessage = {
  agent: string;
  code: string;
  component: string;
  level: number;
  message: string;
  parameters: any;
};
