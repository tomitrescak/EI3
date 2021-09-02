import * as React from "react";

import { inject, Observer, observer } from "mobx-react";
import SyntaxHighlighter from "react-syntax-highlighter";
import { docco } from "react-syntax-highlighter/styles/hljs";
import { List, Menu } from "semantic-ui-react";
import { style } from "typestyle";
import { SocketClient } from "../ws/socket_client";
import { autorun } from "mobx";

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

interface Props {
  context?: App.Context;
  client?: SocketClient;
}

export const Messages = ({ context, client }: Props) => {
  React.useEffect(() => {
    const id = client.send({
      query: "MonitorInstitution",
      isSubscription: true,
      receiver: (data: any) => {
        console.log(data);
      },
    });
    return () => {
      client.unsubscribe("MonitorInstitution", id);
    };
  }, []);
  return (
    <Observer>
      {() =>
        context.store.messages.length ? (
          <List>
            {context.store.messages.map((m, i) => (
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

export const Highlight = observer(({ context }: Props) => {
  return (
    <SyntaxHighlighter language="cs" style={docco} showLineNumbers={true}>
      {(context.store.compiledCode || "").trim()}
    </SyntaxHighlighter>
  );
});

Highlight.displayName = "HighlightView";

export const Errors = observer(({ context }: Props) => (
  <>
    {context.store.errors.length ? (
      <List>
        {context.store.errors.map((m, i) => {
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
));

Errors.displayName = "ErrorsView";

@inject("context", "client")
@observer
export class MiddleView extends React.Component<Props> {
  state = { activeItem: "messages" };

  handleItemClick = (_e: any, { name }: any) =>
    this.setState({ activeItem: name });

  isActive(name: string) {
    return name === this.state.activeItem;
  }

  downloadCode = () => {
    let myWindow = window.open("");
    myWindow.document.write(
      "<pre>" +
        this.props.context.store.compiledCode
          .replace(/>/g, "&gt;")
          .replace(/</g, "&lt;") +
        "</pre>" || "No code was compiled"
    );
    let range = myWindow.document.createRange();
    range.selectNode(myWindow.document.getElementById("hello"));
    myWindow.getSelection().addRange(range);
    (myWindow as any).select();
  };

  render() {
    const { activeItem } = this.state;
    const { context, client } = this.props;
    return (
      <div>
        <Menu pointing secondary compact fluid inverted color="grey">
          <Menu.Item
            name="messages"
            content="Messages"
            icon="mail"
            active={activeItem === "messages"}
            onClick={this.handleItemClick}
          />
          <Menu.Item
            name="errors"
            content={`${context.store.errors.length} Error${
              context.store.errors.length === 1 ? "" : "s"
            }`}
            icon="bug"
            active={activeItem === "errors"}
            onClick={this.handleItemClick}
          />
          <Menu.Item
            name="compiled"
            content="Code"
            icon="code"
            active={activeItem === "compiled"}
            onClick={this.handleItemClick}
          />
          <Menu.Menu position="right">
            <Menu.Item icon="download" onClick={this.downloadCode} />
            <Menu.Item content="Clear" icon="remove" />
          </Menu.Menu>
        </Menu>

        {this.isActive("messages") && (
          <div className={pane}>
            <Messages context={context} client={client} />
          </div>
        )}
        {this.isActive("errors") && (
          <div className={pane}>
            <Errors context={context} />
          </div>
        )}
        {this.isActive("compiled") && (
          <div className={pane + " complete"}>
            <Highlight context={context} />
          </div>
        )}
      </div>
    );
  }
}
