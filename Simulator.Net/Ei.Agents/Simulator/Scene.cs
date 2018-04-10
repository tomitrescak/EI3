using System.Collections.Generic;
using UnityEngine;

namespace Ei.Simulation.Simulator
{
    public class Scene
    {
        public Scene(List<GameObject> gameObjects) {
            GameObjects = gameObjects;
        }
        public List<GameObject> GameObjects { get; }   
    }
}