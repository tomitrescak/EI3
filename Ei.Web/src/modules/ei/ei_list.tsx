import * as React from 'react';

import { observer } from 'mobx-react';
import { Button, Header, List, Segment } from 'semantic-ui-react';
import { style } from 'typestyle';
import { Link } from '../../config/router';

const homeStyle = style({
  margin: '12px!important'
});

interface StoredEi {
  id: string;
  name: string;
}

interface Props {
  context: App.Context;
}

export const EiList = observer(({ context }: Props) => {
  const eiString = localStorage.getItem('eis') || '[]';
  const eis: StoredEi[] = JSON.parse(eiString);

  return (
    <Segment className={homeStyle}>
      <Header content="Your Institutions" dividing icon="home" />
      {eis.length === 0 && <span>No Institutions</span>}
      <List>
        {eis.map((e, i) => (
          <List.Item
            icon="home"
            key={i}
            as={Link}
            to={`/ei/${e.name.toUrlName()}/${e.id}`}
            content={e.name}
            action={() => context.store.viewStore.showEi(e.id, e.name.toUrlName())}
          />
        ))}
      </List>

      <Button content="Create Institution" icon="plus" />
    </Segment>
  );
});
