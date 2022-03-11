using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Actions
{
    public class LimitUseAction: Interaction
    {
        public struct ActionUse
        {
            public string ActionId;
            public int UseLimit;
        }

        private object locker = new object();
        private Dictionary<string, int> uses;

        public ActionUse[] actionLimits;

        public void Start()
        {
            this.uses = new Dictionary<string, int>();
            // this.actionLimits = new List<ActionUse>();

            foreach (var action in this.actionLimits)
            {
                this.uses[action.ActionId] = action.UseLimit;
            }

           
        }

        public override async Task<bool> UseObject(GameObject owner, SimulationAgent agent, EnvironmentAction environmentAction)
        {
            var actionId = environmentAction.Id;
            var action = this.actionLimits.FirstOrDefault(w => w.ActionId == actionId);

            if (action.ActionId == null || action.UseLimit == 0)
            {
                return true;
            }

            lock (locker)
            {
                // check if we can use the object
                if (this.uses[actionId] > 0)
                {
                    this.uses[actionId]--;
                    return true;
                }
            }

            return false;

            // wait for the duration
            //await this.UseObject(action);

            //if (shouldDestroy)
            //{
            //    // remove from environment
            //    FindObjectOfType<AgentEnvironment>().RemoveObject(this);

            //    // destroy game object
            //    GameObject.Destroy(this.gameObject);
            //}

            //return canUse;
        }


    }
}
