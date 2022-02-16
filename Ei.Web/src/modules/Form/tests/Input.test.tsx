import { fireEvent, renderStory, screen } from 'library/helpers/tests';
import { observable } from 'mobx';

import { Standard } from './Input.stories';

describe('Form > Input', () => {
  it.skip('renders and changes for single value', () => {
    const state = observable({
      name: 'male',
      salary: 0
    });
    renderStory(() => <Standard state={state} />);
    // loading state
    const name = screen.getByLabelText('Name');
    const salary = screen.getByLabelText('Salary');

    expect(name).toHaveAttribute('placeholder', 'Your name');
    fireEvent.change(name, { target: { value: 'Tomas' } });
    fireEvent.change(salary, { target: { value: '-10' } });

    // check value changes and type conversion
    expect(state.salary).toBe(-10);
    expect(state.name).toBe('Tomas');

    // check validations
    fireEvent.blur(salary);

    expect(screen.getByText('Must be >= 0')).toBeInTheDocument();
  });
});
