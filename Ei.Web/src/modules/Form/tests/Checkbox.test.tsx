import { fireEvent, renderStory, screen } from 'library/helpers/tests';
import { observable } from 'mobx';

import { ArrayTarget, SingleTarget } from './Checkbox.stories';

describe('Form > Checkbox', () => {
  it('renders and changes for single value', () => {
    const state = observable({
      toggled: false
    });
    renderStory(() => <SingleTarget state={state} />);
    // loading state
    const box = screen.getByLabelText('Toggled');

    expect(box).not.toBeChecked();

    fireEvent.click(box);

    expect(box).toBeChecked();
    expect(state.toggled).toBe(true);
  });

  it('renders and changes for array value', () => {
    const state = observable({
      ingredients: ['oregano']
    });
    renderStory(() => <ArrayTarget state={state} />);

    // loading state
    const box = screen.getByLabelText('Oregano');
    expect(box).toBeChecked();

    fireEvent.click(box);

    expect(box).not.toBeChecked();

    fireEvent.click(screen.getByLabelText('Tomatoes'));
    fireEvent.click(screen.getByLabelText('Oregano'));

    expect(state.ingredients).toEqual(['tomatoes', 'oregano']);
  });
});
