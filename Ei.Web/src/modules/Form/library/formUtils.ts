// import { default as toPath } from 'lodash/toPath';
function toPath(path: string | string[]) {
  if (Array.isArray(path)) {
    return path;
  }
  return path.split('.').flatMap(a =>
    a
      .replace(']', '')
      .split('[')
      .map((b: Any) => (isNaN(b) ? b : parseFloat(b)))
  );
}
/** @private is the value an empty array? */
export const isEmptyArray = (value?: any) => Array.isArray(value) && value.length === 0;

/** @private is the given object a Function? */
export const isFunction = (obj: any): obj is Function => typeof obj === 'function';

/** @private is the given object an Object? */
export const isObject = (obj: any, plain = true): obj is Object =>
  obj !== null && typeof obj === 'object' && (!plain || obj.constructor.name === 'Object');

/** @private is the given object an integer? */
export const isInteger = (obj: any): boolean => String(Math.floor(Number(obj))) === obj;

export function isNumber(n: any) {
  return Number(n) === n;
}

/** @private is the given object a string? */
export const isString = (obj: any): obj is string =>
  Object.prototype.toString.call(obj) === '[object String]';

/** @private is the given object a NaN? */
// eslint-disable-next-line no-self-compare
// export const isNaN = (obj: any): boolean => obj !== obj;

/** @private is the given object/value a promise? */
export const isPromise = (value: any): value is PromiseLike<any> =>
  isObject(value) && isFunction(value.then);

/**
 * Deeply get a value from an object via its path.
 */
export function getIn(obj: any, key: string | string[], def?: any, p: number = 0) {
  const path = toPath(key);
  while (obj && p < path.length) {
    obj = obj[path[p++]];
  }
  return obj === undefined ? def : obj;
}

export function setIn(obj: any, path: string, value: any): any {
  let res: any = obj; // this keeps inheritance when obj is a class
  let resVal: any = res;
  let i = 0;
  let pathArray = toPath(path);

  for (; i < pathArray.length - 1; i++) {
    const currentPath: string = pathArray[i];
    let currentObj: any = getIn(obj, pathArray.slice(0, i + 1));

    if (currentObj && (isObject(currentObj) || Array.isArray(currentObj))) {
      resVal = resVal[currentPath] = currentObj;
    } else {
      const nextPath: string = pathArray[i + 1];
      resVal = resVal[currentPath] = isInteger(nextPath) && Number(nextPath) >= 0 ? [] : {};
    }
  }

  // Return original object if new value is the same as current
  if ((i === 0 ? obj : resVal)[pathArray[i]] === value) {
    return obj;
  }

  if (value === undefined) {
    delete resVal[pathArray[i]];
  } else {
    resVal[pathArray[i]] = value;
  }

  // If the path array has a single element, the loop did not run.
  // Deleting on `resVal` had no effect in this scenario, so we delete on the result instead.
  if (i === 0 && value === undefined) {
    delete res[pathArray[i]];
  }

  return res;
}

/** Return the next value for a checkbox */
export function getValueForCheckbox(
  currentValue: string | any[],
  checked: boolean,
  valueProp: any
) {
  // If the current value was a boolean, return a boolean
  if (typeof currentValue === 'boolean') {
    return Boolean(checked);
  }

  // If the currentValue was not a boolean we want to return an array
  let currentArrayOfValues = [];
  let isValueInArray = false;
  let index = -1;

  if (!Array.isArray(currentValue)) {
    // eslint-disable-next-line eqeqeq
    if (!valueProp || valueProp == 'true' || valueProp == 'false') {
      return Boolean(checked);
    }
  } else {
    // If the current value is already an array, use it
    currentArrayOfValues = currentValue;
    index = currentValue.indexOf(valueProp);
    isValueInArray = index >= 0;
  }

  // If the checkbox was checked and the value is not already present in the aray we want to add the new value to the array of values
  if (checked && valueProp && !isValueInArray) {
    return currentArrayOfValues.concat(valueProp);
  }

  // If the checkbox was unchecked and the value is not in the array, simply return the already existing array of values
  if (!isValueInArray) {
    return currentArrayOfValues;
  }

  // If the checkbox was unchecked and the value is in the array, remove the value and return the array
  return currentArrayOfValues.slice(0, index).concat(currentArrayOfValues.slice(index + 1));
}

/** Return multi select values based on an array of options */
export function getSelectedValues(options: any[]) {
  return Array.from(options)
    .filter(el => el.selected)
    .map(el => el.value);
}
