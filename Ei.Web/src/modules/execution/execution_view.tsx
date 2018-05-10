import * as PIXI from 'pixi.js';
import * as React from 'react';

// AVISHKA
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import { field, Form, FormState, Input } from 'semantic-ui-mobx';
import Item, { Button } from 'semantic-ui-react';

class EnvironmentObject {
  name: string;
  image: string;
}

class Project {
  objects: EnvironmentObject[];
}

interface IServerObject {
  id: string;
  name: string;
  x: number;
  y: number;
}

export class ExecutionView extends React.Component {
  canvas: HTMLElement;
  app: PIXI.Application;
  objects: { [index: string]: PIXI.Sprite } = {};

  loadFromServer(items: IServerObject[]) {
    // do your magic
    // Access array parameters here
    for (let item of items) {
      let object = new PIXI.Sprite(PIXI.loader.resources['/images/blob.png'].texture);
      this.objects[item.id] = object;
        object.id = item.id; 
        object.name = item.name;
        object.x = item.x;
        object.y = item.y; 
        this.app.stage.addChild(object);
      };
  }

  addNew(item: IServerObject) {
    // do your magic
    let agent = new PIXI.Sprite(PIXI.loader.resources['/images/explorer.png'].texture);
    this.objects[item.id] = agent;
    agent.id = item.id;
    agent.name = item.name;
    agent.x = item.x;
    agent.y = item.y;

    this.app.stage.addChild(agent);

    alert('Adding Object' + '\nObject Id: ' + agent.id + '\nObject Name: ' + agent.name +
          '\nPosition X: ' + agent.x + '\nPosition Y: ' + agent.y);
  }

  remove(id: string) {
    // do your magic
    this.app.stage.removeChild(this.objects[id]);
  }

  move(id: string, x: number, y: number, timeInMs: number) {
    // do your magic
    // time paramater, animate: move(to, from, time), move to (x, y)
    // utilise the app ticker functionality and set the variable delta as the velocity which equals
    // the total number of frames divided by the distance. To calculate the total frames, times the 
    // frame rate per second with the specified time to reach the x2, y2 position. Distance equals,
    // the square root of x1, x2 to the power of two plus x2, y2 to the power of two.
      let timeInSeconds = timeInMs / 1000; 
      let framesPerSec = 60;
      let distance = Math.sqrt((Math.pow(x - this.objects[id].x, 2)) + (Math.pow(y - this.objects[id].y, 2)));
      let totalFrames = framesPerSec * timeInSeconds; 
      let delta = distance / totalFrames;
        this.app.ticker.add(()=> {
          if(this.objects[id].x < x){
            this.objects[id].x += delta;
          }
          else {
            this.objects[id].x += 0;
          }

          if (this.objects[id].y < y){
            this.objects[id].y += delta;
          }
          else {
            this.objects[id].y += 0;
          }

        });
  }

  // REACT METHODS
  render() {
    return (
      <>
        <div ref={n => (this.canvas = n)} id="pixiCanvas" />
        <hr />
        <button
          onClick={() =>
            this.loadFromServer([
              { id: '1', name: 'object1', x: 20, y: 100 },
              { id: '2', name: 'object1', x: 60, y: 120 },
              { id: '3', name: 'object3', x: 120, y: 200 },
              { id: '4', name: 'object2', x: 80, y: 100 },
              { id: '5', name: 'object1', x: 170, y: 300 }
            ])
          }
        >
          Load From Server
        </button>
        <button onClick={() => this.addNew({ id: '5', name: 'object1', x: 30, y: 222 })}>Add New Object</button>
        <button onClick={() => this.remove('4')}>Remove Object</button>
        <button onClick={() => this.move('2', 240, 300, 500)}>Move Object</button>

        <ReactiveForm />
      </>
    );
  }
  componentDidMount() {
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
    this.canvas.appendChild(app.view);

    // test
    let project = new Project();
    project.objects = [
      {
        name: 'Blob',
        image: '/images/blob.png'
      }
    ];
  }

  // CLASS METHODS

  setup = () => {
    let background = new PIXI.Sprite(PIXI.loader.resources['/images/dungeon.png'].texture);
    // Add the cat to the stage
    this.app.stage.addChild(background);
  };
}

///// AVISHKA

class Reactive extends FormState {
  @field testField = '';
  @observable testArray: string[] = [];

  @field newArrayMember = '';
}

let data = new Reactive();

@observer
class ReactiveForm extends React.Component {
  render() {
    return (
      <Form>
        <Input owner={data.fields.testField} label="Test Text (.fields used only with owner)" />
        <div style={{ background: 'salmon' }}>{data.testField}</div>

        <div>Let's do arrays</div>
        {data.testArray.map((element, index) => (
          <div key={index}>
            <div>
              [{index}]: {element}
            </div>
            <Button color="red" icon="trash" onClick={() => data.testArray.splice(index, 1)} />
          </div>
        ))}
        <Input owner={data.fields.newArrayMember} label="New Member" />
        <Button icon="plus" color="blue" onClick={() => data.testArray.push(data.newArrayMember)} />
      </Form>
    );
  }
}
