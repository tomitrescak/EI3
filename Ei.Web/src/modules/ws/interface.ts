export interface QueryProps {
  error?: { message: string };
  networkStatus: number;
  loading: boolean;
  variables: {
    [variable: string]: any;
  };
}

export type ChildProps<P, R> = P & {
  data?: QueryProps & R;
};

export interface OptionProps<TProps, TResult> {
  ownProps: TProps;
  data?: QueryProps & TResult;
}

export type CompositeComponent<P> = React.ComponentClass<P> | React.StatelessComponent<P>;
export type ComponentDecorator<TOwnProps, TMergedProps> = (
  component: CompositeComponent<TMergedProps>
) => React.ComponentClass<TOwnProps>;

export interface Options<TData, TProps> {
  variables?: (props: TProps) => string[];
  props?: (props: OptionProps<TProps, TData>) => any;
  name?: string;
  waitForData?: boolean;
  cache?: boolean;
  loadingComponent?: () => JSX.Element;
}