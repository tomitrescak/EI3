import { Icon, List, Message } from 'semantic-ui-react';

import styled from '@emotion/styled';

import { ComponentOption } from './components/actionsProviderEditor';
import { ComponentDao, ExperimentDao, GameObjectDao } from './experiment_model';

const ListHeader = styled(List.Header)`
  padding: 4px 16px;

  &.secondary {
    background: #dedede;
  }

  &.tertiary {
    background: #cecece;
    padding: 4px 8px;
  }
  font-weight: bold;
  width: 100%;

  :first-of-type {
    margin-top: 0px;
  }
`;

export const ComponentEditor = ({
  currentComponent,
  componentDefinition,
  experiment,
  gameObject,
  index,
  owner,
  reorder,
}: {
  currentComponent: ComponentOption;
  componentDefinition: ComponentDao;
  experiment: ExperimentDao;
  gameObject: GameObjectDao;
  index: number;
  owner: ComponentDao[];
  reorder: boolean;
}) => (
  <>
    <ListHeader
      className={reorder ? "tertiary" : "secondary"}
      style={{ paddingRight: 8 }}
    >
      <div style={{ display: "flex" }}>
        <span style={{ flex: 1 }}>
          {currentComponent == null
            ? "Missing Component"
            : currentComponent.text}
        </span>
        {reorder && (
          <>
            {index > 0 && (
              <Icon
                name="chevron up"
                title="Remove Component"
                style={{
                  color: "#999",
                  cursor: "pointer",
                }}
                onClick={() => {
                  let original = owner.splice(index, 1)[0];
                  owner.splice(index - 1, 0, original);
                }}
              />
            )}
            {index < owner.length - 1 && (
              <Icon
                name="chevron down"
                title="Remove Component"
                style={{
                  color: "#999",
                  cursor: "pointer",
                }}
                onClick={() => {
                  let original = owner.splice(index, 1)[0];
                  owner.splice(index + 1, 0, original);
                }}
              />
            )}
          </>
        )}
        {(reorder || index > 0) && (
          <Icon
            name="close"
            title="Remove Component"
            style={{
              color: "#999",
              cursor: "pointer",
            }}
            onClick={() => owner.splice(index, 1)}
          />
        )}
      </div>
    </ListHeader>
    <List.Item>
      {currentComponent == null ? (
        <Message
          negative
          style={{ margin: 8 }}
          key={componentDefinition.Id}
          content={`Component does not exist! ${componentDefinition.$type}`}
        />
      ) : (
        <>
          <currentComponent.editor
            component={componentDefinition as any}
            experiment={experiment}
            gameObject={gameObject}
          />
          {/* Check all dependencies */}
          {(currentComponent.globalDependencies || [])
            .filter((f) =>
              experiment.GameObjects.every((g) =>
                g.Components.every((c) => c.$type !== f.type)
              )
            )
            .map((t, i) => (
              <div style={{ padding: 8 }}>
                <Message
                  visible
                  error
                  header="Missing Global Dependency"
                  content={t.text}
                  key={i}
                />
              </div>
            ))}
          {(currentComponent.componentDependencies || [])
            .filter((f) =>
              gameObject.Components.every((c) => c.$type !== f.type)
            )
            .map((t, i) => (
              <Message
                error
                header="Missing Component"
                content={t.text}
                key={i}
              />
            ))}
        </>
      )}
    </List.Item>
  </>
);
