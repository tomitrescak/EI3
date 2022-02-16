import { toJS } from 'mobx';
import { observer, useLocalObservable } from 'mobx-react';
import React, { useEffect } from 'react';

import { Checkbox, DatePicker, Input, Radio, Select, TextArea } from '../FormComponents';
import { Formix } from '../Formix';
import { useFormixContext } from '../library/formContext';
import { isRequired, minLength } from '../library/validation';

const IsValid = () => {
  const context = useFormixContext();
  const [valid, setValid] = React.useState(true);

  return (
    <div>
      Valid: {valid.toString()}
      <button type="button" onClick={() => setValid(context.isValid())}>
        Validate
      </button>
    </div>
  );
};

const IsDirty = observer(() => {
  const context = useFormixContext();
  const [dirty, setDirty] = React.useState(true);

  useEffect(() => {
    setDirty(context.isDirty());
  });

  let currentDirty = context.isDirty();

  return <div>Dirty: {currentDirty.toString()}</div>;
});

export const SignupForm = observer(() => {
  const state = useLocalObservable(() => ({
    users: [
      {
        date: null,
        ingredients: [],
        sex: 'male',
        toggled: true,
        name: {
          firstName: 'Tom',
          lastName: '',
          friends: {
            first: '',
            second: 'ff'
          }
        },
        address: {
          street: ''
        }
      },
      {
        name: {
          firstName: 'Michael',
          lastName: 'Trescak'
        }
      }
    ],
    selected: 0
  }));

  // Notice that we have to initialize ALL of fields with values. These
  // could come from props, but since we don't want to prefill this form,
  // we just use an empty string. If you don't do this, React will yell
  // at you.

  const obj = state.users[state.selected];

  console.log('Rendering form');
  return (
    <div style={{ padding: 8, background: 'white' }} className="ui form">
      {/* <ul>
        {state.users.map((u, i) => (
          <li key={i} onClick={() => (state.selected = i)}>
            User {i} {u.name.firstName}
          </li>
        ))}
      </ul> */}
      <span>Selected {state.selected}</span>
      <Formix initialValues={obj}>
        <>
          <Formix
            initialValues={obj.name}
            validationSchema={{
              firstName: [isRequired, minLength(5)]
            }}
            onSubmit={values => {
              setTimeout(() => {
                alert(JSON.stringify(values, null, 2));
              }, 400);
            }}
          >
            {props => (
              <form className="ui form">
                <Input name="firstName" label="First Name" type="text" />
                <Input name="lastName" label="Last Name" type="text" />

                <Formix
                  initialValues={obj.name.friends!}
                  onSubmit={() => {}}
                  validationSchema={{
                    first: [isRequired, minLength(5)]
                  }}
                >
                  <>
                    <Input name="first" label="First Friend" type="text" />
                    <Input name="second" label="Second Friend" type="text" />
                  </>
                </Formix>

                {/* <button type="submit">Submit</button> */}
                <IsValid />
                <IsDirty />
              </form>
            )}
          </Formix>

          {/* <Formix initialValues={obj.address}>
            <form className="ui form">
              <Input id="street" name="street" label="Street" type="text" />
            </form>
          </Formix> */}

          <DatePicker name="date" label="Date" format="dd MMM yyyy" />
          <Input name="name.firstName" label="Name" type="text" />
          <TextArea name="name.firstName" label="Name" type="text" />

          <Checkbox label="Oregano" name="ingredients" value="oregano" />
          <Checkbox label="Tomatoes" name="ingredients" value="tomatoes" />
          <hr />
          <Checkbox label="Toggled" name="toggled" />
          <hr />
          <Radio label="Male" name="sex" value="male" />
          <Radio label="Female" name="sex" value="female" />
          <hr />
          <Select
            selection
            label="Country"
            name="country"
            options={[
              { value: 'it', text: 'Italy' },
              { value: 'sk', text: 'Slovakia' }
            ]}
          />
          <pre>{JSON.stringify(toJS(obj), null, 2)}</pre>
        </>
      </Formix>
    </div>
  );
});

export default {
  title: 'Form'
};
