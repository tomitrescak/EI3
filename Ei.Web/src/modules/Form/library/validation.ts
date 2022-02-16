// Fields and validators

import { getIn } from "./formUtils";

export type ValidatorContext = {
  document: any;
  validators: ValidatorMap<Any>;
  validateAll: Function;
};

var isoDateRegExp = new RegExp(
  /(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+([+-][0-2]\d:[0-5]\d|Z))|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))/
);

export function isISODate(str: string) {
  return isoDateRegExp.test(str);
}

export function validate(
  validators: Validator[],
  value: any,
  context: ValidatorContext
): string | null {
  if (!validators) {
    return null;
  }
  for (let validator of validators) {
    let message = validator(value, context);
    if (message) {
      return message;
    }
  }
  return null;
}

export type Validator = (
  value: any,
  context: ValidatorContext
) => string | null;

export type TypedValidator = (...props: Any[]) => Validator;

export type ValidatorMap<Values> = {
  [P in keyof Values]: Validator[];
};

export const isRequired: Validator = (value?: string | number | undefined) => {
  return value === "" || value == null ? `This field is required!` : null;
};

export const required: TypedValidator = (message: string) => {
  return (value, context) => {
    return isRequired(value, context) ? message : null;
  };
};

export const regEx: TypedValidator = (reg: RegExp, message?: string) => {
  return (value: string) => {
    if (reg.exec(value)) {
      return null;
    }
    if (message) {
      return message;
    }
    return `Unexpected format`;
  };
};

export const sameAs: TypedValidator = (field: string, message?: string) => {
  return (value, context) => {
    return value !== getIn(context.document, field)
      ? message || `Value must match with "${field}"`
      : null;
  };
};

function testInt(value: any) {
  var x;
  if (value == null || value === "") {
    return true;
  }
  if (isNaN(value)) {
    return false;
  }
  x = parseFloat(value);
  return (x | 0) === x;
}

export function testNumeric(str: Any) {
  if (str == null || str === "") {
    return true;
  }
  return (
    Number(str) === str ||
    (!isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
      !isNaN(parseFloat(str)))
  ); // ...and ensure strings of whitespace fail
}

export const isInt: Validator = (n: any) => {
  return testInt(n) ? null : "Expected integer";
};

export const int: TypedValidator = (message: string) => (n: any) => {
  return testInt(n) ? null : message;
};

export const isNumber = (n: any) => {
  return testNumeric(n) ? null : "Expected number";
};

export const number: TypedValidator = (message: string) => (n: any) => {
  return testNumeric(n) ? null : message;
};

export const lt: TypedValidator = (max: number, message?: string) => {
  return function (n, context) {
    const isNum = isNumber(n);
    if (isNum) {
      return isNum;
    }
    return n >= max ? message || `Must be < ${max}` : null;
  };
};

export const lte: TypedValidator = (max: number, message?: string) => {
  return function (n) {
    const isNum = isNumber(n);
    if (isNum) {
      return isNum;
    }
    return n > max ? message || `Must be <= ${max}` : null;
  };
};

export const gt: TypedValidator = (min: number, message?: string) => {
  return function (n) {
    const isNum = isNumber(n);
    if (isNum) {
      return isNum;
    }
    return n <= min ? message || `Must be > ${min}` : null;
  };
};

export const gte: TypedValidator = (min: number, message?: string) => {
  return function (n) {
    const isNum = isNumber(n);
    if (isNum) {
      return isNum;
    }
    return n < min ? message || `Must be >= ${min}` : null;
  };
};

const emailReg =
  /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

export const isEmail: Validator = (value: string) => {
  return value == null || value === "" || emailReg.test(value)
    ? null
    : `Expected email`;
};

export const email: TypedValidator = (message: string) => {
  return (value, context) => (isEmail(value, context) ? message : null);
};

export const minLength: TypedValidator = (length: number, message?: string) => {
  return (value: number | string) => {
    return value == null || value === "" || value.toString().length >= length
      ? null
      : message || `Minimum ${length} characters`;
  };
};

export const maxLength: TypedValidator = (length: number, message?: string) => {
  return (value: number | string) => {
    return value == null || value === "" || value.toString().length <= length
      ? null
      : message || `Maximum ${length} characters`;
  };
};

export const minArrayLength: TypedValidator = (
  length: number,
  message?: string
) => {
  return (value: any[]) => {
    return value == null || value.length >= length
      ? null
      : message || `Minimum ${length} item${length === 1 ? "" : "s"}`;
  };
};

export const maxArrayLength: TypedValidator = (
  length: number,
  message?: string
) => {
  return (value: any[]) => {
    return value == null || value.length <= length
      ? null
      : message || `Maximum ${length} item${length === 1 ? "" : "s"}`;
  };
};

export const eq: TypedValidator = (
  comparer: () => string | string,
  message?: string
) => {
  return (value: Any) => {
    let val2 = typeof comparer === "function" ? comparer() : comparer;
    if (val2 == value) {
      return null;
    }

    return message || `Value ${value} does not match ${val2}`;
  };
};

export const is: TypedValidator = (
  comparer: () => string | string,
  message?: string
) => {
  return (value: Any) => {
    let val2 = typeof comparer === "function" ? comparer() : comparer;
    if (val2 === value) {
      return null;
    }
    let val1 = typeof value === "string" ? `"${value}"` : value;
    val2 = typeof val2 === "string" ? `"${val2}"` : val2;

    return message || `Value ${val1} does not match ${val2}`;
  };
};

export const link: TypedValidator = (name: string) => {
  return (_, context) => {
    if (context.validators[name]) {
      let value = getIn(context.document, name);
      if (value != null) {
        validate(context.validators[name], value, context);
      }
    }
    return null;
  };
};
