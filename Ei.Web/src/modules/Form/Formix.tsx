import { action, configure, isObservable, observable, toJS } from "mobx";
import React, { ChangeEventHandler, useEffect } from "react";

import { FormixProvider, useFormixContext } from "./library/formContext";
import { useEventCallback } from "./library/formHooks";
import {
  getIn,
  getSelectedValues,
  getValueForCheckbox,
  isFunction,
  isObject,
  isString,
  setIn,
} from "./library/formUtils";
import {
  FieldHelperProps,
  FieldInputProps,
  FieldMetaProps,
  FormixContextType,
  FormixHandlers,
  FormixValues,
} from "./library/types";
import { validate, ValidatorContext, ValidatorMap } from "./library/validation";

configure({
  enforceActions: "never",
});

interface FormixConfig<Values> {
  isolated?: boolean;
  formName?: string;
  initialValues: Values;
  children?:
    | ((props: FormixContextType<Values>) => React.ReactNode)
    | React.ReactNode;
  validateOnChange?: boolean;
  validateOnBlur?: boolean;
  validateOnMount?: boolean;
  validationSchema?: ValidatorMap<Partial<Values>>;

  onSubmit?: (
    values: Values /*, formixHelpers: FormixHelpers<Values> */
  ) => void | Promise<any>;
  onChange?: (key: string, next?: Any, previous?: Any) => void;
}

export function useFormix<Values extends FormixValues = FormixValues>({
  validateOnChange = true,
  validateOnBlur = true,
  validateOnMount = false,
  validationSchema = {} as Any,
  isolated,
  onSubmit,
  onChange,
  formName,
  ...rest
}: FormixConfig<Values>): FormixContextType<Values> {
  // const fieldRegistry = React.useRef<FieldRegistry>({});

  // if we pass it plain non observable object we make it observable
  const values =
    isObject(rest.initialValues) && !isObservable(rest.initialValues)
      ? observable(rest.initialValues)
      : rest.initialValues;

  if (!values) {
    throw new Error(
      "You need to specify initial values. Null or undefined will not do :("
    );
  }

  const initialValues = {}; //TODO: JSON.parse(JSON.stringify(toJS(values)));

  const errors = React.useRef(observable({}));
  const touched = React.useRef(observable({}));
  const state = React.useRef(
    observable({
      dirty: false,
      errors: null as string[] | null,
    })
  );
  const childContexts = React.useRef<FormixContextType<Any>[]>([]);

  const getFieldMeta = React.useCallback(
    (name: string): FieldMetaProps<any> => {
      return {
        value: getIn(values, name),
        error: getIn(errors.current, name),
        touched: !!getIn(touched.current, name),
      };
    },
    [errors.current, touched.current, values]
  );

  const setFieldError = React.useCallback(
    (field: string, value: string | undefined) => {
      setIn(errors.current, field, value);
    },
    []
  );

  const validateForm = React.useCallback(() => {
    let currentMessages = Object.keys(validationSchema)
      .map((key) => {
        setIn(touched.current, key, true);
        let result = validateSingle(key);
        if (result) {
          result =
            (formName ? `${formName} > ` : "") +
            key[0].toUpperCase() +
            key.substring(1) +
            ": " +
            result;
        }
        return result;
      })
      .reduce((prev, next) => {
        if (next) {
          prev.push(next);
        }
        return prev;
      }, [] as string[]);
    let resultMessages = childContexts.current
      .flatMap((c) => c.validateForm())
      .reduce((prev, next) => {
        if (next) {
          prev.push(next);
        }
        return prev;
      }, currentMessages);

    state.current.errors = resultMessages.length ? resultMessages : null;
    return resultMessages;
  }, [values]);

  const isValid = React.useCallback(() => {
    return validateForm().length === 0;
  }, [values]);

  const isDirty = React.useCallback(() => {
    return childContexts.current.reduce(
      (prev, next) => next.isDirty() || prev,
      state.current.dirty
    );
  }, [values]);

  const validationContext: ValidatorContext = React.useMemo(
    () => ({
      document: values,
      validateAll: isValid,
      validators: validationSchema,
    }),
    [values, isValid, validationSchema]
  );

  const validateSingle = useEventCallback((field: string) => {
    let fieldsValidators = getIn(validationSchema, field);
    if (fieldsValidators) {
      let value = getIn(values, field);
      let message = validate(fieldsValidators, value, validationContext);
      setIn(errors.current, field, message);
      return message;
    }
    return null;
  });

  const setFieldTouched = useEventCallback(
    (field: string, isTouched: boolean = true, shouldValidate?: boolean) => {
      setIn(touched.current, field, isTouched);
      const willValidate =
        shouldValidate === undefined ? validateOnBlur : shouldValidate;
      return willValidate ? validateSingle(field) : Promise.resolve();
    }
  );

  const setFieldValue = useEventCallback(
    (field: string, value: any, shouldValidate?: boolean) => {
      const originalValue = getIn(values, field);
      setIn(values, field, value);

      if (originalValue !== value) {
        state.current.dirty = true;

        // fire listener
        if (onChange) {
          onChange(field, value, originalValue);
        }
      }

      const willValidate =
        shouldValidate === undefined ? validateOnChange : shouldValidate;
      return willValidate ? validateSingle(field) : Promise.resolve();
    }
  );

  const getFieldHelpers = React.useCallback(
    (name: string): FieldHelperProps<any> => {
      return {
        setValue: (value: any, shouldValidate?: boolean) =>
          setFieldValue(name, value, shouldValidate),
        setTouched: (value: boolean, shouldValidate?: boolean) =>
          setFieldTouched(name, value, shouldValidate),
        setError: (value: any) => setFieldError(name, value),
      };
    },
    [setFieldValue, setFieldTouched, setFieldError]
  );

  const executeChange = React.useCallback(
    (eventOrTextValue: string | React.ChangeEvent<any>, maybePath?: string) => {
      // By default, assume that the first argument is a string. This allows us to use
      // handleChange with React Native and React Native Web's onChangeText prop which
      // provides just the value of the input.
      let field = maybePath;
      let val = eventOrTextValue;
      let parsed;
      // If the first argument is not a string though, it has to be a synthetic React Event (or a fake one),
      // so we handle like we would a normal HTML change event.
      if (!isString(eventOrTextValue)) {
        // If we can, persist the event
        // @see https://reactjs.org/docs/events.html#event-pooling
        if ((eventOrTextValue as any).persist) {
          (eventOrTextValue as React.ChangeEvent<any>).persist();
        }
        const target = eventOrTextValue.target
          ? (eventOrTextValue as React.ChangeEvent<any>).target
          : (eventOrTextValue as React.ChangeEvent<any>).currentTarget;

        const { type, name, id, value, checked, outerHTML, options, multiple } =
          target;

        field = maybePath ? maybePath : name ? name : id;

        val = /number|range/.test(type)
          ? /\d+\./.test(value)
            ? value
            : ((parsed = parseFloat(value)), isNaN(parsed) ? "" : parsed)
          : /checkbox/.test(type) // checkboxes
          ? getValueForCheckbox(getIn(values, field!), checked, value)
          : !!multiple // <select multiple>
          ? getSelectedValues(options)
          : value;
      }

      if (field) {
        // Set form fields by name
        setFieldValue(field, val);
      }
    },
    [setFieldValue, values]
  );

  const handleChange = useEventCallback<ChangeEventHandler>(
    (
      eventOrPath: string | React.ChangeEvent<any>
    ): void | ((eventOrTextValue: string | React.ChangeEvent<any>) => void) => {
      if (isString(eventOrPath)) {
        return (event) => executeChange(event, eventOrPath);
      } else {
        executeChange(eventOrPath);
      }
    }
  );

  const executeBlur = React.useCallback(
    (e: any, path?: string) => {
      if (e.persist) {
        e.persist();
      }
      const { name, id, outerHTML } = e.target;
      const field = path ? path : name ? name : id;

      setFieldTouched(field, true);
    },
    [setFieldTouched]
  );

  const handleBlur = useEventCallback<FormixHandlers["handleBlur"]>(
    (eventOrString: any): void | ((e: any) => void) => {
      if (isString(eventOrString)) {
        return (event) => executeBlur(event, eventOrString);
      } else {
        executeBlur(eventOrString);
      }
    }
  );

  const saved = React.useCallback(
    action(() => {
      state.current.dirty = false;
      childContexts.current.forEach((c) => c.saved());
    }),
    [values]
  );

  const getFieldProps = React.useCallback(
    (nameOrOptions): FieldInputProps<any> => {
      const isAnObject = isObject(nameOrOptions);
      const name = isAnObject ? nameOrOptions.name : nameOrOptions;
      const valueState = getIn(values, name);

      const field: FieldInputProps<any> = {
        name,
        value: valueState,
        onChange: handleChange as Any,
        onBlur: handleBlur,
      };
      if (isAnObject) {
        const {
          type,
          value: valueProp, // value is special for checkboxes
          as: is,
          multiple,
        } = nameOrOptions;

        if (type === "checkbox") {
          if (valueProp === undefined) {
            field.checked = !!valueState;
          } else {
            field.checked = !!(
              Array.isArray(valueState) && ~valueState.indexOf(valueProp)
            );
            field.value = valueProp;
          }
        } else if (type === "radio") {
          field.checked = valueState === valueProp;
          field.value = valueProp;
        } else if (is === "select" && multiple) {
          field.value = field.value || [];
          field.multiple = true;
        }
      }
      return field;
    },
    [handleBlur, handleChange, values]
  );

  const handleReset = useEventCallback((e) => {
    if (e && e.preventDefault && isFunction(e.preventDefault)) {
      e.preventDefault();
    }

    if (e && e.stopPropagation && isFunction(e.stopPropagation)) {
      e.stopPropagation();
    }

    action(() => {
      // replace all keys
      if (values == null) {
        return;
      }
      for (let key of Object.keys(values)) {
        if (initialValues[key]) {
          (values as Any)[key] = initialValues[key] as any;
        } else {
          delete values[key];
        }
      }
    });
  });

  const handleSubmit = useEventCallback(
    async (e?: React.FormEvent<HTMLFormElement>) => {
      if (e && e.preventDefault && isFunction(e.preventDefault)) {
        e.preventDefault();
      }

      if (e && e.stopPropagation && isFunction(e.stopPropagation)) {
        e.stopPropagation();
      }

      // validate form
      let messages = validateForm();

      // if all ok execute the handler
      if (messages.length == 0 && onSubmit && values) {
        await onSubmit(values);
        saved();
      }
    }
  );

  const context = useFormixContext();

  const currentContext: FormixContextType<Values> = {
    getFieldHelpers,
    getFieldMeta,
    getFieldProps,
    setFieldError,
    setFieldValue,
    setFieldTouched,
    isValid,
    validateForm,
    childContexts: childContexts.current,
    isDirty,
    saved,
    handleReset,
    handleSubmit,
    state: state.current,
    parentContext: context,
    get root() {
      if (currentContext.parentContext == null) {
        return currentContext;
      } else return currentContext.parentContext.root;
    },
    // registerField,
    // unregisterField,
  };

  // register in parent context

  useEffect(() => {
    if (!isolated && context != null) {
      context.childContexts.push(currentContext);
    }
  }, [isValid, isDirty]);

  return currentContext;
}

export function Formix<
  Values extends FormixValues = FormixValues,
  ExtraProps = {}
>(props: FormixConfig<Values> & ExtraProps) {
  const formixbag = useFormix<Values>(props);
  const { children } = props;

  return (
    <FormixProvider value={formixbag}>
      {children // children come last, always called
        ? isFunction(children)
          ? children(formixbag)
          : React.Children.count(children) > 0
          ? React.Children.only(children)
          : null
        : null}
    </FormixProvider>
  );
}
