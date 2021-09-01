using Ei.Agents.Core;
using Ei.Agents.Core.Behaviours;
using Ei.Agents.Sims;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Vittoria.Core;
using Vittoria.Statistics;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Vittoria.Behaviours
{
    public class Spawn : EiBehaviour, IUpdates
    {
        public enum Renderer
        {
            SimRenderer,
            FastRenderer
        }

        public enum AgentType
        {
            RandomWalker,
            Sim
        }

        private Simulation simulation;
        private float minX;
        private float minY;
        private float maxX;
        private float maxY;
        private int spawned;

        
        public int MinAgentCount { get; set; }
        public int MaxAgentCount { get; set; }
        public int SpawnPeriodMs { get; set; }
        public int MinSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public float MinDecayRatePerSecond { get; set; }
        public float MaxDecayRatePerSecond { get; set; }
        public float MinInitialDecay { get; set; }
        public float MaxInitialDecay { get; set; }
        public Renderer AgentRenderer { get; set; }
        public AgentType Type { get; set; }

        public float DayLengthInSeconds {
            get { return BehaviourConfiguration.DayLengthInSeconds; }
            set { BehaviourConfiguration.DayLengthInSeconds = value; }
        }

        [ExpandableObject]
        [DisplayName("Sim Objects")]
        public List<SimObjectDAO> SimObjects { get; set; }

        public Spawn() {
            this.SimObjects = new List<SimObjectDAO>();
            this.MinDecayRatePerSecond = 0.3f;
            this.MaxDecayRatePerSecond = 1f;
            this.MinInitialDecay = -50;
            this.MaxInitialDecay = 50;
            this.AgentRenderer = Renderer.SimRenderer;
            this.Type = AgentType.RandomWalker;
        }

        public void Init() {
            this.simulation = this.gameObject.GetSimulator() as Simulation;
        }

        public void Start() {
            this.minX = 0;
            this.maxX = (float)this.simulation.writeableBmp.Width - 50;
            this.minY = 0;
            this.maxY = (float)this.simulation.writeableBmp.Height - 50;

            // init sim objects
            this.SpawnObjects();

            // init agents
            this.SpawnAgents();
        }

        public void Update() {
            // we will randomly put objects on the canvas
            foreach (var obj in this.SimObjects) {
                if (obj.SeedPeriodInMinutes > 0 && obj.NextSeed < Time.time) {
                    var count = UnityEngine.Random.Range(obj.MinSeedCount, obj.MaxSeedCount);
                    for (var i=0; i<count; i++) {
                        this.SpawnObject(obj, i);
                    }
                    obj.NextSeed += obj.SeedPeriodInMinutes * BehaviourConfiguration.SimulatedMinutesToReal;
                }

            }
        }

        void SpawnAgents() {
            // add required statistics
            if (this.Type == AgentType.Sim) {
                this.simulation.Statistics.ProcessStatistics(new SimsStatistics());
                //this.simulation.Statistics.ProcessStatistics(new FpsStatistics());
            } else {
                this.simulation.Statistics.ProcessStatistics(new FpsStatistics());
            }

            for (int i = 0; i < this.MinAgentCount; i++) {
                this.SpawnAgent();
            }

            if (this.MaxAgentCount > this.MinAgentCount) {
                var timer = new System.Timers.Timer();
                timer.Interval = this.SpawnPeriodMs;
                timer.AutoReset = true;
                timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                    this.SpawnAgent();
                    if (this.spawned == this.MaxAgentCount) {
                        timer.Stop();
                    }
                };
                timer.Start();
            }
        }

        private void TimeSpawnAgent(object sender, System.Timers.ElapsedEventArgs e) {
            throw new NotImplementedException();
        }

        void SpawnAgent() {
            var agent = new GameObject();

            // set transform

            agent.Transform.position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(minY, maxY),
                     0);
            agent.name = this.spawned.ToString();

            // add navigation components
            var nav = agent.AddComponent<LinearNavigation>();
            nav.SpeedPxPerSecond = UnityEngine.Random.Range(MinSpeed, MaxSpeed);

            // create agent specific functionality
            switch (this.Type) {
                case AgentType.RandomWalker:
                    this.SpawnRandomWalker(agent);
                    break;
                case AgentType.Sim:
                    this.SpawnSim(agent);
                    break;
            }

            // add renderer
            // we only allow sim renderer for sims
            if (this.Type == AgentType.Sim) {
                switch (this.AgentRenderer) {
                    case Renderer.FastRenderer:
                        agent.AddComponent<FastRenderer>();
                        break;
                    case Renderer.SimRenderer:
                        agent.AddComponent<SimRenderer>();
                        break;
                }
            } else {
                agent.AddComponent<FastRenderer>();
            }

            this.simulation.CreateInstance(agent, true);
            this.spawned++;
        }

        void SpawnObjects() {
            foreach (var obj in this.SimObjects) {
                for (var k = 0; k < obj.Count; k++) {
                    this.SpawnObject(obj, k);
                }
            }
        }

        void SpawnObject(SimObjectDAO obj, int k) {
            var agent = new GameObject();

            // add position
            agent.Transform.position = new Vector3(
                UnityEngine.Random.Range(minX, maxX),
                UnityEngine.Random.Range(minY, maxY),
                 0);
            agent.name = obj.Name + "_" + k;

            // add advertisement
            var adv = agent.AddComponent<Ei.Agents.Sims.SimObject>();
            adv.Actions = obj.Actions.Select(s => new SimAction(
                s.Name,
                s.Uses,
                s.Modifiers.Select(m => new ModifierAdvertisement(m.Delta, m.Type, m.PersonalityModifiers)).ToArray(),
                s.DurationInMinutes,
                s.Plan.ToArray())).ToArray();
            adv.Icon = obj.Icon;
            adv.Name = obj.Name;

            // add renderer
            agent.AddComponent<IconRenderer>();

            this.simulation.CreateInstance(agent, true);
        }

        // different agents

        void SpawnRandomWalker(GameObject agent) {
            var rnav = agent.AddComponent<RandomNavigation>();
            rnav.Width = (float) this.simulation.writeableBmp.Width;
            rnav.Height = (float) this.simulation.writeableBmp.Height;
        }

        void SpawnSim(GameObject agent) {
            var reasoner = agent.AddComponent<SimReasoner>();

            // add sims stuff
            var sim = agent.AddComponent<Sim>();
            foreach (var modifier in sim.modifiers) {
                modifier.DecayRatePerSecond = UnityEngine.Random.Range(MinDecayRatePerSecond, MaxDecayRatePerSecond);
                modifier.XValue = UnityEngine.Random.Range(MinInitialDecay, MaxInitialDecay);
            }
        }

    }
}
