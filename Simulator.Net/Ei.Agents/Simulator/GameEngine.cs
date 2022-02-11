using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Ei.Simulation.Statistics;
using Ei.Logs;

namespace Ei.Simulation.Simulator
{
    public class GameEngine : IGameEngine
    {
        // fields

        public static GameEngine Instance;

        private Scene scene;
        private GameObject[] updateableGameObject;
        private bool changedStructure;
        private List<CancellationTokenSource> processThreads;

        private object locker = new object();
        public bool IsRunning { get; private set; }
        
        // properties
        
        public Collection<GameObject> GameObjects { get; private set; }
        public Dictionary<Type, List<MonoBehaviour>> Behaviours { get; private set; }
        public List<StatisticTrait> StatisticData { get; private set; }


        public GameEngine(Scene scene) {
            GameEngine.Instance = this;
            GameObject.simulation = this;
            this.scene = scene;
            this.IsRunning = false;

            this.Behaviours = new Dictionary<Type, List<MonoBehaviour>>();
            this.GameObjects = new ObservableCollection<GameObject>(scene.GameObjects);
            this.StatisticData = new List<StatisticTrait>();

            this.changedStructure = true;
            this.processThreads = new List<CancellationTokenSource>();
        }
        
        // events


        public event Action<StatisticTrait, DataPoint> StatisticTraitUpdated;
        
        // methods

        public void Start() {

            this.IsRunning = true;

            // init timer
            Time.Start();

            // init agents
            foreach (var agent in this.GameObjects)
            {
                agent.Init(this);
            }

            // start all agents 
            for (var i = 0; i < this.GameObjects.Count; i++) {
                // add the visual renderer
                if (this.GameObjects[i].Enabled) {
                    this.Instantiate(this.GameObjects[i], false);
                }
            }
            
            // start processing frames
            this.RunUpdate();
        }

        public void Stop() {
            foreach (var thread in this.processThreads) {
                thread.Cancel();
            }
            this.processThreads.Clear();
            this.IsRunning = false;
        }

        
        private void ProcessFrame() {
            
            lock (locker) {
                if (this.changedStructure) {
                    this.updateableGameObject = this.GameObjects.Where(a => a.Updates).OrderBy(a => a.layer).ToArray();
                }
            }

            // start all agents 
            for (var i = 0; i < this.updateableGameObject.Length; i++) {
                var agent = this.updateableGameObject[i];
                if (!agent.Enabled) {
                    continue;
                }
                agent.Update();
                
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
            var cancelSource = new CancellationTokenSource();
            var thread = new Thread(() => {
                while (true) {
                    cancelSource.Token.ThrowIfCancellationRequested();
                    statistic.ProcessSamples();
                    // this.PlotModel.InvalidatePlot(true);
                    Thread.Sleep(statistic.ProcessTimeInMilliseconds);
                }
            }) {IsBackground = true};
            thread.Start();

            this.processThreads.Add(cancelSource);
        }

        /// <summary>
        /// Creates and initialises a new gameobject
        /// </summary>
        /// <param name="agent">Gameobject to be created</param>
        /// <param name="init">Should Init function be called?</param>
        /// <returns>Created GameObject</returns>
        public GameObject Instantiate(GameObject agent, bool init = true) {
            lock (locker) {
                agent.Enabled = true;

                if (init)
                {
                    agent.Init(this);
                }
                agent.Start();

                // add it to the agents
                if (this.GameObjects.IndexOf(agent) == -1) {
                    this.AddGameObject(agent);
                }

                // filter 
                this.changedStructure = true;
                
                return agent;
            }
        }

        private void RunUpdate() {
            var cancelSource = new CancellationTokenSource();
            var thread = new Thread(() => {
                while (true) {
                    if (cancelSource.Token.IsCancellationRequested)
                    {
                        Log.Warning("Game Engine", "Stopping Game Engine ...");
                        break;
                    }
                    this.ProcessFrame();
                }
            });
            thread.IsBackground = true;
            thread.Start();

            this.processThreads.Add(cancelSource);
        }

        public IEnumerable<T> FindObjectsOfType<T>() where T : MonoBehaviour
        {
            // return GameObject.simulation.Behaviours[typeof(T)].Cast<T>().ToList();
            if (this.Behaviours.ContainsKey(typeof(T)))
            {
                return this.Behaviours[typeof(T)].Cast<T>();
            }
            return null; // new T[0];
        }

        public T FindObjectOfType<T>() where T : MonoBehaviour
        {
            // return GameObject.simulation.Behaviours[typeof(T)].Cast<T>().ToList();
            if (this.Behaviours.ContainsKey(typeof(T)))
            {
                return this.Behaviours[typeof(T)].Cast<T>().First();
            }
            return null; // new T[0];
        }

        private void AddGameObject(GameObject go) {
            this.GameObjects.Add(go);
        }

        public void Destroy(GameObject go) {
            this.GameObjects.Remove(go);
        }

        protected virtual void OnStatisticTraitUpdated(StatisticTrait trait, DataPoint gameObject) {
            StatisticTraitUpdated?.Invoke(trait, gameObject);
        }
    }
}
