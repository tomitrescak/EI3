import React from "react";
import { observer } from "mobx-react";
import { Message } from "semantic-ui-react";

import { useFormixContext } from "./library/formContext";

type Props = {
  title?: string;
};

// @todo tests
export const FormErrors = observer(
  ({ title = "Please correct the following errors" }: Props) => {
    // iOS needs an "action" attribute for nice input: https://stackoverflow.com/a/39485162/406725
    // We default the action to "#" in case the preventDefault fails (just updates the URL hash)
    const { state } = useFormixContext();
    if (state.errors && state.errors.length) {
      return (
        <Message
          icon="warning sign"
          negative
          header={title}
          list={state.errors}
        />
      );
    }
    return null;
  }
);
