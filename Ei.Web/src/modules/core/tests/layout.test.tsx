import * as React from 'react';

import { mount } from 'enzyme';

import { Layout } from '../layout';


const Component = () => <div>Rendered</div>

describe('Layout @view', () => {
  it('renders child component', () => {
    expect(true).toBe(true);
    const m = mount(<Layout><Component /><div>ee</div></Layout>);
    
    expect(m.html()).toMatchSnapshot(); 
  });
});
