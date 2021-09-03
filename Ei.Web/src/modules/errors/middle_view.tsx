import React from "react";

import { Observer, observer } from "mobx-react";
import SyntaxHighlighter from "react-syntax-highlighter";
import { docco } from "react-syntax-highlighter/styles/hljs";
import { List, Menu } from "semantic-ui-react";
import { style } from "typestyle";
import { SocketClient } from "../ws/socket_client";
import { useAppContext } from "../../config/context";

const pane = {
  padding: "6px",
  display: "none",
  position: "absolute",
  overflow: "auto",
  margin: "0px",
  top: "42px",
  right: "0px",
  left: "0px",
  bottom: "0px",
  $nest: {
    ".complete pre": {
      position: "absolute",
      overflow: "auto",
      margin: "0px",
      right: "0px",
      left: "0px",
      bottom: "0px",
      overflowX: "none!important",
    },
  },
} as any;

const activePane = style(pane, { display: "block" });
const hiddenPane = style(pane);

interface Props {}

export const Messages = () => {
  const context = useAppContext();
  React.useEffect(() => {
    const id = context.client.send({
      query: "MonitorInstitution",
      isSubscription: true,
      receiver: (data: any) => {
        console.log(data);
      },
    });
    return () => {
      context.client.unsubscribe("MonitorInstitution", id);
    };
  }, []);
  return (
    <Observer>
      {() =>
        context.messages.length ? (
          <List>
            {context.messages.map((m, i) => (
              <List.Item key={i}>{m}</List.Item>
            ))}
          </List>
        ) : (
          <div>No Message</div>
        )
      }
    </Observer>
  );
};

Messages.displayName = "MessagesView";

export const Highlight = observer(() => {
  const context = useAppContext();
  return (
    <SyntaxHighlighter language="cs" style={docco} showLineNumbers={true}>
      {(context.compiledCode || "").trim()}
    </SyntaxHighlighter>
  );
});

Highlight.displayName = "HighlightView";

export const Errors = observer(() => {
  const context = useAppContext();
  return (
    <>
      {context.errors.length ? (
        <List>
          {context.errors.map((m, i) => {
            return (
              <List.Item key={i}>
                <List.Content>
                  <List.Header>
                    Line {m.Line}: {m.Message}
                  </List.Header>
                  {m.Code && m.Code.length > 0 && (
                    <List.Description>
                      <SyntaxHighlighter
                        language="cs"
                        style={docco}
                        showLineNumbers={true}
                        startingLineNumber={m.Line - 5}
                      >
                        {m.Code.join("\n") || ""}
                      </SyntaxHighlighter>
                    </List.Description>
                  )}
                </List.Content>
              </List.Item>
            );
          })}
        </List>
      ) : (
        <div>No compilation errors! G'day! 🤓</div>
      )}
    </>
  );
});

export const MiddleView = observer((props: Props) => {
  const [activeItem, setActiveItem] = React.useState("errors");
  const context = useAppContext();

  const handleItemClick = (_e: any, { name }: any) => setActiveItem(name);

  const isActive = (name: string) => {
    return name === activeItem;
  };

  const downloadCode = () => {
    let myWindow = window.open("");
    myWindow.document.write(
      "<pre>" +
        context.compiledCode.replace(/>/g, "&gt;").replace(/</g, "&lt;") +
        "</pre>" || "No code was compiled"
    );
    let range = myWindow.document.createRange();
    range.selectNode(myWindow.document.getElementById("hello"));
    myWindow.getSelection().addRange(range);
    (myWindow as any).select();
  };

  return (
    <div>
      <Menu pointing secondary compact fluid inverted color="grey">
        <Menu.Item
          name="messages"
          content="Messages"
          icon="mail"
          active={activeItem === "messages"}
          onClick={handleItemClick}
        />
        <Menu.Item
          name="errors"
          content={`${context.errors.length} Error${
            context.errors.length === 1 ? "" : "s"
          }`}
          icon="bug"
          active={activeItem === "errors"}
          onClick={handleItemClick}
        />
        <Menu.Item
          name="compiled"
          content="Code"
          icon="code"
          active={activeItem === "compiled"}
          onClick={handleItemClick}
        />
        <Menu.Menu position="right">
          <Menu.Item icon="download" onClick={downloadCode} />
          <Menu.Item content="Clear" icon="remove" />
        </Menu.Menu>
      </Menu>

      {isActive("messages") && (
        <div className={pane}>
          <Messages />
        </div>
      )}
      {isActive("errors") && (
        <div className={pane}>
          <Errors />
        </div>
      )}
      {isActive("compiled") && (
        <div className={pane + " complete"}>
          <Highlight />
        </div>
      )}
    </div>
  );
});
