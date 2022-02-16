import React from 'react';
import { FormixContextType } from './types';

export const FormixContext = React.createContext<FormixContextType<any>>(undefined as any);
export const FormixProvider = FormixContext.Provider;
export const FormixConsumer = FormixContext.Consumer;

export function useFormixContext<Values>() {
  const formix = React.useContext<FormixContextType<Values>>(FormixContext);
  return formix;
}
