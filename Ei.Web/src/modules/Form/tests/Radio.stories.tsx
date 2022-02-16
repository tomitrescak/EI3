import React, { useEffect } from 'react';
import { Formix } from '../Formix';

import { observer, useLocalObservable } from 'mobx-react';
import { Checkbox, Radio } from '../FormComponents';
import { isRequired } from '../library/validation';
import { Form } from '../Form';
import { FormErrors } from '../FormErrors';

export const Standard = observer(
  ({
    state = useLocalObservable(() => ({
      toggled: 'male'
    }))
  }) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          toggled: [isRequired]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <Radio label="Male" name="toggled" value="male" />
          <Radio label="Female" name="toggled" value="female" />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export const Toggle = observer(
  ({
    state = useLocalObservable(() => ({
      toggled: 'male'
    }))
  }) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          toggled: [isRequired]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <Radio label="Other" toggle name="toggled" value="other" />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export default {
  title: 'Form/Radio',
  style: {
    padding: 32
  }
};
