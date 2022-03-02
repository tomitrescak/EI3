import React from "react";

import { Observer, observer } from "mobx-react";
import SyntaxHighlighter from "react-syntax-highlighter";
import { docco } from "react-syntax-highlighter/styles/hljs";
import { List, Menu } from "semantic-ui-react";
// import { SocketClient } from "../ws/socket_client";
import { useAppContext } from "../../config/context";
import styled from "@emotion/styled";

const Pane = styled.div`
  padding: 6px;
  position: absolute;
  overflow: auto;
  margin: 0px;
  top: 42px;
  right: 0px;
  left: 0px;
  bottom: 0px;

  &.complete pre {
    margin: 0px;
    right: 0px;
    left: 0px;
    bottom: 0px;
    overflow-x: none !important;
  }
`;

// const ActivePane = style(pane, { display: "block" });
// const HiddenPane = style(pane);

const Table = styled.table`
  width: 100%;
  th {
    background: green;
    color: white;
    position: sticky;
    text-align: left;
    top: -8px; /* Don't forget this, required for the stickiness */
    box-shadow: 0 2px 2px -1px rgba(0, 0, 0, 0.4);
  }
`;
export const Messages = observer(() => {
  const context = useAppContext();
  const ref = React.useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    if (ref.current) {
      ref.current!.parentElement.scrollTop = ref.current!.scrollHeight;
    }
    console.log("Scrolling to: " + ref.current!.scrollHeight);
  });

  return (
    <div ref={ref}>
      <Table style={{ position: "relative" }}>
        <thead>
          <tr>
            <th>Agent</th>
            <th>Component</th>
            <th>Message</th>
          </tr>
        </thead>
        <tbody>
          {context.messages.length ? (
            context.messages.map((m, i) => (
              <tr key={i}>
                <td>{m.agent}</td>
                <td>{m.component}</td>
                <td>{m.message}</td>
              </tr>
            ))
          ) : (
            <div>No Message</div>
          )}
        </tbody>
      </Table>
    </div>
  );
});

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

export const MiddleView = observer(() => {
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
        <Pane>
          <Messages />
        </Pane>
      )}
      {isActive("errors") && (
        <Pane>
          <Errors />
        </Pane>
      )}
      {isActive("compiled") && (
        <Pane className={"complete"}>
          <Highlight />
        </Pane>
      )}
    </div>
  );
});
