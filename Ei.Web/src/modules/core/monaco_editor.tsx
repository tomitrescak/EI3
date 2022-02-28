import * as React from "react";

import { IObservableArray } from "mobx";
import { Observer, observer } from "mobx-react";
import Editor, { OnMount } from "@monaco-editor/react";
import { Property } from "../ei/property_model";
import styled from "@emotion/styled";

interface Props {
  update: (value: string) => void;
  value: () => string;
  height?: number;
  i?: IObservableArray<Property>;
  w?: IObservableArray<Property>;
  o?: IObservableArray<Property>;
  r?: IObservableArray<Property>;
  a?: IObservableArray<Property>;
}

const EditorContainer = styled.div`
  textarea {
    opacity: 0;
  }
`;

export class CodeEditor extends React.Component<Props> {
  monaco: any;
  previous: any;
  editor: any;
  holder: any;

  editorMounted: OnMount = (editor, monaco) => {
    this.editor = editor;
    this.updateDefinitions(this.props, monaco);
  };

  createDefinitions(properties: IObservableArray<Property>) {
    if (!properties) {
      return [];
    }
    return properties.map((p) => ({
      label: p.Name,
      kind: 9, // monaco.languages.CompletionItemKind.Property,
      documentation: "Default: " + p.DefaultValue,
      insertText: p.Name,
    }));
  }

  // componentWillUpdate(nextProps: Props) {
  //   debugger;
  //   this.updateDefinitions(nextProps);
  // }

  updateDefinitions(props: Props, monaco: any = null) {
    if (monaco) {
      this.monaco = monaco;
    }

    const i = this.createDefinitions(props.i);
    const w = this.createDefinitions(props.w);
    const o = this.createDefinitions(props.o);
    const r = this.createDefinitions(props.r);
    const a = this.createDefinitions(props.a);

    if (!this.monaco) {
      return;
    }
    if (this.previous) {
      this.previous.dispose();
    }
    this.previous = this.monaco.languages.registerCompletionItemProvider("*", {
      provideCompletionItems: function (model: any, position: any) {
        // find out if we are completing a property in the 'dependencies' object.
        let textUntilPosition = model.getValueInRange({
          startColumn: 1,
          endLineNumber: position.lineNumber,
          endColumn: position.column,
        });
        if (textUntilPosition.match(/i\.$/)) {
          return { suggestions: i };
        } else if (textUntilPosition.match(/w\.$/)) {
          return { suggestions: w };
        } else if (textUntilPosition.match(/o\.$/)) {
          return { suggestions: o };
        } else if (textUntilPosition.match(/r\.$/)) {
          return { suggestions: r };
        } else if (textUntilPosition.match(/a\.$/)) {
          return { suggestions: a };
        }
        return { suggestions: [] };
      },
    });
  }

  updateDimensions() {
    this.editor.layout();
  }

  componentDidMount() {
    window.addEventListener("resize", this.updateDimensions.bind(this));
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.updateDimensions.bind(this));
  }

  update = (value: string) => {
    // global._monaco_updated = Date.now();
    this.props.update(value);
  };

  render() {
    this.updateDefinitions(this.props);

    const { value } = this.props;
    return (
      <Observer>
        {() => (
          <EditorContainer ref={(node) => (this.holder = node)}>
            <Editor
              height={this.props.height || 200}
              language="csharp"
              options={{
                // lineNumbers: "on",
                automaticLayout: true,
                theme: "vs-dark",
                minimap: { enabled: false },
                quickSuggestions: {
                  other: true,
                  comments: true,
                  strings: true,
                },
                quickSuggestionsDelay: 10,
              }}
              value={value()}
              onChange={this.update}
              onMount={this.editorMounted}
              // editorWillMount={}
              // editorDidMount={this.bindEditor}
            />
          </EditorContainer>
        )}
      </Observer>
    );
  }
}
