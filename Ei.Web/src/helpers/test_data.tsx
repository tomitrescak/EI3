import { context as createContext, ContextModel } from '../config/context';

type ContextOverride = {
  cache?: boolean;
  overrideContext?: Partial<App.Context>;
  overrideStore?: Partial<App.Store>;
};

export const create = {
  context(
    { cache = false, overrideContext = {}, overrideStore = {} }: ContextOverride = {}
  ): App.Context {
    if (cache) {
      return createContext();
    }
    const clone = new ContextModel(cache);
    const newContext = { ...clone, ...overrideContext };

    if (Object.getOwnPropertyNames(overrideStore).length > 0) {
      newContext.store = { ...newContext.store, ...overrideStore } as any;
    }

    newContext.store.context = newContext;
    return newContext;
  },
};
