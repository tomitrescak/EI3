﻿//using System;
//using System.Collections.Generic;
//using System.Threading;
//using Ei.Core.Ontology;
//using Ei.Core.Runtime.Planning.Environment;
//using Ei.Simulation.Core;
//using Ei.Simulation.Physiology;
//using Ei.Simulation.Sims.Behaviours;
//using Ei.Simulation.Simulator;
//using UnityEngine;
//using Project = Ei.Simulation.Simulator.Project;

//namespace Ei.Simulation.Behaviours
//{
//    public class EiProjectStarter : MonoBehaviour
//    {
//        private Dictionary<EnvironmentData, GameObject> objectMappings;
//        private Dictionary<PhysiologyBasedAgent, GameObject> agentMappings;

//        public string ProjectPath { get; set; }
//        public int AgentsPerSecond { get; set; }
//        public int AgentsLaunched { get; private set; }
        
//        public Project project { get; private set; }
//        public Institution ei { get; private set; }
////        public void Init() {
////            if (!string.IsNullOrEmpty(this.ProjectPath)) {
////                var path = Path.Combine(Environment.CurrentDirectory, this.ProjectPath);
////                this.project = Project.Open(path);
////
////                this.InitProject(this.project);
////            }    
////        }

//        public void InitProject(Project project, Institution ei) {
//            if (this.AgentsPerSecond == 0) {
//                throw new Exception("You need to launch at least one agent per second!");
//            }
//            this.project = project;
//            this.ei = ei;
            
//            if (this.project != null && this.project.Manager != null) {
//                this.project.Manager.Stop();
//            }

//            this.objectMappings = new Dictionary<EnvironmentData, GameObject>();
//            this.agentMappings = new Dictionary<PhysiologyBasedAgent, GameObject>();

//            // register listeners
//            this.project.AgentLaunched += ProjectOnAgentLaunched;

//            // load environment
//            this.project.Environment.ObjectAdded += AddObjectToCanvas;
//            this.project.Environment.ObjectRemoved += RemoveObjectFromCanvas;
//        }

//        public void Start() {
//            // open environment
//            this.project.Environment.OpenEnvironment();

//            // start this project
//            var thread = new Thread(() => this.project.Start(this.ei, this.AgentsPerSecond));
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

//        private void ProjectOnAgentLaunched(SimulationAgent agent) {
//            // instantiate new simobject
//            var newObj = new GameObject(agent.Name);
//            var a = agent as PhysiologyBasedAgent;
//            newObj.transform.position = new Vector3(agent.X, agent.Y, 0);

//            var sim = newObj.AddComponent<LinearNavigation>();
//            this.agentMappings.Add(a, newObj);

//            var eiAgent = newObj.AddComponent<EiAgent>();

//            eiAgent.agent = a;
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
//    }
//}