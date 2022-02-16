export type ComponentDao = {
  [index: string]: any;
  Id: string;
  $type: string;
};

export type GameObjectDao = {
  Id: string;
  Name: string;
  Components: ComponentDao[];
};

export type ExperimentDao = {
  Id: string;
  Name: string;
  GameObjects: GameObjectDao[];
};
