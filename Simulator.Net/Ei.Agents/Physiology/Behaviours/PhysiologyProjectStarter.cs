using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YamlDotNet.Serialization;
using System.ComponentModel;
using UnityEngine;
using Ei.Core.Runtime.Planning.Environment;
using Ei.Simulation.Agents.Behaviours;
using Ei.Simulation.Core;
using Ei.Simulation.Physiology;
using Ei.Simulation.Sims.Behaviours;
using Ei.Simulation.Simulator;
using Ei.Simulation.Statistics;

namespace Vittoria.Behaviours
{
    public class PhysiologyProjectStarter : EiBehaviour
    {
        [YamlIgnore]
        private PhysiologyProject project { get; set; }

        private Dictionary<EnvironmentData, GameObject> objectMappings;
        private Dictionary<PhysiologyBasedAgent, GameObject> agentMappings;

        // properties

        [Category("General")]
        public string Institution { get; set; }

        [Category("General")]
        public string Organisation { get; set; }

        [Category("General")]
        public string Password { get; set; }


        [Category("Runtime")]
        public double DayLengthInSeconds { get; set; }

        [Category("Runtime")]
        public int AgentsPerSecond { get; set; }

        [Category("Runtime")]
        public int AgentsLaunched { get; private set; }


        [Category("Agents")]
        public List<AgentCount> AgentCounts { get; set; }

        [Category("Agents")]
        public double AgentSpeed { get; set; }

        [Category("Agents")]
        public List<double> SpeedDiversity { get; set; }

        [Category("Agents")]
        public double MetersPerPixel { get; set; }

        [Category("Physiology")]
        public List<double> PhysiologyDiversity { get; set; }

        [Category("Physiology")]
        public double HungerTreshold { get; set; }

        [Category("Physiology")]
        public double FatigueTreshold { get; set; }

        [Category("Physiology")]
        public double ThirstThreshold { get; set; }

        [Category("Physiology")]
        public double KillThirstThreshold { get; set; }

        [Category("Physiology")]
        public double KillHungerThreshold { get; set; }

        [Category("Physiology")]
        public double RestSpeed { get; set; }

        [Category("Environment")]
        public int Width { get; set; }

        [Category("Environment")]
        public int Height { get; set; }

        [Category("Environment")]
        public List<EnvironmentDataAction> ActionsWithNoLocation { get; set; }

        [Category("Environment")]
        public List<EnvironmentDataDefinition> Elements { get; set; }

        // constructor

        public PhysiologyProjectStarter()
        {
            this.AgentCounts = new List<AgentCount>();
            this.ActionsWithNoLocation = new List<EnvironmentDataAction>();
            this.Elements = new List<EnvironmentDataDefinition>();
        }

        // methods


        public void Init()
        {
            this.project = new PhysiologyProject();

            this.objectMappings = new Dictionary<EnvironmentData, GameObject>();
            this.agentMappings = new Dictionary<PhysiologyBasedAgent, GameObject>();

            // register listeners
            this.project.AgentLaunched += Project_AgentLaunched;
        }

        private void Project_AgentLaunched(SimulationAgent obj)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            this.project.AgentCounts = this.AgentCounts.Select(c => new Ei.Simulation.Simulator.AgentCount
            {
                Groups = c.Groups.Select(g => g.Split(',')).ToArray(),
                Color = c.Color.ToArray(),
                Count = c.Count,
                FreeTimeGoals = c.FreeTimeGoals.ToArray()
            }).ToArray();
            
            this.project.AgentSpeed = this.AgentSpeed;
            this.project.DayLengthInSeconds = this.DayLengthInSeconds;
            this.project.Organisation = this.Organisation;
            this.project.MetersPerPixel = this.MetersPerPixel;
            this.project.SpeedDiversity = SpeedDiversity?.ToArray();
            this.project.PhysiologyDiversity = PhysiologyDiversity?.ToArray();
            this.project.HungerTreshold = this.HungerTreshold;
            this.project.FatigueTreshold = this.FatigueTreshold;
            this.project.ThirstThreshold = this.ThirstThreshold;
            this.project.KillThirstThreshold = this.KillThirstThreshold;
            this.project.KillHungerThreshold = this.KillHungerThreshold;
            this.project.RestSpeed = this.RestSpeed;
            this.project.EnvironmentDefinition = new AgentEnvironmentDefinition
            {
                Width = this.Width,
                Height = this.Height,
                ActionsWithNoLocation = this.ActionsWithNoLocation.Select(d =>
                    new EnvironmentDataAction
                    {
                        DestroyAfter = d.DestroyAfter,
                        Duration = d.Duration,
                        Id = d.Id
                    }).ToArray(),
                Elements = this.Elements.Select(d => new EnvironmentDataDefinition
                {
                    Actions = d.Actions.Select(a => new EnvironmentDataAction
                    {
                        DestroyAfter = a.DestroyAfter,
                        Duration = a.Duration,
                        Id = a.Id
                    }).ToArray(),
                    Id = d.Id,
                    Image = d.Image,
                    Max = d.Max,
                    Probability = d.Probability,
                    Range = d.Range.ToArray(),
                    Seed = d.Seed
                }).ToArray()
            };

            // load environment
            this.project.Environment.ObjectAdded += AddObjectToCanvas;
            this.project.Environment.ObjectRemoved += RemoveObjectFromCanvas;


            project?.Manager?.Stop();

            // open environment
            this.project.Environment.OpenEnvironment();

            // init statistics
            var simulation = this.gameObject.GetSimulator() as Runner;
            simulation?.ProcessStatistics(new FpsStatistics());


            // start this project
            var thread = new Thread(() => this.project.Start(null, this.AgentsPerSecond));
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }

        private void RemoveObjectFromCanvas(AgentEnvironment environment, EnvironmentData obj)
        {
            GameObject.Destroy(this.objectMappings[obj]);
        }

        private void AddObjectToCanvas(AgentEnvironment environment, EnvironmentData obj)
        {
            // instantiate new simobject
            var agent = new GameObject(obj.Id);
            agent.transform.position = new Vector3(obj.X, obj.Y, 0);

            // add simobject
            var sim = agent.AddComponent<SimObject>();
            sim.Icon = obj.Definition.Image;

            // remember this object
            this.objectMappings.Add(obj, agent);

            // SIMULATOR ONLY
            this.CreateInstance(agent, true, 2);
        }

        private void ProjectOnAgentLaunched(SimulationAgent original)
        {
            PhysiologyBasedAgent agent = (PhysiologyBasedAgent) original;

            // instantiate new simobject
            var newObj = new GameObject(agent.Name);
            newObj.transform.position = new Vector3(agent.X, agent.Y, 0);

            var sim = newObj.AddComponent<LinearNavigation>();
            this.agentMappings.Add(agent, newObj);

            var eiAgent = newObj.AddComponent<EiAgent>();

            eiAgent.agent = agent;
            agent.View = eiAgent;

            var color = newObj.AddComponent<ColorProvider>();
            color.SetOriginal(agent.Color[0], agent.Color[1], agent.Color[2]);

            // be yellow
            color.RestoreOriginal();

            // put this in the top layer
            newObj.layer = 1;

            // SIMULATOR ONLY
            this.CreateInstance(newObj, true, 1);

            this.AgentsLaunched++;
            this.OnPropertyChanged("AgentsLaunched");
        }

        public class AgentCount
        {
            public List<string> Groups { get; set; }
            public int Count { get; set; }
            public List<string> FreeTimeGoals { get; set; }
            public List<byte> Color { get; set; }
        }

    }
}