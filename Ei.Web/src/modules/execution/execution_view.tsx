import * as PIXI from 'pixi.js';
import * as React from 'react';
import { Button } from 'semantic-ui-react';

// you need to create a model of agent
class Agent {
	x: number;
  y: number;
  
	constructor (x: number, y: number){
    this.x = x;
    this.y = y;
  }
	moveToLocation(x: number, y: number) {
		return; 
	}
}

// test
let agent = new Agent(10, 30);
agent.moveToLocation(100, 300);

// then we need to also have a map of environment
class EnvironmentObject {
	name: string;
	image: string;
}

class Project {
	objects: EnvironmentObject[];
}

class Environment {
	addObject(id: string, name: string, x: number, y:number) { 
    let blob = new PIXI.Sprite(PIXI.loader.resources['/images/blob.png'].texture);
    blob.name = 'Blob';
    blob.vy = 1;
  }
  addAgent(x: number, y: number) {
    let cat = new PIXI.Sprite(PIXI.loader.resources['/images/explorer.png'].texture);
    cat.name = 'cat';
    cat.vy = 1;
  }; 
  removeObject(id: string){
    let deleteBlob = document.getElementById('Blob');
    deleteBlob.removeChild(deleteBlob);
  };
}


export class ExecutionView extends React.Component {
  canvas: HTMLElement;
  app: PIXI.Application;

  // REACT METHODS
  render() {
    return (
      <>
        <div ref={n => (this.canvas = n)} id="pixiCanvas" />
      </>
    );
  }
  componentDidMount() {
    let type = 'WebGL';
    let cat: PIXI.Sprite;
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
    let e = new Environment();
    e.addObject('id1', 'Blob', 10, 10); // displays the object
    e.removeObject('id1'); // removes the object
    e.addAgent(20, 30); // adds agent */
  }

  // CLASS METHODS

  setup = () => {
    let background = new PIXI.Sprite(PIXI.loader.resources['/images/dungeon.png'].texture);
  // Add the cat to the stage
    this.app.stage.addChild(background);
  }
}
