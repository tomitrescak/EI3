using System.Collections.Generic;
using System;
using System.Linq;
using Ei.Core.Ontology;
using Ei.Logs;
using Ei.Core.Runtime.Planning;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Planning;
using UnityEngine;
using Ei.Simulation.Behaviours.Actuators;

namespace Ei.Simulation.Behaviours.Reasoners
{
    public class ManualDecisionReasoner : Reasoner
    {
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

        private WorkflowPosition lastPosition;
        private bool processingNotified;
        
        public override void Reason()
        {
            if (this.actuator.IsProcessing == false)
            {
                this.processingNotified = false;
            }

            // skip if we are working on some actions
            if (this.actuator.IsProcessing && this.processingNotified == false)
            {
                Log.Info(this.agent.Name, "Reasoner", "Available Connections");
                this.processingNotified = true;
                return;
            }

            if (this.lastPosition == this.agent.Governor.Position)
            {
                return;
            }

            this.lastPosition = this.agent.Governor.Position;

            if (this.agent.Governor.Position == null)
            {
                return;
            }

            var connections = this.agent.Governor.Position.ViableConnections(this.agent.Governor);
            
            var parameters = new List<string>();
            foreach (var c in connections)
            {
                if (c.Action != null)
                {
                    parameters.Add(this.agent.Governor.Workflow.Id);
                    parameters.Add(c.Id);
                    parameters.Add(c.Action.Id);
                }
            }
            // var parameters = connections.Select(c => c.Action?.Name)

            var pars = parameters.ToArray();

            // compare with previous
            
            Log.Info(this.agent.Name, "Reasoner", "Available Connections", pars);
            
            // var planNode = new AStarNode(connections[UnityEngine.Random.Range(0, connections.Length)]);
            // this.actuator.Enqueue(planNode, this.agent, this.planManager);
        }

    }


}