import { observer } from "mobx-react";
import React from "react";
// import { SketchPicker } from 'react-color';
import { Form } from "semantic-ui-react";

import styled from "@emotion/styled";

import { ErrorLabel } from "./FormComponents";
import { useField } from "./library/formHooks";
import { FieldInputProps } from "./library/types";

const Swatch = styled.div`
  padding: 5px;
  background: #fff;
  border-radius: 1px;
  box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.1);
  display: inline-block;
  width: 100%;
  cursor: pointer;
`;

const Color = styled.div`
  width: 100%;
  height: 26px;
  border-radius: 2px;
`;

const PopOver = styled.div`
  position: absolute;
  z-index: 2;
`;

const Cover = styled.div`
  position: fixed;
  top: 0px;
  right: 0px;
  bottom: 0px;
  left: 0px;
`;

type Props = {
  field: FieldInputProps<any>;
};

export class ColorPickerControl extends React.Component<Props> {
  state = {
    displayColorPicker: false,
    hex: this.props.field.value,
  };

  handleClick = () => {
    this.setState({ displayColorPicker: !this.state.displayColorPicker });
  };

  handleClose = () => {
    this.setState({ displayColorPicker: false });

    this.props.field.onChange({
      target: { value: this.state.hex, name: this.props.field.name },
    } as Any);
  };

  handleChange = (color: { hex: string }) => {
    this.setState({ hex: color.hex });
  };

  render() {
    const background = this.state.hex;

    return (
      <>
        <Swatch onClick={this.handleClick}>
          <Color style={{ background }} />
        </Swatch>
        {this.state.displayColorPicker ? (
          <PopOver>
            <Cover onClick={this.handleClose} />
            {/* <SketchPicker color={this.state.hex} onChange={this.handleChange} /> */}
          </PopOver>
        ) : null}
      </>
    );
  }
}

export const ColorPicker = observer(
  ({
    label,
    style,
    width,
    className,
    inputLabel,
    id,
    ...props
  }: {
    label: string;
    id?: string;
    width: Any;
    style?: Any;
    className?: string;
    name: string;
    inputLabel?: string | React.ReactNode;
  }) => {
    const [field, meta] = useField(props);
    return (
      <Form.Field
        error={meta.touched && meta.error ? true : false}
        width={width}
        style={style}
        className={className}
      >
        {label && <label htmlFor={id || props.name}>{label}</label>}
        <ColorPickerControl field={field} />
        {meta.touched && meta.error ? (
          <ErrorLabel message={meta.error} />
        ) : null}
      </Form.Field>
    );
  }
);

export default ColorPicker;
