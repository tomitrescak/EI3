using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Ei.Simulation.Statistics;

namespace Ei.Simulation.Simulator
{
    public class Runner : ISimulation
    {
        // fields

        public static Runner Instance;

        private Scene scene;
        private GameObject[] _updatableAgents;
        private bool changedStructure;
        private List<Thread> processThreads;

        private object locker = new object();
        
        // properties
        
        public Collection<GameObject> Agents { get; private set; }
        public Dictionary<Type, List<MonoBehaviour>> Behaviours { get; private set; }
        public List<StatisticTrait> StatisticData { get; private set; }

        public Runner(Scene scene) {
            Runner.Instance = this;
            GameObject.simulation = this;
            this.scene = scene;

            this.Behaviours = new Dictionary<Type, List<MonoBehaviour>>();
            this.Agents = new ObservableCollection<GameObject>(scene.GameObjects);
            this.StatisticData = new List<StatisticTrait>();

            this.changedStructure = true;

            // init agents
            foreach (var agent in this.Agents) {
                agent.Init(this);
            }
            
            this.processThreads = new List<Thread>();
        }
        
        // events

        public event Action<Runner, GameObject> GameObjectAdded;
        public event Action<Runner, GameObject> GameObjectRemoved;
        public event Action<StatisticTrait, DataPoint> StatisticTraitUpdated;
        
        // methods

        public void Start() {

            // init timer
            Time.Start();

            // start all agents 
            for (var i = 0; i < this.Agents.Count; i++) {
                // add the visual renderer
                if (this.Agents[i].Enabled) {
                    this.CreateInstance(this.Agents[i], false);
                }
            }
            
            // start processing frames
            this.RunUpdate();
        }

        public void Stop() {
            foreach (var thread in this.processThreads) {
                thread.Abort();
            }
            this.processThreads.Clear();
        }

        
        public void ProcessFrame() {
            
            lock (locker) {
                if (this.changedStructure) {
                    this._updatableAgents = this.Agents.Where(a => a.Updates).OrderBy(a => a.layer).ToArray();
                }
            }

            // start all agents 
            for (var i = 0; i < this._updatableAgents.Length; i++) {
                var agent = this._updatableAgents[i];
                if (!agent.Enabled) {
                    continue;
                }
//                if (!agent.Started) {
//                    agent.Start();
//                }

                if (agent.Started) {
                    agent.Update();
                }
            }
            
            // sleep for the rest if we are over 120 FPS
            
            Time.FrameEnd();
        }
        
        public void ProcessStatistics(IStatistic statistic)
        {
            // add given traits
            this.StatisticData.AddRange(statistic.Traits);

            // add listeners
            statistic.Traits.ForEach(t => t.PointAdded += (trait, point) => this.OnStatisticTraitUpdated(trait, point));
            
            // init axes
//            foreach (var axis in statistic.Axes) {
//                if (this.PlotModel.Axes.All(a => a.Key != axis.Key)) {
//                    // add exis if it does not exists
//                    this.PlotModel.Axes.Add(axis);
//                }
//            }

            // sort traits by name
            this.StatisticData.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            
            // init datacontext
            var thread = new Thread(() => {
                while (true) {
                    statistic.ProcessSamples();
                    // this.PlotModel.InvalidatePlot(true);
                    Thread.Sleep(statistic.ProcessTimeInMilliseconds);
                }
            }) {IsBackground = true};
            thread.Start();

            this.processThreads.Add(thread);
        }

        /// <summary>
        /// Creates and initialises a new gameobject
        /// </summary>
        /// <param name="agent">Gameobject to be created</param>
        /// <param name="init">Should Init function be called?</param>
        /// <param name="renderer">0 = Nothing, 1=Fast Renderer, 2=Icon Renderer, 3=Sim Renderer</param>
        /// <returns>Created GameObject</returns>
        public GameObject CreateInstance(GameObject agent, bool init = false, int renderer = 0) {
            lock (locker) {
                agent.Enabled = true;

                if (init) {
                    agent.Init(this);
                }

                agent.Start();
                agent.Started = true;

                // add it to the agents
                if (this.Agents.IndexOf(agent) == -1) {
                    this.AddAgent(agent);
                }

                // filter 
                this.changedStructure = true;
                
                return agent;
            }
        }

        private void RunUpdate() {
            var thread = new Thread(() => {
                while (true) {
                    this.ProcessFrame();
                }
            });
            thread.IsBackground = true;
            thread.Start();

            this.processThreads.Add(thread);
        }

        private void AddAgent(GameObject agent) {
            this.Agents.Add(agent);
            
            // notify
            this.OnGameObjectAdded(this, agent);
        }

        public void RemoveAgent(GameObject agent) {
            this.Agents.Remove(agent);
            
            // notify
            this.OnGameObjectRemoved(this, agent);
        }

        protected virtual void OnGameObjectAdded(Runner sender, GameObject gameObject) {
            GameObjectAdded?.Invoke(sender, gameObject);
        }

        protected virtual void OnGameObjectRemoved(Runner sender, GameObject gameObject) {
            GameObjectRemoved?.Invoke(sender, gameObject);
        }

        protected virtual void OnStatisticTraitUpdated(StatisticTrait trait, DataPoint gameObject) {
            StatisticTraitUpdated?.Invoke(trait, gameObject);
        }
    }
}
