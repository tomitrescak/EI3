using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using Ei.Logs;
using Ei.Core.Ontology;
using Ei.Core.Runtime.Planning.Environment;

namespace Ei.Simulation.Simulator
{
    public abstract class Project : INotifyPropertyChanged
    {
        // fields

        private AgentEnvironment environment;
        private string directory;
        private bool paused;
        private Queue<Action> launchQueue;
        private System.Timers.Timer launchTimer;

        // properties
        
        public AgentCount[] AgentCounts { get; set; }

        public double DayLengthInSeconds { get; set; }
        
        public double MetersPerPixel { get; set; }

        public double AgentSpeed { get; set; }
        
        public double[] SpeedDiversity { get; set; }
        
        public double[] PhysiologyDiversity { get; set; }

        public string Organisation { get; set; }
        
        public string Password { get; set; }

        public AgentEnvironmentDefinition EnvironmentDefinition { get; set; }

        public InstitutionManager Manager { get; private set; }
        
        public Institution Ei { get; private set; }
        
        public List<SimulationAgent> Agents { get; set; }

        public string RealTime {
            get {
                var realSeconds = Ei.Time.ElapsedMilliseconds / 1000f;

                TimeSpan t1 = TimeSpan.FromSeconds(realSeconds);

                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                    t1.Hours,
                    t1.Minutes,
                    t1.Seconds);

            }
        }

        public double SimulatedTime {
            get {
                var realSeconds = Ei.Time.ElapsedMilliseconds / 1000f;
                return realSeconds * (86400 / DayLengthInSeconds);
            }
        }

        public string SimulatedTimeString => FormatTime(this.SimulatedTime);

        public AgentEnvironment Environment => this.environment ?? (this.environment = new AgentEnvironment(this.EnvironmentDefinition));

        public double SimulatedSecond => 86400 / this.DayLengthInSeconds;

        public bool Paused {
            get => this.paused;
            set {
                this.paused = value;
                if (value) {
                    this.Waiter.Reset();
                }
                else {
                    this.Waiter.Set();
                }
            }
        }

        public ManualResetEvent Waiter { get; set; }
        public bool Started { get; set; }


        // events

        public event Action<SimulationAgent> AgentLaunched;
        public event PropertyChangedEventHandler PropertyChanged;

        // constructor

        public Project() {
            this.launchQueue = new Queue<Action>();
        }
        
        // methods
        
        public void SetDirectory(string dir) {
            this.directory = dir;
        }
    
        public void Start(Institution ei, int launchAgentsPerSecond) {
            // init institution
            this.Ei = ei;
            this.Ei.Resources.Tick = (float)(86400f / this.DayLengthInSeconds);
            this.Waiter = new ManualResetEvent(true);
            
            // init
            this.Agents = new List<SimulationAgent>();

            // start institution
            Manager = (InstitutionManager)InstitutionManager.Launch(this.Ei);
            Ei = Manager.Ei;
            Ei.Start();
            
            var agentsLaunched = 0;
            var totalAgents = 0;

            // set up timer
            launchTimer = new System.Timers.Timer();
            launchTimer.Interval = (1f / launchAgentsPerSecond) * 1000;
            launchTimer.Elapsed += LaunchAgent;
            launchTimer.AutoReset = false;

            this.launchQueue = new Queue<Action>();

            // launch all agents
            var counts = new int[this.AgentCounts.Length];

            for (var i = 0; i < this.AgentCounts.Length; i++) {
                totalAgents += this.AgentCounts[i].Count;
                counts[i] = this.AgentCounts[i].Count;
            }

            // we are launching agents one by category
            while (totalAgents > 0) {
                for (var i = 0; i < this.AgentCounts.Length; i++) {
                    if (counts[i] == 0) {
                        continue;
                    }
                    counts[i] = counts[i] - 1;
                    totalAgents--;

                    var agentCount = this.AgentCounts[i];
                    // connect agent to the institutions

                    this.launchQueue.Enqueue(() => {
                        var agent = this.CreateAgent(agentCount.Groups, agentCount.FreeTimeGoals);

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

        public double CalculateDuration(float waitTimeInSeconds) {
            return waitTimeInSeconds / SimulatedSecond;
        }

        public static string FormatTime(double simulatedSeconds) {
            TimeSpan t2 = TimeSpan.FromSeconds(simulatedSeconds);

            return string.Format("{0}d {1:D2}:{2:D2}",
                t2.Days,
                t2.Hours,
                t2.Minutes,
                t2.Seconds);
        }
        
        // abstract methods

        protected abstract SimulationAgent CreateAgent(string[][] groups, string[] goals);
        
        // private methods
        
        private void LaunchAgent(object sender, ElapsedEventArgs e) {
            if (this.launchQueue.Count == 0) {
                this.Started = true;
                return;
            }
            var launchAction = this.launchQueue.Dequeue();
            launchAction();

            this.launchTimer.Start();
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
