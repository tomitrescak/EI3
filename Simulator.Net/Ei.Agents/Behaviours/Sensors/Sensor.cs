using Ei.Simulation.Behaviours.Environment.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ei.Simulation.Behaviours.Environment;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Sensors
{
    public class Sensor: MonoBehaviour
    {
        private AgentEnvironment environment;
        
        public void Start()
        {
            this.environment = FindObjectOfType<AgentEnvironment>();
            if (this.environment == null)
            {
                throw new Exception("You must define en environment for sensor to work");
            }
        }

        public List<ActionsProvider> FindActionObjects(string action)
        {
            if (!this.environment.Actions.ContainsKey(action)) 
            {
                return null;
            }
            // returns all objects, percieves the whole environment
            return this.environment.Actions[action];
        } 
    }
}
