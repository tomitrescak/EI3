import React, { useEffect } from 'react';
import { Formix } from '../Formix';

import { observer, useLocalObservable } from 'mobx-react';
import { Select, AsyncSelect } from '../FormComponents';
import { isRequired, gte } from '../library/validation';
import { Form } from '../Form';
import { FormErrors } from '../FormErrors';

const makes = [
  { text: 'Honda', value: 'honda' },
  { text: 'Mazda', value: 'Mazda' }
];

export const Standard = observer(
  ({
    state = useLocalObservable(() => ({
      make: null
    }))
  }: Any) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          make: [isRequired]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <Select selection label="Make" name="make" placeholder="Make" options={makes} />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export const Asynchronous = observer(
  ({
    state = useLocalObservable(() => ({
      make: null
    }))
  }: Any) => {
    return (
      <Formix
        initialValues={state}
        validationSchema={{
          make: [isRequired]
        }}
      >
        <Form className="ui form">
          <FormErrors />
          <AsyncSelect
            selection
            label="Make"
            name="make"
            placeholder="Make"
            asyncOptions={() =>
              new Promise(resolve => {
                setTimeout(() => resolve(makes), 50);
              })
            }
          />
          <button type="submit">Submit</button>
        </Form>
      </Formix>
    );
  }
);

export default {
  title: 'Form/Select',
  style: {
    padding: 32
  }
};
