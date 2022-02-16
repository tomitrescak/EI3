import React, { useEffect } from 'react';
import { Formix } from '../Formix';

import { observer, useLocalObservable } from 'mobx-react';
import { DatePicker } from '../FormComponents';
import { isRequired, gte } from '../library/validation';
import { Form } from '../Form';
import { FormErrors } from '../FormErrors';

export const Standard = observer(
  ({
    state = useLocalObservable(() => ({
      dob: null
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
          <DatePicker label="Born" name="dob" placeholder="Your birth date" />
          <DatePicker label="Born" name="dob" placeholder="Your birth date" format="dd/MM/yyyy" />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export default {
  title: 'Form/Date',
  style: {
    padding: 32
  }
};
