import sinon from 'sinon';

import { renderStory, screen, fireEvent } from 'library/helpers/tests';
import { Standard, Asynchronous } from './Select.stories';
import { observable } from 'mobx';

describe('Form > Select', () => {
  it('renders and changes for single value', () => {
    const state = observable({
      make: '',
      salary: 0
    });
    renderStory(() => <Standard state={state} />);

    // check validations
    fireEvent.click(screen.getByText('Submit'));

    expect(screen.getByText('This field is required!')).toBeInTheDocument();

    // changes
    fireEvent.click(screen.getByText('Honda'));
    expect(screen.queryByText('This field is required!')).not.toBeInTheDocument();

    expect(state.make).toBe('honda');
  });

  it('renders and changes for single value', async () => {
    const state = observable({
      make: '',
      salary: 0
    });
    renderStory(() => <Asynchronous state={state} />);

    // check validations
    fireEvent.click(screen.getByText('Submit'));

    expect(screen.getByText('This field is required!')).toBeInTheDocument();

    // waits to load
    const honda = await screen.findByText('Honda');
    fireEvent.click(honda);
    expect(screen.queryByText('This field is required!')).not.toBeInTheDocument();

    expect(state.make).toBe('honda');
  });
});
