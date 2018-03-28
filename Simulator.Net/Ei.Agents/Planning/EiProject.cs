using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Runtime.Planning.Environment;
using Ei.Simulator.Core;
using UnityEngine;
using Project = Ei.Simulator.Core.Project;
using Ei.Agents.Sims;
using Ei.Agents.Core.Behaviours;
using System.Threading;
using Ei.Agents.Core;
using System.IO;

namespace Ei.Agents.Planning
{
    public class EiProject : EiBehaviour
    {
        private Dictionary<EnvironmentData, GameObject> objectMappings;
        private Dictionary<PhysiologyBasedAgent, GameObject> agentMappings;

        public string ProjectPath { get; set; }
        public int AgentsPerSecond { get; set; }
        public int AgentsLaunched { get; private set; }

        public Project project { get; set; }

        public void Init() {
            var path = Path.Combine(Environment.CurrentDirectory, this.ProjectPath);
            this.project = Project.Open(path);
        }

        public void InitProject(Project project) {
            this.project = project;
            if (this.project != null) {
                this.project.Manager.Stop();
            }

            this.objectMappings = new Dictionary<EnvironmentData, GameObject>();
            this.agentMappings = new Dictionary<PhysiologyBasedAgent, GameObject>();

            // register listeners
            this.project.AgentLaunched += ProjectOnAgentLaunched;

            // load environment
            this.project.Environment.ObjectAdded += AddObjectToCanvas;
            this.project.Environment.ObjectRemoved += RemoveObjectFromCanvas;
        }

        public void Start() {
            // open environment
            this.project.Environment.OpenEnvironment();

            // start this project
            var thread = new Thread(() => this.project.LazyStart(this.AgentsPerSecond));
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Lowest;
            thread.Start();

        }

        private void RemoveObjectFromCanvas(AgentEnvironment environment, EnvironmentData obj) {
            GameObject.Destroy(this.objectMappings[obj]);
        }

        private void AddObjectToCanvas(AgentEnvironment environment, EnvironmentData obj) {
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

        private void ProjectOnAgentLaunched(PhysiologyBasedAgent agent) {
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
    }
}
