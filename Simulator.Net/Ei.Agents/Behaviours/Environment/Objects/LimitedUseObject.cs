using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Objects
{
    [Serializable]
    public class LimitedUseAction : EnvironmentAction
    {
        public int DestroyAfter;
    }

    public class LimitedUseObject: EnvironmentObject
    {
        private LimitedUseAction[] actions;

        private object locker = new object();
        private Dictionary<string, int> uses;

        public override EnvironmentAction[] Actions
        {
            get { return actions; }
            set { actions = value as LimitedUseAction[]; }
        }

        public new void Start()
        {
            base.Start();

            this.uses = new Dictionary<string, int>();
        }

        public override async Task<bool> Use(string id)
        {
            var action = this.actions.First(w => w.Id == id);

            var canUse = action.DestroyAfter == 0;
            var shouldDestroy = false;

            lock (locker)
            {
                // check if we can use the object
                if (action.DestroyAfter > 0 && action.DestroyAfter - this.uses[id] > 0)
                {
                    canUse = true;
                    
                    if (!this.uses.ContainsKey(id))
                    {
                        this.uses.Add(id, 0);
                    }

                    this.uses[id]++;

                    // return how many uses are remainig

                    if (action.DestroyAfter - this.uses[id] <= 0)
                    {
                        shouldDestroy = true;
                    }
                }
            }

            // wait for the duration
            await this.UseObject(action);

            if (shouldDestroy)
            {
                // remove from environment
                FindObjectOfType<AgentEnvironment>().RemoveObject(this);

                // destroy game object
                GameObject.Destroy(this.gameObject);
            }

            return canUse;
        }

    }
}
