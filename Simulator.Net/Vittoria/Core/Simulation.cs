using Ei.Agents.Core;
using Ei.Agents.Sims;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UnityEngine;
using Vittoria.Behaviours;
using Vittoria.Statistics;

namespace Vittoria.Core
{
    public class Simulation : ISimulation
    {
        // fields

        public static Simulation Instance;

        private IProject project;
        private Canvas canvas;
        private Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();
        private Dictionary<string, Image> objects = new Dictionary<string, Image>();
        public WriteableBitmap writeableBmp;
        private GameObject[] _updatableAgents;
        private bool changedStructure;

        // properties

        public Vittoria.Controls.Statistics Statistics { get; set; }
        public Collection<GameObject> Agents { get; private set; }
        public Dictionary<Type, List<MonoBehaviour>> Behaviours { get; private set; }
        public Canvas Canvas { get { return canvas; } }
        public BitmapContext BitmapContext { get; private set; }

        

        public Simulation(IProject project, Canvas canvas, WriteableBitmap writeableBmp) {
            Simulation.Instance = this;
            GameObject.simulation = this;
            this.project = project;
            this.canvas = canvas;

            if (this.project == null)
            {
                return;
            }

            this.Behaviours = new Dictionary<Type, List<MonoBehaviour>>();
            this.Agents = new ObservableCollection<GameObject>(project.Agents);
            this.writeableBmp = writeableBmp;

            this.changedStructure = true;

            // init agents
            foreach (var agent in this.Agents) {
                agent.Init(this);
            }
        }

        public void Start() {
            // init timer
            Time.Start();

            // notify project
            // this.project.Start();


            // add fps statistic
            // this.Statistics.ProcessStatistics(new FpsStatistics());

            // start all agents 
            for (var i = 0; i < this.Agents.Count; i++) {
                // add the visual renderer
                if (this.Agents[i].Enabled) {
                    this.CreateInstance(this.Agents[i], false);
                }
            }
        }

        public void ProcessFrame() {
            if (this.changedStructure) {
                this._updatableAgents = this.Agents.Where(a => a.Updates).OrderBy(a => a.layer).ToArray();
            }

            using (this.BitmapContext = this.writeableBmp.GetBitmapContext()) {
                this.writeableBmp.Clear(Colors.White);

                // start all agents 
                for (var i = 0; i < this._updatableAgents.Length; i++) {
                    var agent = this._updatableAgents[i];
                    if (!agent.Enabled) {
                        continue;
                    }
                    if (!agent.Started) {
                        agent.Start();
                    }
                    agent.Update();
                }
            }
        }

        /// <summary>
        /// Creates and initialises a new gameobject
        /// </summary>
        /// <param name="agent">Gameobject to be created</param>
        /// <param name="init">Should Init function be called?</param>
        /// <param name="renderer">0 = Nothing, 1=Fast Renderer, 2=Icon Renderer, 3=Sim Renderer</param>
        /// <returns>Created GameObject</returns>
        public GameObject CreateInstance(GameObject agent, bool init = false, int renderer = 0) {
            agent.Enabled = true;

            // add renderer
            switch (renderer) {
                case 1:
                    agent.AddComponent<FastRenderer>();
                    break;
                case 2:
                    agent.AddComponent<IconRenderer>();
                    break;
                case 3:
                    agent.AddComponent<SimRenderer>();
                    break;
            }

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

        private void AddAgent(GameObject agent) {
            // check thread
            if (Application.Current == null) return;
            if (!Application.Current.Dispatcher.CheckAccess()) {
                Application.Current.Dispatcher.Invoke(() => this.AddAgent(agent));
                return;
            }
            this.Agents.Add(agent);
        }

        public void RemoveAgent(GameObject agent) {
            if (Application.Current == null) return;
            if (!Application.Current.Dispatcher.CheckAccess()) {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => RemoveAgent(agent)));
                return;
            }
            this.Agents.Remove(agent);
        }
    }
}
