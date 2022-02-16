import sinon from 'sinon';

import { renderStory, screen, fireEvent, render, wait } from 'library/helpers/tests';
import { Standard, Toggle } from './Radio.stories';
import { observable } from 'mobx';

describe('Form > Radio', () => {
  it('renders and changes for single value', () => {
    const state = observable({
      toggled: 'male'
    });
    renderStory(() => <Standard state={state} />);
    // loading state
    const box = screen.getByLabelText('Male');

    expect(box).toBeChecked();

    fireEvent.click(box);
    expect(box).not.toBeChecked();

    fireEvent.click(screen.getByLabelText('Female'));
    expect(state.toggled).toBe('female');
  });

  it('renders and changes for array value', () => {
    const state = observable({
      toggled: 'other'
    });
    renderStory(() => <Toggle state={state} />);

    // loading state
    const box = screen.getByLabelText('Other');
    expect(box).toBeChecked();
    fireEvent.click(box);
    expect(box).not.toBeChecked();

    expect(state.toggled).toBe('');
  });
});
