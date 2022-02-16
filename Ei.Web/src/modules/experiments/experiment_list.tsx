import { observer } from "mobx-react-lite";
import React from "react";
import { Link } from "react-router-dom";
import { Label, List } from "semantic-ui-react";
import { AccordionContent, AccordionTitle } from "../components/accordion";
import { AccordionButton } from "../components/hierarchic_entity_view";
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
        <AccordionButton
          floated="right"
          icon="plus"
          compact
          color="green"
          onClick={ei.createExperiment}
        />
      </AccordionTitle>
      <AccordionContent active={active}>
        <List>
          {ei.Experiments.map((e, ix) => (
            <List.Item
              key={e.Id + ix}
              as={Link}
              to={`/ei/${ei.Name.toUrlName()}/experiment/${
                e.Name || "None"
              }?ei=${ei.Id}&id=${e.Id || "none"}`}
            >
              {e.Name || "None"}
            </List.Item>
          ))}
        </List>
      </AccordionContent>
    </>
  );
});
