import React, { useEffect } from 'react';
import { Formix } from '../Formix';

import { observer, useLocalObservable } from 'mobx-react';
import { Input } from '../FormComponents';
import { isRequired, gte } from '../library/validation';
import { Form } from '../Form';
import { FormErrors } from '../FormErrors';

export const Standard = observer(
  ({
    state = useLocalObservable(() => ({
      name: null,
      salary: 0
    }))
  }: Any) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          name: [isRequired],
          salary: [gte(0)]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <Input label="Name" name="name" placeholder="Your name" />
          <Input type="number" label="Salary" inputLabel="$" name="salary" />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export default {
  title: 'Form/Input',
  style: {
    padding: 32
  }
};
