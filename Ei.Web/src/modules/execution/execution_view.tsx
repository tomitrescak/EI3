import * as PIXI from 'pixi.js';
import * as React from 'react';

import { observable } from 'mobx';
import { Line } from 'react-chartjs-2';

class EnvironmentObject {
  name: string;
  image: string;
}

interface IServerObject {
  id: number;
  name: string;
  x: number;
  y: number;
}

class StatisticModel {
  @observable statisticPoints = 0;

  data = {
    labels: [] as any[],
    datasets: [
      {
        label: 'Hunger Level Dataset',
        data: [] as any[],
        fill: false,
        backgroundColor: 'rgba(75,192,192,0.4)',
        borderColor: 'rgba(75,192,192,1)',
        borderCapStyle: 'butt',
        borderDash: [] as any[],
        borderDashOffset: 0.0,
        borderJoinStyle: 'miter',
        lineTension: 0,
        pointBorderColor: 'rgba(75,192,192,1)',
        pointBackgroundColor: '#fff',
        pointBorderWidth: 1,
        pointHoverRadius: 5,
        pointHoverBackgroundColor: 'rgba(75,192,192,1)',
        pointHoverBorderColor: 'rgba(220,220,220,1)',
        pointHoverBorderWidth: 2,
        pointRadius: 1,
        pointHitRadius: 10
      }
    ]
  };

  options = {
    title: {
      display: true,
      text: 'Hunger Level'
    },
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      yAxes: [
        {
          ticks: {
            max: 100,
            min: 0,
            stepSize: 10
          },
          scaleLabel: {
            display: true,
            labelString: 'Hunger Value'
          }
        }
      ],
      xAxes: [
        {
          scaleLabel: {
            display: true,
            labelString: 'Time (Seconds)'
          }
        }
      ]
    },
    elements: {
      line: {
        tension: 0 // disables bezier curves
      }
    }
  };
}

class ServerModel {
  statistics: StatisticModel;
  app: PIXI.Application;
  objects: { [index: string]: IdentifiableSprite } = {};
  theme = {
    Blob: '/images/blob.png',
    Explorer: '/images/explorer.png'
  };
  maxId = 0;

  constructor() {
    // init pixi
    let type = 'WebGL';
    if (!PIXI.utils.isWebGLSupported()) {
      type = 'canvas';
    }

    // Create a Pixi Application
    let app = new PIXI.Application({
      width: 513,
      height: 513,
      backgroundColor: 0xffffff
    });

    PIXI.loader.reset();
    PIXI.loader
      .add('/images/explorer.png')
      .add('/images/dungeon.png')
      .add('/images/blob.png')
      .load(this.setup);

    this.app = app;

    this.statistics = new StatisticModel();
  }

  get themeKeys() {
    return Object.keys(this.theme);
  }

  setup = () => {
    let background = new PIXI.Sprite(PIXI.loader.resources['/images/dungeon.png'].texture);
    this.app.stage.addChild(background);

    // init items
    // this.loadFromServer([
    //   { id: 1, name: 'Blob', x: 20, y: 100 },
    //   { id: 2, name: 'Blob', x: 60, y: 120 },
    //   { id: 3, name: 'Blob', x: 120, y: 200 },
    //   { id: 4, name: 'Blob', x: 80, y: 100 },
    //   { id: 5, name: 'Blob', x: 170, y: 300 }
    // ]);

    for (let i = 0; i < 200; i++) {
      let obj = this.getRandomKey(this.theme);
      this.addNew({
        id: this.maxId + 1,
        name: obj,
        x: Math.random() * 400,
        y: Math.random() * 400
      });
    }

    // start random execution
    setInterval(this.randomAction, 50);
  };

  getRandomInt(max: number) {
    return Math.floor(Math.random() * Math.floor(max));
  }

  getRandomItem<P>(obj: { [index: string]: P }): P {
    let keys = Object.keys(obj);
    if (keys.length > 0) {
      return obj[keys[this.getRandomInt(keys.length)]];
    }
    return null;
  }

  getRandomKey(obj: any): string {
    let keys = Object.keys(obj);
    return keys[this.getRandomInt(keys.length)];
  }

  randomAction = () => {
    let rnd = this.getRandomInt(1);

    switch (rnd) {
      case 0:
        let movable = this.getRandomItem(this.objects);
        model.move(movable.id, Math.random() * 400, Math.random() * 400, Math.random() * 5000);
        break;
      case 1:
        let obj = this.getRandomKey(this.theme);
        this.addNew({
          id: this.maxId + 1,
          name: obj,
          x: Math.random() * 400,
          y: Math.random() * 400
        });
        break;
      case 2:
        let item = this.getRandomItem(this.objects);
        if (item) {
          this.remove(item.id);
        }
        break;
    }
  };

  loadFromServer(items: IServerObject[]) {
    // do your magic
    // Access array parameters here
    for (let item of items) {
      this.addNew(item);
    }
  }

  addNew(item: IServerObject) {
    // do your magic
    let agent = new PIXI.Sprite(
      PIXI.loader.resources[this.theme[item.name]].texture
    ) as IdentifiableSprite;
    this.objects[item.id] = agent;
    agent.id = item.id;
    agent.name = item.name;
    agent.x = item.x;
    agent.y = item.y;

    this.app.stage.addChild(agent);

    // tslint:disable-next-line:no-console
    // console.log(
    //   'Adding Object' +
    //     '\nObject Id: ' +
    //     agent.id +
    //     '\nObject Name: ' +
    //     agent.name +
    //     '\nPosition X: ' +
    //     agent.x +
    //     '\nPosition Y: ' +
    //     agent.y
    // );

    if (this.maxId < agent.id) {
      this.maxId = agent.id;
    }
  }

  remove(id: number) {
    // do your magic
    this.app.stage.removeChild(this.objects[id]);
  }

  move(id: number, x: number, y: number, timeInMs: number) {
    // do your magic
    // time paramater, animate: move(to, from, time), move to (x, y)
    // utilise the app ticker functionality and set the letiable delta as the velocity which equals
    // the total number of frames divided by the distance. To calculate the total frames, times the
    // frame rate per second with the specified time to reach the x2, y2 position. Distance equals,
    // the square root of x1, x2 to the power of two plus x2, y2 to the power of two.
    let timeInSeconds = timeInMs / 1000;
    let framesPerSec = 60;
    let distance = Math.sqrt(
      Math.pow(x - this.objects[id].x, 2) + Math.pow(y - this.objects[id].y, 2)
    );
    let totalFrames = framesPerSec * timeInSeconds;
    let delta = distance / totalFrames;
    this.app.ticker.add(() => {
      if (this.objects[id].x < x) {
        this.objects[id].x += delta;
      } else {
        this.objects[id].x += 0;
      }

      if (this.objects[id].y < y) {
        this.objects[id].y += delta;
      } else {
        this.objects[id].y += 0;
      }
    });
  }
}

const model = new ServerModel();

interface IdentifiableSprite extends PIXI.Sprite {
  id: number;
}

export class ExecutionView extends React.Component {
  canvas: HTMLElement;

  // REACT METHODS
  render() {
    return (
      <>
        <div ref={n => (this.canvas = n)} id="pixiCanvas" />
        <hr />
        <LineChart model={model} />
      </>
    );
  }
  componentDidMount() {
    this.canvas.appendChild(model.app.view);
  }

  // CLASS METHODS
}

interface ChartProps {
  model: ServerModel;
}

export class LineChart extends React.Component<ChartProps> {
  line: Line;

  componentDidMount() {
    setInterval(() => {
      this.props.model.statistics.data.labels.push(
        this.props.model.statistics.data.labels.length / 2
      );
      this.props.model.statistics.data.datasets[0].data.push(Math.floor(Math.random() * 100));
      this.line.chartInstance.update();
    }, 500);
  }

  render() {
    return (
      <div>
        <Line
          ref={node => (this.line = node)}
          options={this.props.model.statistics.options}
          data={this.props.model.statistics.data}
          width={600}
          height={500}
        />
      </div>
    );
  }
}
