
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
        PlanManager planManager;

        // methods

        protected override bool Connected()
        {
            // we use plan manager to create plan actions
            this.planManager = new PlanManager();
            
            // automatically start participation in the institution
            this.Governor.Continue();


            return true;
        }

        protected override void Reason()
        {
            // wait in case we pause
            //if (this.timer.Paused)
            //{
            //    this.RunAfter(this.Reason, 1);
            //    return;
            //}

            Log.Debug(this.Governor.Name, "Reasoning ...");

            // skip if we are working on some actions
            if (this.Actuator.IsProcessing)
            {
                return;
            }

            // we either continue executing a current plan
            // or we generate a new goal

            var connections = this.Governor.Position.ViableConnections(this.Governor);
            if (connections.Length == 0)
            {
                throw new Exception("Agent always has to have at least one option to choose from!");
            }

            var planNode = new AStarNode(connections[UnityEngine.Random.Range(0, connections.Length)]);

            // add environment action responsible for this
            // planNode.CostData = this.environment.NoLocationInfo(planNode.Arc.Action.Id).Id;

            //this.Plan = new List<AStarNode> { planNode };

            //this.PlanHistory.Insert(0,
            //    new PlanHistory
            //    {
            //        StartString = this.timer.SimulatedTimeString,
            //        Goals = "",
            //        Result = "Planning",
            //        GoalType = "Free"
            //    });

            //this.ContinuePlan(this.Governor, false);

            this.Actuator.Enqueue(planNode, this, this.planManager);


        }

    }


}
