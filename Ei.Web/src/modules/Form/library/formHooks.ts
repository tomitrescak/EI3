import React from 'react';

import { useFormixContext } from './formContext';
import { isObject } from './formUtils';
import { FieldHelperProps, FieldInputProps, FieldMetaProps, FieldValidator } from './types';

export type MinProps = {
  name?: string;
  type?: string;
  validate?: FieldValidator;
};

export function useField<Val = any>(
  propsOrFieldName: string | MinProps
): [FieldInputProps<Val>, FieldMetaProps<Val>, FieldHelperProps<Val>] {
  const formix = useFormixContext();
  const {
    getFieldProps,
    getFieldMeta,
    getFieldHelpers
    //  registerField, unregisterField
  } = formix;

  const isAnObject = isObject(propsOrFieldName);

  // Normalize propsOrFieldName to FieldHookConfig<Val>
  const props: MinProps = isAnObject
    ? (propsOrFieldName as MinProps)
    : { name: propsOrFieldName as string };

  const { name: fieldName } = props;

  // React.useEffect(() => {
  //   if (fieldName) {
  //     registerField(fieldName, {
  //       validate: validateFn
  //     });
  //   }
  //   return () => {
  //     if (fieldName) {
  //       unregisterField(fieldName);
  //     }
  //   };
  // }, [registerField, unregisterField, fieldName, validateFn]);

  if (fieldName == null) {
    throw new Error('You always have to specify the name of the field');
  }

  return [getFieldProps(props), getFieldMeta(fieldName), getFieldHelpers(fieldName)];
}

// React currently throws a warning when using useLayoutEffect on the server.
// To get around it, we can conditionally useEffect on the server (no-op) and
// useLayoutEffect in the browser.
// @see https://gist.github.com/gaearon/e7d97cdf38a2907924ea12e4ebdf3c85
const useIsomorphicLayoutEffect =
  typeof window !== 'undefined' &&
  typeof window.document !== 'undefined' &&
  typeof window.document.createElement !== 'undefined'
    ? React.useLayoutEffect
    : React.useEffect;

export function useEventCallback<T extends (...args: any[]) => any>(fn: T): T {
  const ref: any = React.useRef(fn);

  // we copy a ref to the callback scoped to the current state/props on each render
  useIsomorphicLayoutEffect(() => {
    ref.current = fn;
  });

  return React.useCallback((...args: any[]) => ref.current.apply(void 0, args), []) as T;
}
