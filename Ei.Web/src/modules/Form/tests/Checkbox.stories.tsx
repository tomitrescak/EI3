import React, { useEffect } from 'react';
import { Formix } from '../Formix';

import { observer, useLocalObservable } from 'mobx-react';
import { Checkbox } from '../FormComponents';
import { isRequired } from '../library/validation';
import { Form } from '../Form';
import { FormErrors } from '../FormErrors';

export const SingleTarget = observer(
  ({
    state = useLocalObservable(() => ({
      toggled: null
    }))
  }: Any) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          toggled: [isRequired]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <Checkbox label="Toggled" name="toggled" />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export const ArrayTarget = observer(
  ({
    state = useLocalObservable(() => ({
      ingredients: ['oregano']
    }))
  }) => {
    return (
      <>
        <Formix initialValues={state}>
          <div className="ui form">
            <Checkbox label="Oregano" name="ingredients" value="oregano" />
            <Checkbox label="Tomatoes" name="ingredients" value="tomatoes" />
            <button>Submit</button>
          </div>
        </Formix>
      </>
    );
  }
);

export default {
  title: 'Form/Checkbox',
  style: {
    padding: 32
  }
};
