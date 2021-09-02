import * as React from "react";

import { IObservableArray, observable } from "mobx";
import { observer } from "mobx-react";
import { Button, Header, Icon, List, Segment } from "semantic-ui-react";
import { style } from "typestyle";
import { Link } from "react-router-dom";
import { Ei } from "./ei_model";
import { useAppContext } from "../../config/context";

const homeStyle = style({
  margin: "12px!important",
});

interface StoredEi {
  id: string;
  name: string;
}

interface Props {
  eis: IObservableArray<StoredEi>;
}

export const EiListContainer = () => {
  const eiString = localStorage.getItem("eis") || "[]";
  const eis: IObservableArray<StoredEi> = observable.array(
    JSON.parse(eiString)
  );

  return <EiList eis={eis} />;
};

function openFile(callback: (content: string) => void) {
  let readFile = function (e: React.MouseEvent<HTMLInputElement>) {
    var file = e.currentTarget.files[0];
    if (!file) {
      return;
    }
    var reader = new FileReader();
    reader.onload = function (e) {
      var contents = e.target.result;
      callback(contents as string);
      document.body.removeChild(fileInput);
    };
    reader.readAsText(file);
  };
  let fileInput = document.createElement("input");
  fileInput.type = "file";
  fileInput.style.display = "none";
  fileInput.onchange = readFile as any;
  document.body.appendChild(fileInput);

  fileInput.click();
  // clickElem(fileInput)
}

export const EiList = observer(({ eis }: Props) => {
  const context = useAppContext();
  return (
    <Segment className={homeStyle}>
      <Header content="Your Institutions" dividing icon="home" />
      {eis.length === 0 && <span>No Institutions</span>}
      <List>
        {eis.map((e, i) => (
          <List.Item key={i}>
            <List.Content>
              <Button
                size="tiny"
                icon="trash"
                color="red"
                onClick={async () => {
                  if (
                    await context.Ui.confirmDialogAsync(
                      "Do you want to delete this institution? Action cannot be undone!"
                    )
                  ) {
                    localStorage.removeItem("ws." + e.id);
                    eis.remove(eis.find((ei) => ei.id === ei.id));
                    localStorage.setItem(
                      "eis",
                      JSON.stringify(
                        eis.map((ei) => ({ id: ei.id, name: ei.name }))
                      )
                    );
                  }
                }}
              />

              <Button
                size="tiny"
                icon="download"
                color="blue"
                onClick={() => {
                  let a = window.document.createElement("a");
                  a.href = window.URL.createObjectURL(
                    new Blob([localStorage.getItem("ws." + e.id)], {
                      type: "text/json",
                    })
                  );
                  a.download = `${e.name}.json`;

                  // Append anchor to body.
                  document.body.appendChild(a);
                  a.click();

                  // Remove anchor from body
                  document.body.removeChild(a);
                }}
              />

              <Icon name="home" />
              <Link to={`/ei/${e.name.toUrlName()}/${e.id}`}>{e.name}</Link>
            </List.Content>
          </List.Item>
        ))}
      </List>

      <Button
        content="Load Institution"
        icon="upload"
        onClick={async () => {
          openFile((content) => {
            try {
              const json = JSON.parse(content);
              const id = json.Id;

              if (eis.some((e) => e.id === id)) {
                context.Ui.alertError("Institution with this id exists: " + id);
                return;
              }

              localStorage.setItem("ws." + id, content);
              eis.push({ id, name: json.Name });
              localStorage.setItem(
                "eis",
                JSON.stringify(eis.map((e) => ({ id: e.id, name: e.name })))
              );

              context.Ui.alert("Institution Loaded: " + json.Name);
            } catch (ex) {
              context.Ui.alertError(
                "Could not open the institution: " + ex.message
              );
            }
          });
        }}
      />

      <Button
        content="Create Institution"
        icon="plus"
        onClick={async () => {
          let promptValue = await context.Ui.promptText(
            "Name of the new institution?"
          );

          if (promptValue) {
            const name = promptValue.value;
            const id = name.toId();

            if (eis.some((e) => e.id === id)) {
              context.Ui.alertError("Institution with this id exists: " + id);
            }

            const ei = Ei.create(id, name, context);
            localStorage.setItem("ws." + id, JSON.stringify(ei.json));
            eis.push({ id, name });
            localStorage.setItem(
              "eis",
              JSON.stringify(eis.map((e) => ({ id: e.id, name: e.name })))
            );
          }
        }}
      />
    </Segment>
  );
});
