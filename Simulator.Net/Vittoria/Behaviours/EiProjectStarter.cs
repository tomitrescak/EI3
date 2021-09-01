//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading;
//using Ei.Logs;
//using Ei.Ontology;
//using Ei.Runtime.Planning.Environment;
//using YamlDotNet.Serialization;
//using System.ComponentModel;
//using System.Timers;
//using Ei.Agents.Core.Behaviours;
//using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
//using Ei.Simulator.Core;
//using UnityEngine;
//using Ei.Agents.Sims;
//using Ei.Agents.Planning;
//using Ei.Agents.Core;
//using Vittoria.Core;
//using Vittoria.Statistics;
//using Project = Ei.Simulation.Simulator.Project;

//namespace Vittoria.Behaviours
//{
//    public class EiProjectStarter : EiBehaviour
//    {
//        [YamlIgnore]
//        private Project project { get; set; }

//        private Dictionary<EnvironmentData, GameObject> objectMappings;
//        private Dictionary<PhysiologyBasedAgent, GameObject> agentMappings;

//        // properties

//        [Category("General")]
//        public string Institution { get; set; }

//        [Category("General")]
//        public string Organisation { get; set; }

//        [Category("General")]
//        public string Password { get; set; }


//        [Category("Runtime")]
//        public double DayLengthInSeconds { get; set; }

//        [Category("Runtime")]
//        public int AgentsPerSecond { get; set; }

//        [Category("Runtime")]
//        public int AgentsLaunched { get; private set; }


//        [Category("Agents")]
//        public List<AgentCount> AgentCounts { get; set; }

//        [Category("Agents")]
//        public double AgentSpeed { get; set; }

//        [Category("Agents")]
//        public List<double> SpeedDiversity { get; set; }

//        [Category("Agents")]
//        public double MetersPerPixel { get; set; }

//        [Category("Physiology")]
//        public List<double> PhysiologyDiversity { get; set; }

//        [Category("Physiology")]
//        public double HungerTreshold { get; set; }

//        [Category("Physiology")]
//        public double FatigueTreshold { get; set; }

//        [Category("Physiology")]
//        public double ThirstThreshold { get; set; }

//        [Category("Physiology")]
//        public double KillThirstThreshold { get; set; }

//        [Category("Physiology")]
//        public double KillHungerThreshold { get; set; }

//        [Category("Physiology")]
//        public double RestSpeed { get; set; }


//        [Category("Environment")]
//        public int Width { get; set; }

//        [Category("Environment")]
//        public int Height { get; set; }

//        [Category("Environment")]
//        public List<EnvironmentDataAction> ActionsWithNoLocation { get; set; }

//        [Category("Environment")]
//        public List<EnvironmentDataDefinition> Elements { get; set; }

//        // constructor

//        public EiProjectStarter() {
//            this.AgentCounts = new List<AgentCount>();
//            this.ActionsWithNoLocation = new List<EnvironmentDataAction>();
//            this.Elements = new List<EnvironmentDataDefinition>();
//        }

//        // methods



//        public void Init() {
//            this.project = new Project();

//            this.objectMappings = new Dictionary<EnvironmentData, GameObject>();
//            this.agentMappings = new Dictionary<PhysiologyBasedAgent, GameObject>();

//            // register listeners
//            this.project.AgentLaunched += ProjectOnAgentLaunched;
//        }

//        public void Start() {
//            // assign current project
//            Project.Current = this.project;

//            this.project.AgentCounts = this.AgentCounts.Select(c => new Ei.Simulator.Core.AgentCount {
//                Groups = c.Groups.Select(g => g.Split(',')).ToArray(),
//                Color = c.Color.ToArray(),
//                Count = c.Count,
//                FreeTimeGoals = c.FreeTimeGoals.ToArray()
//            }).ToArray();
//            this.project.AgentSpeed = this.AgentSpeed;
//            this.project.DayLengthInSeconds = this.DayLengthInSeconds;
//            this.project.Institution = this.Institution;
//            this.project.Organisation = this.Organisation;
//            this.project.MetersPerPixel = this.MetersPerPixel;
//            this.project.SpeedDiversity = this.SpeedDiversity == null ? null : this.SpeedDiversity.ToArray();
//            this.project.PhysiologyDiversity = this.PhysiologyDiversity == null ? null : this.PhysiologyDiversity.ToArray();
//            this.project.HungerTreshold = this.HungerTreshold;
//            this.project.FatigueTreshold = this.FatigueTreshold;
//            this.project.ThirstThreshold = this.ThirstThreshold;
//            this.project.KillThirstThreshold = this.KillThirstThreshold;
//            this.project.KillHungerThreshold = this.KillHungerThreshold;
//            this.project.RestSpeed = this.RestSpeed;
//            this.project.EnvironmentDefinition = new Ei.Runtime.Planning.Environment.AgentEnvironmentDefinition {
//                Width = this.Width,
//                Height = this.Height,
//                ActionsWithNoLocation = this.ActionsWithNoLocation.Select(d => new Ei.Runtime.Planning.Environment.EnvironmentDataAction {
//                    DestroyAfter = d.DestroyAfter,
//                    Duration = d.Duration,
//                    Id = d.Id
//                }).ToArray(),
//                Elements = this.Elements.Select(d => new Ei.Runtime.Planning.Environment.EnvironmentDataDefinition {
//                    Actions = d.Actions.Select(a => new Ei.Runtime.Planning.Environment.EnvironmentDataAction {
//                        DestroyAfter = a.DestroyAfter,
//                        Duration = a.Duration,
//                        Id = a.Id
//                    }).ToArray(),
//                    Id = d.Id,
//                    Image = d.Image,
//                    Max = d.Max,
//                    Probability = d.Probability,
//                    Range = d.Range.ToArray(),
//                    Seed = d.Seed
//                }).ToArray()
//            };

//            // load environment
//            this.project.Environment.ObjectAdded += AddObjectToCanvas;
//            this.project.Environment.ObjectRemoved += RemoveObjectFromCanvas;
//            this.project.OpenInstitution();


//            if (this.project != null && this.project.Manager != null) {
//                this.project.Manager.Stop();
//            }

//            // open environment
//            this.project.Environment.OpenEnvironment();

//            // init statistics
//            var simulation = this.gameObject.GetSimulator() as Simulation;
//            simulation.Statistics.ProcessStatistics(new FpsStatistics());


//            // start this project
//            var thread = new Thread(() => this.project.LazyStart(this.AgentsPerSecond));
//            //thread.IsBackground = true;
//            //thread.Priority = ThreadPriority.Lowest;
//            thread.Start();

//        }

//        private void RemoveObjectFromCanvas(AgentEnvironment environment, EnvironmentData obj) {
//            GameObject.Destroy(this.objectMappings[obj]);
//        }

//        private void AddObjectToCanvas(AgentEnvironment environment, EnvironmentData obj) {
//            // instantiate new simobject
//            var agent = new GameObject(obj.Id);
//            agent.transform.position = new Vector3(obj.X, obj.Y, 0);

//            // add simobject
//            var sim = agent.AddComponent<SimObject>();
//            sim.Icon = obj.Definition.Image;

//            // remember this object
//            this.objectMappings.Add(obj, agent);

//            // SIMULATOR ONLY
//            this.CreateInstance(agent, true, 2);
//        }

//        private void ProjectOnAgentLaunched(PhysiologyBasedAgent agent) {
//            // instantiate new simobject
//            var newObj = new GameObject(agent.Name);
//            newObj.transform.position = new Vector3(agent.X, agent.Y, 0);

//            var sim = newObj.AddComponent<LinearNavigation>();
//            this.agentMappings.Add(agent, newObj);

//            var eiAgent = newObj.AddComponent<EiAgent>();

//            eiAgent.agent = agent;
//            agent.View = eiAgent;

//            var color = newObj.AddComponent<ColorProvider>();
//            color.SetOriginal(agent.Color[0], agent.Color[1], agent.Color[2]);

//            // be yellow
//            color.RestoreOriginal();

//            // put this in the top layer
//            newObj.layer = 1;

//            // SIMULATOR ONLY
//            this.CreateInstance(newObj, true, 1);

//            this.AgentsLaunched++;
//            this.OnPropertyChanged("AgentsLaunched");
//        }

//        public class AgentCount
//        {
//            public List<string> Groups { get; set; }
//            public int Count { get; set; }
//            public List<string> FreeTimeGoals { get; set; }
//            public List<byte> Color { get; set; }
//        }

//        public class EnvironmentDataAction
//        {
//            public string Id { get; set; }
//            public int DestroyAfter { get; set; }
//            public float Duration { get; set; }

//            public override string ToString() {
//                return this.Id != null ? string.Format("{0} [{1}]", this.Id, this.Duration) : "Action";
//            }
//        }

//        public class EnvironmentDataDefinition
//        {
//            public EnvironmentDataDefinition() {
//                this.Actions = new List<EnvironmentDataAction>();
//                this.Range = new List<int> { 0, 0 };
//            }

//            public string Id { get; set; }
//            public string Image { get; set; }
//            public int Seed { get; set; }
//            public List<int> Range { get; set; }
//            public float Probability { get; set; }
//            public int Max { get; set; }
//            public List<EnvironmentDataAction> Actions { get; set; }

//            public override string ToString() {
//                return this.Id != null ? string.Format("{0} [{1}, {2}]", this.Id, this.Seed, this.Max) : "Action";
//            }
//        }
//    }
//}
