using Ei.Core.Runtime.Planning;
using Ei.Logs;
using Ei.Simulation.Behaviours.Actuators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Simulation.Behaviours.Reasoners
{
    public class PlanningReasoner : Reasoner
    {
        private PlanManager planManager;
        private Actuator actuator;
        private SimulationAgent agent;
        private GoalState[][] freeTimeGoals;

        public string[] GoalDefinition;
        public bool WaitForActuatorToFinish = true;

        [JsonIgnore]
        private GoalState[][] FreeTimeGoals
        {
            get
            {
                if (this.freeTimeGoals == null && this.GoalDefinition != null)
                {
                    this.freeTimeGoals = new GoalState[GoalDefinition.Length][];
                    for (var i = 0; i < GoalDefinition.Length; i++)
                    {
                        this.freeTimeGoals[i] = GoalState.ParseStringGoals(this.agent.Governor, GoalDefinition[i]);
                    }
                }
                return this.freeTimeGoals;
            }
        }

        // methods

        public void Start()
        {
            this.actuator = GetComponent<Actuator>();
            this.agent = GetComponent<SimulationAgent>();

            // we use plan manager to create plan actions
            this.planManager = new PlanManager(this.actuator);
        }

        protected virtual GoalState[] GenerateGoals()
        {
            return null;
        }

        public override async void Reason()
        {
            // skip if we are working on some actions
            if (this.WaitForActuatorToFinish && this.actuator.IsProcessing)
            {
                return;
            }

            if (this.planManager.IsPlanning)
            {
                return;
            }

            var currentGoal = this.FreeTimeGoals[UnityEngine.Random.Range(0, this.FreeTimeGoals.Length)];
            var plan = await this.planManager.CreateActionPlan(this.agent, currentGoal, "");

            if (plan == null)
            {
                Log.Warning(this.agent.Governor.Name + " Planning", "could not find the plan ...");
            }
            else
            {
                this.actuator.Enqueue(plan);
            }
        }
    }
}
