using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    public abstract class NavigationBase: MonoBehaviour
    {
        public float SpeedKmH = 5;
           
        protected float destinationX;
        protected float destinationY;

        public void MoveToDestination(Vector3 position)
        {
            this.MoveToDestination(position.x, position.y);
        }

        public abstract Task<bool> MoveToDestination(float x, float y);
    }
}
