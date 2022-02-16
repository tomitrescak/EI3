export type FieldValidator = (value: any) => string | void | Promise<string | void>;

/**
 * Values of fields in the form
 */
export interface FormixValues {
  [field: string]: any;
}

export interface FormixState {
  errors: string[] | null;
}

/** Field metadata */
export interface FieldMetaProps<Value> {
  /** Value of the field */
  value: Value;
  /** Error message of the field */
  error?: string;
  /** Has the field been visited? */
  touched: boolean;
}

/** Imperative handles to change a field's value, error and touched */
export interface FieldHelperProps<Value> {
  /** Set the field's value */
  setValue: (value: Value, shouldValidate?: boolean) => void;
  /** Set the field's touched value */
  setTouched: (value: boolean, shouldValidate?: boolean) => void;
  /** Set the field's error value */
  setError: (value: Value) => void;
}

export interface FormixHandlers {
  /** Form submit handler */
  handleSubmit: (e?: React.FormEvent<HTMLFormElement>) => void;
  /** Reset form event handler  */
  handleReset: (e?: React.SyntheticEvent<any>) => void;
  handleBlur: {
    /** Classic React blur handler, keyed by input name */
    (e: React.FocusEvent<any>): void;
  };
  handleChange: {
    /** Classic React change handler, keyed by input name */
    (e: React.ChangeEvent<any>): void;
  };
  getFieldProps: <Value = any>(props: any) => FieldInputProps<Value>;
  getFieldMeta: <Value>(name: string) => FieldMetaProps<Value>;
  getFieldHelpers: <Value = any>(name: string) => FieldHelperProps<Value>;
}

/** Field input value, name, and event handlers */
export interface FieldInputProps<Value> {
  /** Value of the field */
  value: Value;
  /** Name of the field */
  name: string;
  /** Multiple select? */
  multiple?: boolean;
  /** Is the field checked? */
  checked?: boolean;
  /** Change event handler */
  onChange: FormixHandlers['handleChange'];
  /** Blur event handler */
  onBlur: FormixHandlers['handleBlur'];
}

/**
 * State, handlers, and helpers made available to Formix's primitive components through context.
 */
export type FormixContextType<Values> = {
  getFieldProps: <Value = any>(props: any) => FieldInputProps<Value>;
  getFieldMeta: <Value>(name: string) => FieldMetaProps<Value>;
  getFieldHelpers: <Value = any>(name: string) => FieldHelperProps<Value>;

  setFieldValue: <Value = any>(name: string, value: Value) => void;
  setFieldError: <Value = any>(name: string, error: string) => void;
  setFieldTouched: <Value = any>(name: string, touched: boolean) => void;

  isValid: () => boolean;
  validateForm: () => string[];
  isDirty: () => boolean;
  saved: () => void;
  handleReset: FormixHandlers['handleReset'];
  handleSubmit: FormixHandlers['handleSubmit'];

  root: FormixContextType<Any>;
  parentContext: FormixContextType<Any>;
  childContexts: FormixContextType<Any>[];
  state: FormixState;

  // registerField: (name: string, fns: { validate?: FieldValidator }) => void;
  // unregisterField: (name: string) => void;
};

export type FormixErrors<Values> = {
  [K in keyof Values]?: Values[K] extends any[]
    ? Values[K][number] extends object // [number] is the special sauce to get the type of array's element. More here https://github.com/Microsoft/TypeScript/pull/21316
      ? FormixErrors<Values[K][number]>[] | string | string[]
      : string | string[]
    : Values[K] extends object
    ? FormixErrors<Values[K]>
    : string;
};

/**
 * Formix state helpers
 */
export interface FormixHelpers<Values> {
  /** Validate form values */
  validateForm: (values?: any) => Promise<FormixErrors<Values>>;
  /** Validate field value */
  validateField: (field: string) => void;
  /** Reset form */
  resetForm: () => void;
  /** Submit the form imperatively */
  submitForm: () => Promise<void>;
}
