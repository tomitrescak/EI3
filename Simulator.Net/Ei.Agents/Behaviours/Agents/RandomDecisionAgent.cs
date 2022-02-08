
using System.Collections.Generic;
using System;
using Ei.Logs;
using Ei.Core.Runtime.Planning;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Planning;

namespace Ei.Simulation.Behaviours.Agents
{
    public class RandomDecisionAgent : SimulationAgent
    {

        // methods

        protected override bool Connected()
        {
            // automatically 
            this.Governor.Continue();
            return true;
        }

        protected override void Reason()
        {
            // wait in case we pause
            if (this.timer.Paused)
            {
                this.RunAfter(this.Reason, 1);
                return;
            }

            Log.Debug(this.Governor.Name, "Reasoning ...");


            // we either continue executing a current plan
            // or we generate a new goal

            if (this.State == AgentState.ExecutingPlan)
            {
                this.ExecuteNextPlanStep(this.Governor);
                return;
            }

            // find a random arc to join

            var connections = this.Governor.Position.ViableConnections(this.Governor);
            if (connections.Length == 0)
            {
                throw new Exception("Agent always has to have at least one option to choose from!");
            }

            this.Plan = new List<AStarNode> { new AStarNode(connections[UnityEngine.Random.Range(0, connections.Length)]) };

            this.PlanHistory.Insert(0,
                new PlanHistory
                {
                    StartString = this.timer.SimulatedTimeString,
                    Goals = "",
                    Result = "Planning",
                    GoalType = "Free"
                });

            this.ContinuePlan(this.Governor, false);


        }

    }


}
