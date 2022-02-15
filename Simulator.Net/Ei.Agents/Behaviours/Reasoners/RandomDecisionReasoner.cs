
using System.Collections.Generic;
using System;
using Ei.Logs;
using Ei.Core.Runtime.Planning;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Planning;
using UnityEngine;
using Ei.Simulation.Behaviours.Actuators;

namespace Ei.Simulation.Behaviours.Reasoners
{
    public class RandomDecisionReasoner : Reasoner
    {
        PlanManager planManager;
        Actuator actuator;
        SimulationAgent agent;

        // methods

        public void Start()
        {
            this.actuator = GetComponent<Actuator>();
            this.agent = GetComponent<SimulationAgent>();

            // we use plan manager to create plan actions
            this.planManager = new PlanManager(this.actuator);
        }

        public override void Reason()
        {

            // skip if we are working on some actions
            if (this.actuator.IsProcessing)
            {
                return;
            }

            // Log.Debug(this.agent.Governor.Name, "Reasoning ...");

            var connections = this.agent.Governor.Position.ViableConnections(this.agent.Governor);
            if (connections.Length == 0)
            {
                throw new Exception("Agent always has to have at least one option to choose from!");
            }

            var planNode = new AStarNode(connections[UnityEngine.Random.Range(0, connections.Length)]);

            this.actuator.Enqueue(planNode, this.agent, this.planManager);


        }

    }


}
