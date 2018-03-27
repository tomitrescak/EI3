import * as PIXI from 'pixi.js';
import * as React from 'react';
import { Button } from 'semantic-ui-react';

export class ExecutionView extends React.Component {
  canvas: HTMLElement;
  app: PIXI.Application;

  addLogo = () => {
    let cat = new PIXI.Sprite(PIXI.loader.resources['/images/wsu.jpg'].texture);
    cat.x = Math.random() * 400;
    cat.y = Math.random() * 400;

    // Add the cat to the stage
    this.app.stage.addChild(cat);
  };

  clearCanvas = () => {
    this.app.stage.removeChildren();
  }

  render() {
    return (
      <>
        <div ref={n => (this.canvas = n)} id="pixiCanvas" />
        <Button onClick={this.addLogo} content="Add Logo" />
        <Button onClick={this.clearCanvas} content="Clear" color="red" icon="trash" />
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
      width: 600,
      height: 600
      // antialias: true,
      // transparent: false,
      // resolution: 1
    });

    PIXI.loader.reset();
    PIXI.loader.add('/images/wsu.jpg').load();

    this.app = app;

    function setup() {
      // Create the cat sprite
    }

    this.canvas.appendChild(app.view);
  }
}
