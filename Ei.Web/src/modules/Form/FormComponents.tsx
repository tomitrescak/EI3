import parse from "date-fns/parse";
import parseISO from "date-fns/parseISO";
import { Observer, observer } from "mobx-react";
import React from "react";
import DatePickerView from "react-datepicker";
import {
  Checkbox as SuiCheckbox,
  CheckboxProps,
  Dropdown,
  DropdownItemProps,
  DropdownProps,
  Form as SuiForm,
  FormFieldProps,
  Icon,
  Input as SuiInput,
  InputProps,
  Radio as SuiRadio,
  RadioProps,
  StrictFormFieldProps,
  TextArea as SuiTextArea,
  TextAreaProps,
} from "semantic-ui-react";
import NumberInput from "semantic-ui-react-numberinput";

import styled from "@emotion/styled";

import { useField } from "./library/formHooks";
import { isNumber } from "./library/validation";

import type { ReactDatePickerProps } from "react-datepicker";
export { Form as SUIForm } from "semantic-ui-react";
// import 'react-datepicker/src/stylesheets/datepicker.scss';

const ErrorMessage = styled.div`
  margin-top: 8px !important;
  margin-bottom: 2px !important;
  margin-left: 0px;
  color: #9f3a38;
  font-size: smaller;
`;

export const DropdownGroup = styled.div`
  display: flex;
  align-items: center;

  .searchLabel {
    border-top-right-radius: 0px !important;
    border-bottom-right-radius: 0px !important;
    margin-right: 0px !important;
    height: 38px;
  }
  .searchControl,
  .searchControl div:first-of-type,
  input {
    border-top-left-radius: 0px !important;
    border-bottom-left-radius: 0px !important;
    margin-left: 0px !important;
    flex: 1;
  }
  .field {
    margin-bottom: 0px !important;
  }
`;

export const ErrorLabel = ({ message }: { message: string }) => (
  <ErrorMessage color="danger">
    <Icon name="warning sign" color="red" />
    {message}
  </ErrorMessage>
);

export const Input = observer(
  ({
    label,
    width,
    style,
    className,
    inputLabel,
    ...props
  }: InputProps & { name: string; inputLabel?: string | React.ReactNode }) => {
    const [field, meta] = useField(props);
    return (
      <SuiForm.Field
        error={meta.touched && meta.error ? true : false}
        width={width}
        style={style}
        className={className}
      >
        {label && <label htmlFor={props.id || props.name}>{label}</label>}
        {props.type === "number" ? (
          <>
            <Observer>
              {() => (
                <NumberInput
                  buttonPlacement="right"
                  label={inputLabel}
                  id={props.id || props.name}
                  allowEmptyValue
                  {...field}
                  {...props}
                  value={(field.value == null ? "" : field.value).toString()}
                  onChange={(value: Any) => {
                    field.onChange({
                      target: {
                        value: value,
                        name: field.name,
                        type: "number",
                      },
                    } as Any);
                  }}
                />
              )}
            </Observer>
          </>
        ) : (
          <>
            <SuiInput
              label={inputLabel}
              id={props.id || props.name}
              {...field}
              {...props}
              value={field.value || ""}
            />
          </>
        )}

        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </SuiForm.Field>
    );
  }
);

export const TextArea = observer(
  ({ label, width, className, ...props }: TextAreaProps & { name: string }) => {
    const [field, meta] = useField(props);

    return (
      <SuiForm.Field
        error={meta.touched && meta.error ? true : false}
        width={width}
        className={className}
      >
        {label && <label htmlFor={props.id || props.name}>{label}</label>}
        <SuiTextArea id={props.id || props.name} {...field} {...props} />
        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </SuiForm.Field>
    );
  }
);

const DateControl = React.forwardRef(({ clear, ...rest }: Any, ref) => {
  const clearInput = (e: React.KeyboardEvent<any>) => {
    if (e.keyCode === 8 || e.keyCode === 46) {
      clear();
    }
  };

  return (
    <SuiInput
      id={rest.id || rest.name}
      ref={ref}
      {...rest}
      icon="calendar"
      readOnly={true}
      onKeyDown={clearInput}
    />
  );
});

const DatePickerField = styled(SuiForm.Field)`
  .react-datepicker-wrapper {
    width: 100%;
  }
`;

function testNumber(value: Any): boolean {
  return !isNumber(value);
}

export const DatePicker = observer(
  ({
    width,
    label,
    className,
    format = "dd MMM yyyy",
    inputFormat,
    ...props
  }: Omit<ReactDatePickerProps, "onChange"> & {
    width?: StrictFormFieldProps["width"];
    label?: string;
    className?: string;
    format?: string;
    id?: string;
    name?: string;
    placeholder?: string;
    inputFormat?: "iso";
  }) => {
    const [field, meta] = useField(props);

    function clear() {
      field.onChange({
        target: { name: props.name, value: null },
      } as Any);
    }

    return (
      <DatePickerField
        error={meta.touched && meta.error ? true : false}
        width={width}
        className={className}
      >
        <label htmlFor={props.id || props.name}>{label}</label>
        <DatePickerView
          {...props}
          customInput={<DateControl clear={clear} />}
          onChange={(date: Date) => {
            field.onChange({
              target: {
                name: props.name,
                value: inputFormat === "iso" ? date : date.toISOString(),
              },
            } as Any);
          }}
          dateFormat={format}
          selected={
            field.value
              ? field.value instanceof Date
                ? field.value
                : isNumber(field.value)
                ? new Date(field.value)
                : inputFormat === "iso"
                ? parseISO(field.value)
                : parse(field.value, format, new Date())
              : null
          }
        />
        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </DatePickerField>
    );
  }
);

export interface LabelProps {
  label: string;
}
export const Label = (props: FormFieldProps & LabelProps) => {
  const { children, ...rest } = props;
  return (
    <SuiForm.Field {...rest}>
      <label>{props.label}</label>
    </SuiForm.Field>
  );
};

export const FieldError = observer(({ name }: { name: string }) => {
  const [_, meta] = useField(name);
  return meta.touched && meta.error ? (
    <ErrorLabel message={meta.error} />
  ) : null;
});

export const Select = observer(
  ({
    width,
    label,
    className,
    style,
    ...props
  }: DropdownProps & { name: string }) => {
    const [field, meta] = useField(props as Any);

    return (
      <SuiForm.Field
        style={style}
        error={meta.touched && meta.error ? true : false}
        width={width}
        className={className}
      >
        {label && <label htmlFor={props.id || props.name}>{label}</label>}
        <Dropdown
          id={props.id || props.name}
          aria-label={label}
          fluid
          {...(field as Any)}
          {...props}
          onChange={(e, dProps) => {
            field.onChange({
              target: { name: dProps.name, value: dProps.value },
            } as Any);
            props.onChange && props.onChange(e, dProps);
          }}
          value={field.value}
        />
        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </SuiForm.Field>
    );
  }
);

export const AsyncSelect = observer(
  ({
    asyncOptions,
    ...props
  }: DropdownProps & { asyncOptions?: () => Promise<DropdownItemProps[]> }) => {
    const [loading, setLoading] = React.useState(true);
    const [selectOptions, setOptions] = React.useState<DropdownItemProps[]>([]);

    if (asyncOptions && loading) {
      asyncOptions().then((result) => {
        setLoading(false);
        setOptions(result);
      });
    }

    return (
      <Select name="" {...props} options={selectOptions} loading={loading} />
    );
  }
);

// () => {
//   field.onChange({
//     target: {
//       type: 'radio',
//       name: props.name,
//       value: props.value === field.value ? '' : props.value
//     }
//   } as Any);
// }

export const Radio = observer(({ width, className, ...props }: RadioProps) => {
  const [field, meta, helpers] = useField(props);

  return (
    <SuiForm.Field
      error={meta.touched && meta.error ? true : false}
      width={width}
      className={className}
    >
      <SuiRadio
        {...field}
        {...props}
        id={props.id || props.value || props.name}
        value={props.value}
        type="radio"
        onChange={field.onChange}
        onClick={() => {
          if (props.value === field.value) {
            helpers.setValue("");
          }
        }}
        checked={field.value === props.value}
      />
      {meta.touched && meta.error ? <ErrorLabel message={meta.error} /> : null}
    </SuiForm.Field>
  );
});

export const Checkbox = observer(
  ({
    width,
    className,
    extraLabel,
    ...props
  }: CheckboxProps & { name: string }) => {
    const [field, meta] = useField({ ...props, type: "checkbox" });

    if (field.value === true || field.value === false || field.value == null) {
      field.value = undefined;
    }

    return (
      <SuiForm.Field
        error={meta.touched && meta.error ? true : false}
        width={width}
        className={className}
        style={{ display: "flex", alignItems: "center" }}
      >
        <SuiCheckbox
          {...field}
          {...props}
          type="checkbox"
          id={props.id || props.value || props.name}
          checked={field.checked || false}
          onChange={
            field.onChange
            //   (_, { checked }) => {
            //   field.onChange({
            //     target: { type: 'checkbox', checked, name: props.name, value: props.value }
            //   } as Any);
            // }
          }
        />
        {extraLabel ? <span className="ml-h">{extraLabel}</span> : null}
        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </SuiForm.Field>
    );
  }
);
