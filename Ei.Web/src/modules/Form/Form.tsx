import React from "react";
import { Form as SuiForm } from "semantic-ui-react";

import { useFormixContext } from "./library/formContext";

export type FormikFormProps = Pick<
  React.FormHTMLAttributes<HTMLFormElement>,
  Exclude<
    keyof React.FormHTMLAttributes<HTMLFormElement>,
    "onReset" | "onSubmit"
  >
> & { keyboardSave?: string; loading?: boolean };

type FormProps = React.ComponentPropsWithoutRef<"form"> & {
  keyboardSave?: string;
  loading?: boolean;
};

// @todo tests
export const Form = React.forwardRef<HTMLFormElement, FormProps>(
  (props: FormikFormProps, ref) => {
    // iOS needs an "action" attribute for nice input: https://stackoverflow.com/a/39485162/406725
    // We default the action to "#" in case the preventDefault fails (just updates the URL hash)
    const { action, ...rest } = props;
    const _action = action || "#";
    const { handleReset, handleSubmit } = useFormixContext();
    return (
      <SuiForm
        onSubmit={handleSubmit}
        ref={ref}
        onReset={handleReset}
        action={_action}
        {...rest}
      />
    );
  }
);

Form.displayName = "Form";
