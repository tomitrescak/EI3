import * as PIXI from 'pixi.js';
import * as React from 'react';

// AVISHKA
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import { field, Form, FormState, Input } from 'semantic-ui-mobx';
import { Button } from 'semantic-ui-react';

// // you need to create a model of agent
// class Agent {
// 	x: number;
//   y: number;
  
// 	constructor (x: number, y: number){
//     this.x = x;
//     this.y = y;
//   }
// 	moveToLocation(x: number, y: number) {
// 		return; 
// 	}
// }

// test
// let agent = new Agent(10, 30);
// agent.moveToLocation(100, 300);

// then we need to also have a map of environment
class EnvironmentObject {
	name: string;
	image: string;
}

class Project {
	objects: EnvironmentObject[];
}

class Environment {
  objects: { [index: string]: PIXI.Sprite } = {};
  stage: PIXI.Container;

  constructor(stage: PIXI.Container) {
    this.stage = stage;
  }

	addObject(id: string, name: string, x: number, y:number) { 
    let blob = new PIXI.Sprite(PIXI.loader.resources['/images/blob.png'].texture);
    blob.name = 'Blob';
    blob.vy = 1;
    blob.x = x;
    blob.y = y;

    // rememeber object for further manipulation
    this.objects[id] = blob;

    // add to canvas
    this.stage.addChild(blob);
  }
  
  addAgent(id: string, x: number, y: number) {
    let agent = new PIXI.Sprite(PIXI.loader.resources['/images/explorer.png'].texture);
    agent.name = 'cat';
    agent.vy = 1;

    // rememeber object for further manipulation
    this.objects[id] = agent;

    // add to canvas
    this.stage.addChild(agent);
  }

  removeObject(id: string){
    this.stage.removeChild(this.objects[id]);
  }
}


export class ExecutionView extends React.Component {
  canvas: HTMLElement;
  app: PIXI.Application;

  // REACT METHODS
  render() {
    return (
      <>
        <div ref={n => (this.canvas = n)} id="pixiCanvas" />
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
    PIXI.loader.add('/images/explorer.png').add('/images/dungeon.png').add('/images/blob.png').load(this.setup);

    this.app = app;
    this.canvas.appendChild(app.view);

    // test
    let project = new Project();
    project.objects = [{
      name: 'Blob',
      image: '/images/blob.png'
    }];
    
  }

  // CLASS METHODS

  setup = () => {
    let background = new PIXI.Sprite(PIXI.loader.resources['/images/dungeon.png'].texture);
    // Add the cat to the stage
    this.app.stage.addChild(background);

    // add the rest

    let e = new Environment(this.app.stage);
    e.addObject('id1', 'Blob', 10, 10); // displays the object
    // e.removeObject('id1'); // removes the object
    e.addAgent('agent1', 20, 30); // adds agent */
  }
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
        <div style={{background: 'salmon'}}>{data.testField}</div>

        <div>Let's do arrays</div>
        { 
          data.testArray.map((element, index) => (
            <div key={index}>
              <div >[{index}]: { element }</div>
              <Button color="red" icon="trash" onClick={() => data.testArray.splice(index, 1)} />
            </div>
          )) 
        }
        <Input owner={data.fields.newArrayMember} label="New Member" />
        <Button  icon="plus" color="blue" onClick={() => data.testArray.push(data.newArrayMember) } />
      </Form>
    )
  }
}
