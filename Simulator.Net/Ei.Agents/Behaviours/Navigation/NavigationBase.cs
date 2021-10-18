using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    public abstract class NavigationBase: MonoBehaviour
    {
        public float Speed;
           
        protected float destinationX;
        protected float destinationY;

        [JsonIgnore]
        public bool Navigating { get; protected set; }

        public void MoveToDestination(Vector3 position)
        {
            this.MoveToDestination(position.x, position.y);
        }

        public abstract void MoveToDestination(float x, float y);

        protected void MovedToDestination()
        {
            GetComponent<SimulationAgent>().MovedToLocation();
        }
    }
}
