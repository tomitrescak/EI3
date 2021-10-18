using System.Collections.Generic;
using UnityEngine;

namespace Ei.Simulation.Simulator
{
    public class Scene
    {
        public Scene() { 
            this.GameObjects = new List<GameObject> (); 
            this.Prefabs = new List<GameObject> (); 
        }
        public Scene(List<GameObject> gameObjects) {
            GameObjects = gameObjects;
        }
        public List<GameObject> GameObjects { get; set; }
        public List<GameObject> Prefabs { get; set; }
    }
}