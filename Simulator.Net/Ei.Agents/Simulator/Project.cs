using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Ei.Logs;
using Ei.Ontology;
using Ei.Runtime.Planning.Environment;
using Ei.Simulator.Experiments;
using YamlDotNet.Serialization;
using System.ComponentModel;
using System.Timers;

namespace Ei.Simulator.Core
{
   

    public class Project: INotifyPropertyChanged
    {
        // fields

        private AgentEnvironment environment;
        private string directory;
        private bool paused;

        // properties
        
        public int Width { get; set; }
        public int Height { get; set; }
        public AgentCount[] AgentCounts { get; set; }

        public double DayLengthInSeconds { get; set; }
        public double MetersPerPixel { get; set; }

        public double AgentSpeed { get; set; }
        public double[] SpeedDiversity { get; set; }
        public double[] PhysiologyDiversity { get; set; }

        public double HungerTreshold { get; set; }
        public double FatigueTreshold { get; set; }
        public double ThirstThreshold { get; set; }

        public double KillThirstThreshold { get; set; }
        public double KillHungerThreshold { get; set; }

        public double RestSpeed { get; set; }

        public string Organisation { get; set; }
        public string Password { get; set; }

        public AgentEnvironmentDefinition EnvironmentDefinition { get; set; }

        public InstitutionManager Manager { get; private set; }
        public Institution Ei { get; private set; }

        public static Project Current { get; set; }
        public List<SimulationAgent> Agents { get; set; }

        public string Institution { get; set; }

        public List<IExperiment> Experiments { get; private set; }

        public string RealTime
        {
            get
            {
                var realSeconds = Ei.Time.ElapsedMilliseconds / 1000f;

                TimeSpan t1 = TimeSpan.FromSeconds(realSeconds);

                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                    t1.Hours,
                    t1.Minutes,
                    t1.Seconds);
              
            }
        }

        public double SimulatedTime
        {
            get
            {
                var realSeconds = Ei.Time.ElapsedMilliseconds / 1000f;
                return realSeconds * (86400 / DayLengthInSeconds);
            }
        }

        public string SimulatedTimeString
        {
            get
            {
                return FormatTime(this.SimulatedTime);
            }
        }

        public AgentEnvironment Environment
        {
            get
            {
                if (this.environment == null)
                {
                    this.environment = new AgentEnvironment(this.EnvironmentDefinition);
                }
                return this.environment;
            }
        }

        public double SimulatedSecond { get { return 86400 / this.DayLengthInSeconds; } }

        public bool Paused
        {
            get { return this.paused; }
            set
            {
                this.paused = value;
                if (value)
                {
                    this.Waiter.Reset();
                }
                else
                {
                    this.Waiter.Set();
                }
            }
        }

        public ManualResetEvent Waiter { get; set; }
        public bool Started { get; set; }


        // events

        public event Action<PhysiologyBasedAgent> AgentLaunched;
        public event PropertyChangedEventHandler PropertyChanged;

        // constructor

        public Project()
        {
            this.Experiments = new List<IExperiment>();
        } 

        // methods

        public static Project Open(string path)
        {
            var input = new StringReader(File.ReadAllText(path));
            var deserializer = new Deserializer();

            Current = deserializer.Deserialize<Project>(input);

            // rememeber directory

            Current.SetDirectory(Path.GetDirectoryName(path));

            Current.OpenInstitution();

            return Current;
        }

        public void OpenInstitution() {
            // load institution
            switch (this.Institution) {
                case "Uruk":
                    this.Ei = new DefaultInstitution();
                    this.Ei.Resources.Tick = (float) (86400f / this.DayLengthInSeconds);
                    break;
                default:
                    throw new Exception("Institution Not Found: " + this.Institution);
            }
            
            this.Waiter = new ManualResetEvent(true);
        }

        public void SetDirectory(string dir)
        {
            this.directory = dir;
        }

        private Queue<Action> launchQueue = new Queue<Action>();
        private System.Timers.Timer launchTimer;

        public void LazyStart(int launchAgentsPerSecond) {
            // init
            this.Agents = new List<SimulationAgent>();

            // start institution
            Manager = (InstitutionManager)InstitutionManager.Launch(this.Ei);
            Ei = Manager.Ei;
            Ei.Start();


            int agentsLaunched = 0;
            int totalAgents = 0;

            // set up timer
            launchTimer = new System.Timers.Timer();
            launchTimer.Interval = (1f / launchAgentsPerSecond) * 1000;
            launchTimer.Elapsed += LaunchAgent;
            launchTimer.AutoReset = false;

            this.launchQueue = new Queue<Action>();

            // launch all agents
            foreach (var agentCount in this.AgentCounts) {
                totalAgents += agentCount.Count;

                for (var i = 0; i < agentCount.Count; i++) {
                    // connect agent to the institutions
                    
                    this.launchQueue.Enqueue(() => {
                        var agent = new PhysiologyBasedAgent(agentCount.Groups[0][1], agentCount.FreeTimeGoals);

                        // set color
                        if (agentCount.Color == null) agent.Color = new byte[] { 255, 255, 0 };
                        else agent.Color = agentCount.Color;

                        // add a new agent
                        this.Agents.Add(agent);

                        // notify ui that new agent was created
                        if (this.AgentLaunched != null) {
                            this.AgentLaunched(agent);
                        }

                        Log.Info("Project", "Launching: " + agent.Name);
                        agent.Connect(this.Manager, this.Organisation, this.Password, agentCount.Groups);
                    });

                    
                }
            }

            // start initialising all agents
            this.launchTimer.Start();

            // start environment evolution
            this.Environment.Start();
        }

        private void LaunchAgent(object sender, ElapsedEventArgs e) {
            if (this.launchQueue.Count == 0) {
                Project.Current.Started = true;
                return;
            }
            var launchAction = this.launchQueue.Dequeue();
            launchAction();

            this.launchTimer.Start();
        }

        public void Start(bool async = true)
        {
            // init
            this.Agents = new List<SimulationAgent>();

            // start institution
            Manager = InstitutionManager.Launch(this.Ei);
            Ei = Manager.Ei;


            int agentsLaunched = 0;
            int totalAgents = 0;

            // launch all agents
            foreach (var agentCount in this.AgentCounts)
            {
                totalAgents += agentCount.Count;

                for (var i = 0; i < agentCount.Count; i++)
                {
                    var agent = new PhysiologyBasedAgent(agentCount.Groups[0][0], agentCount.FreeTimeGoals);

                    // set color
                    if (agentCount.Color == null) agent.Color = new byte[] {255, 255, 0};
                    else agent.Color = agentCount.Color;

                    // add a new agent
                    this.Agents.Add(agent);

                    // notify ui that new agent was created
                    if (this.AgentLaunched != null)
                    {
                        this.AgentLaunched(agent);
                    }

                    // connect agent to the institutions
                    if (async) {
                        new Thread(() => {
                            Log.Info("Project", "Launching: " + agent.Name);
                            agent.Connect(this.Manager, this.Organisation, this.Password, agentCount.Groups);

                            if (++agentsLaunched == totalAgents) {
                                Project.Current.Started = true;
                            }
                        }).Start();
                    } else {
                        Log.Info("Project", "Launching: " + agent.Name);
                        agent.Connect(this.Manager, this.Organisation, this.Password, agentCount.Groups);

                        if (++agentsLaunched == totalAgents) {
                            Project.Current.Started = true;
                        }
                    }
                }
            }
            

            // start environment evolution

            this.Environment.Start();
        }

        public string GetPath(string path)
        {
            return Path.Combine(this.directory, path);
        }


        public double CalculateDuration(float waitTimeInSeconds)
        {
            return waitTimeInSeconds / SimulatedSecond;
        }

        public static string FormatTime(double simulatedSeconds)
        {
            TimeSpan t2 = TimeSpan.FromSeconds(simulatedSeconds);

            return string.Format("{0}d {1:D2}:{2:D2}",
                t2.Days,
                t2.Hours,
                t2.Minutes,
                t2.Seconds);
        }

        
    }

    public class AgentCount
    {
        public string[][] Groups { get; set; }
        public int Count { get; set; }
        public string[] FreeTimeGoals { get; set; }
        public byte[] Color { get; set; }
    }
}
