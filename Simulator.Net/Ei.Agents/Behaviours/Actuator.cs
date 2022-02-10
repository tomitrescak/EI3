using Ei.Core.Runtime.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    #region struct ActionItem
    public class ActionItem
    {
        public Func<Task<bool>> action;

        public ActionItem(Func<Task<bool>> action)
        {
            this.action = action;
        }
    }
    #endregion

    public class Actuator : MonoBehaviour
    {
        private Queue<ActionItem> actionQueue;
        private bool processing;

        public bool ShouldProcess => this.processing == false && this.actionQueue.Count > 0;

        public bool IsProcessing => processing;

        public Actuator()
        {
            this.actionQueue = new Queue<ActionItem>();
            this.processing = false;
        }

        public void Enqueue(AStarNode planNode, SimulationAgent agent, PlanManager manager)
        {
            var plan = new List<AStarNode> { planNode };

            this.Enqueue(plan, agent, manager);
        }

        public void Enqueue(List<AStarNode> plan, SimulationAgent agent, PlanManager manager)
        {
            this.Enqueue(new ActionItem(() => manager.ExecutePlan(agent, plan)));
        }

        public void Enqueue(ActionItem item)
        {
            this.actionQueue.Enqueue(item);
            this.ProcessQueue();
        }

        public async Task ProcessQueue()
        {
            if (this.processing)
            {
                return;
            }

            // execute all actions in queue that can be executed at this time
            while (this.actionQueue.Count > 0)
            {
                var item = actionQueue.Dequeue();
                await item.action();
            }

            this.processing = false;
        }
    }
}
