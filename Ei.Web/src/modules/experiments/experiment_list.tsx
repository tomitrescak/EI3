import { observer } from "mobx-react-lite";
import React from "react";
import { Link } from "react-router-dom";
import { Label, List } from "semantic-ui-react";
import { AccordionContent, AccordionTitle } from "../components/accordion";
import { Ei } from "../ei/ei_model";

interface Props {
  active: boolean;
  index: number;
  ei: Ei;
  handleClick: any;
}

export const ExperimentList = observer((props: Props) => {
  const { active, index, ei, handleClick } = props;

  return (
    <>
      <AccordionTitle active={active} index={index} onClick={handleClick}>
        <Label
          size="tiny"
          color="blue"
          circular
          content={ei.Experiments.length}
        />{" "}
        Experiments
      </AccordionTitle>
      <AccordionContent active={active}>
        <List>
          <List.Item
            as={Link}
            to={`/${ei.Name.toUrlName()}/experiment/default/general/1?ei=${
              ei.Id
            }`}
          >
            General
          </List.Item>
        </List>
      </AccordionContent>
    </>
  );
});
