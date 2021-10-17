using Ei.Core.Ontology;
using Ei.Logs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    public class AgentProperties
    {
        public GameObject agent;
        public int Count { get; set; }
    }

    public class AgentSpawn: MonoBehaviour
    {
        public AgentSpawn()
        {
            this.launchQueue = new Queue<Action>();
        }

        public AgentProperties[] Agents;
        public float LaunchPerSecond;


        private Queue<Action> launchQueue;
        private System.Timers.Timer launchTimer;


        public void Start()
        {
            // var agentsLaunched = 0;
            var totalAgents = 0;

            // set up timer
            launchTimer = new System.Timers.Timer();
            launchTimer.Interval = (1f / this.LaunchPerSecond) * 1000;
            launchTimer.Elapsed += LaunchAgent;
            launchTimer.AutoReset = false;

            this.launchQueue = new Queue<Action>();

            // launch all agents
            var counts = new int[this.Agents.Length];

            for (var i = 0; i < this.Agents.Length; i++)
            {
                totalAgents += this.Agents[i].Count;
                counts[i] = this.Agents[i].Count;
            }

            // we are launching agents one by category
            while (totalAgents > 0)
            {
                for (var i = 0; i < this.Agents.Length; i++)
                {
                    if (counts[i] == 0)
                    {
                        continue;
                    }
                    counts[i] = counts[i] - 1;
                    totalAgents--;

                    var agentDefinition = this.Agents[i];
                    // connect agent to the institutions

                    this.launchQueue.Enqueue(() =>
                    {
                        var newAgent = this.Instantiate(agentDefinition.agent);
                        var viewAgent = newAgent.GetComponent<SimulationAgent>(); 
                        viewAgent.Connect();
                    });
                }
            }

            // start initialising all agents
            this.launchTimer.Start();
        }

        private void LaunchAgent(object sender, ElapsedEventArgs e)
        {
            if (this.launchQueue.Count == 0)
            {
                return;
            }
            var launchAction = this.launchQueue.Dequeue();
            launchAction();

            this.launchTimer.Start();
        }
    }
}
