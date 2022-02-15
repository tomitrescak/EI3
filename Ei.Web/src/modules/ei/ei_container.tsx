import React from "react";
import { Message } from "semantic-ui-react";
import { useAppContext } from "../../config/context";
import { useQuery } from "../../helpers/client_helpers";

import { Ei } from "./ei_model";

export const EiContainer = ({ children }: React.PropsWithChildren<any>) => {
  const { ei } = useQuery();
  const context = useAppContext();

  if (context.ei == null) {
    const storedName = "ws." + ei;
    const eiSource = localStorage.getItem(storedName);
    if (eiSource == null) {
      return <Message header="Institution not found" />;
    }
    const parsedEi = JSON.parse(eiSource);
    let eiModel = new Ei(parsedEi, context);
    context.ei = eiModel;
    context.Ui.history.startHistory(eiModel, context);
  }

  return <>{children}</>;
};

EiContainer.displayName = "EiContainer";
