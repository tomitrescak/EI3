using Ei.Core.Runtime;
using Ei.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Objects
{

    public class EnvironmentObject : MonoBehaviour
    {
        private EnvironmentAction[] actions;

        // public string Id;

        [NonSerialized]
        public Governor Owner;


        public string Name;

        public string Icon;

        public virtual EnvironmentAction[] Actions { 
            get { return actions; } 
            set { this.actions = value; } 
        }

        public void Start()
        {
            // register with the environment
            FindObjectOfType<AgentEnvironment>().AddObject(this);
        }

        public virtual async Task<bool> Use(string actionId)
        {
            var action = this.Actions.First(w => w.Id == actionId);
            if (action == null)
            {
                throw new Exception("This objects does not provide action: " + actionId);
            }

            return await this.UseObject(action);
        }

        protected virtual async Task<bool> UseObject(EnvironmentAction action)
        {
            await Task.Delay((int)action.Duration);
            return true;
        }
    }

}
