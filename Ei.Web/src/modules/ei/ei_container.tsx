import React from "react";
import { useParams } from "react-router-dom";
import { Message } from "semantic-ui-react";
import { useAppContext } from "../../config/context";

import { Ei } from "./ei_model";

export const EiContainer = ({ children }: React.PropsWithChildren<any>) => {
  const { eiId } = useParams<{ eiId: string }>();
  const context = useAppContext();

  if (context.ei == null) {
    const storedName = "ws." + eiId;
    const eiSource = localStorage.getItem(storedName);
    if (eiSource == null) {
      return <Message header="Institution not found" />;
    }
    const parsedEi = JSON.parse(eiSource);
    let ei = new Ei(parsedEi, context);
    context.ei = ei;
    context.Ui.history.startHistory(ei, context);
  }

  return <>{children}</>;
};

EiContainer.displayName = "EiContainer";
