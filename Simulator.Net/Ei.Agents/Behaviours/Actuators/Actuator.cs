using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Costs;
using Ei.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Ei.Simulation.Behaviours.PlanManager;

namespace Ei.Simulation.Behaviours.Actuators
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

        [JsonIgnore]
        public bool ShouldProcess => this.processing == false && this.actionQueue.Count > 0;

        [JsonIgnore]
        public bool IsProcessing => processing;

        public Actuator()
        {
            this.actionQueue = new Queue<ActionItem>();
            this.processing = false;
        }

        public virtual ICostManager CreateCostManager(PlannerSession session)
        {
            // return new TravelCostManager(agent, this.environment, (int)this.transform.X, (int)this.transform.Y)
            return null;
        }

        public virtual async Task<bool> ExecutePlanItem(SimulationAgent agent, AStarNode planItem)
        {
            return this.PerformAction(agent, planItem);

        }

        protected bool PerformAction(SimulationAgent agent, AStarNode planItem, params VariableInstance[] parameters)
        {
            if (this.gameObject.GameEngine.IsRunning == false)
            {
                Log.Warning(agent.Name, "Actuator", "Game Engine Not Running: Ignoring Action");
                return false;
            }

            var result = agent.Governor.PerformAction(planItem.Arc.Id, planItem.Arc.Action.Id, parameters);
            if (result.IsOk)
            {
                return true;
            }
            return false;
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

            this.processing = true;

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
