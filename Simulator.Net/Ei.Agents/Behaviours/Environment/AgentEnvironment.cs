using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Ei.Core.Ontology.Actions;
using Ei.Core.Runtime;
using Ei.Simulation.Behaviours.Environment;
using Ei.Simulation.Behaviours.Environment.Objects;
using Ei.Simulation.Sims.Behaviours;
using Newtonsoft.Json;
using UnityEngine;
using Action = Ei.Core.Ontology.Actions.ActionBase;

namespace Ei.Simulation.Behaviours
{
    public class AgentEnvironment: MonoBehaviour
    {
        // private int period = 0;

        #region Fields
        private int width;
        private int height;
        private int normalisationFactor = 10;
        
        private readonly Dictionary<string, Dictionary<string, double>> distances;
        private readonly Dictionary<string, List<EnvironmentObject>> actions;
        private readonly List<EnvironmentObject> objects;
        
        //private readonly Queue<System.Action> actionQueue;
        //private readonly AutoResetEvent actionStop;
        // private readonly Dictionary<EnvironmentObject, GameObject> objectMappings;

        public AgentEnvironmentDefinition Definition;
        #endregion

        #region Properties
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, double>> Distances { get { return distances; } }

        [JsonIgnore]
        public Dictionary<string, List<EnvironmentObject>> Actions
        {
            get
            {
                return actions;
            }
        }

        [JsonIgnore]
        public List<EnvironmentObject> Objects
        {
            get
            {
                return objects;
            }
        }

        protected Dictionary<string, NoLocationAction> ActionsWithNoLocation { get; private set; }

        // properties


        #endregion


        // constructors

        public AgentEnvironment()
        {
            this.Definition = new AgentEnvironmentDefinition();
            this.distances = new Dictionary<string, Dictionary<string, double>>();
            this.actions = new Dictionary<string, List<EnvironmentObject>>();
            this.objects = new List<EnvironmentObject>();
            //this.objectMappings = new Dictionary<EnvironmentObject, GameObject>(); 
            //this.actionQueue = new Queue<System.Action>(1000);
            //this.actionStop = new AutoResetEvent(false);
        }

        // methods

        public void Init()
        {
            this.width = this.Definition.Width;
            this.height = this.Definition.Height;

            this.OpenEnvironment();

            //var thread = new Thread(ProcessQueue);
            //thread.IsBackground = true;
            //thread.Start();
        }

        //private void ProcessQueue()
        //{
        //    while (true)
        //    {
        //        while (this.actionQueue.Count > 0)
        //        {
        //            // execute action
        //            this.actionQueue.Dequeue()();
        //        }

        //        // wait for next impulse
        //        this.actionStop.WaitOne();
        //    }
        //}

        public void Start()
        {
            

            //            if (this.definition.Elements != null && this.definition.Elements.Length > 0)
            //            {
            //                var timer = new Timer(2000);
            //                timer.Elapsed += Evolve;
            //                timer.Enabled = true;
            //                timer.AutoReset = true;
            //            }
        }

        private void OpenEnvironment()

        {

            if (this.Definition == null)
            {
                throw new Exception("You need environment definition to evolve!");
            }

            // add actions with no location
            if (this.Definition.ActionsWithNoLocation != null)
            {
                this.ActionsWithNoLocation = new Dictionary<string, NoLocationAction>();
                foreach (var act in this.Definition.ActionsWithNoLocation)
                {
                    this.ActionsWithNoLocation.Add(act.Id, act);
                }
            }
        }

        public NoLocationAction NoLocationInfo(string actionId)
        {
            return this.ActionsWithNoLocation != null ? this.ActionsWithNoLocation.FirstOrDefault(w => w.Key == actionId).Value : null;
        }

        //public void AddNoLocationAction(string id, int destroyAfter = 0, float duration = 0)
        //{
        //    if (this.ActionsWithNoLocation == null)
        //    {
        //        this.ActionsWithNoLocation = new Dictionary<string, EnvironmentDataAction>();
        //    }

        //    this.ActionsWithNoLocation.Add(id, new EnvironmentDataAction { Id = id, DestroyAfter = destroyAfter, Duration = duration });
        //}


        //        private void Evolve(object sender, ElapsedEventArgs elapsedEventArgs)
        //        {
        //            var objs = this.objects.ToArray();
        //
        //            foreach (var obj in objs)
        //            {
        // 
        //                if (obj.Definition.CanCreate() && Probability(obj.Definition.Probability))
        //                {
        //                    var def = obj.Definition;
        //
        //                    var newX = obj.X + this.rnd.Next(-def.Range[0], def.Range[0]);
        //                    var newY = obj.Y + this.rnd.Next(-def.Range[1], def.Range[1]);
        //
        //                    this.AddObject(new EnvironmentData(def, newX, newY));
        //                }
        //                
        //            }
        //
        //            // now check the completely missing values
        //
        //            foreach (var element in this.definition.Elements)
        //            {
        //                var found = objs.Any(w => w.Definition == element);
        //
        //                if (!found)
        //                {
        //                    var newcount = this.rnd.Next(0, element.Seed);
        //                    for (var i = 0; i < newcount; i++)
        //                    {
        //                        this.AddObject(new EnvironmentData(element, this.RandomX, this.RandomY));
        //                    }
        //                }
        //            }
        //        }
        //
        //        private bool Probability(float probability)
        //        {
        //            return this.rnd.Next(1, 10001) <= (probability * 100);
        //
        //        }

        //private void InitEnvironment(EnvironmentData[] environment, int normalisationFactor)
        //{
        //    this.normalisationFactor = normalisationFactor;

        //    // browse all elements and build all the tables

        //    foreach (var edx in environment)
        //    {
        //        this.AddObject(edx);
        //    }
        //}

        //        private void AddObject(EnvironmentDataDefinition def)
        //        {
        //            this.AddObject();
        //        }

        //public void AddObject(string id)
        //{
        //    this.AddObject(id, this.RandomX, this.RandomY);
        //}

        //public void AddObject(string id, int x, int y, Dictionary<string, VariableInstance[]> parameters = null, Governor owner = null)
        //{
        //    var definition = this.Definition.Elements.First(w => w.Id == id);
        //    var data = new EnvironmentData(definition, x, y, parameters, owner);

        //    // add object to canvas

        //    this.AddObject(data);
        //}

        public void AddObject(EnvironmentObject edx)
        {
            // rememebr all objects

            if (this.Objects.Any(o => o.Name == edx.Name)) {
                throw new Exception("You cannot have two environment objects with the same name: " + edx.Name);
            }

            this.objects.Add(edx);

            // prepare distance record

            if (!this.distances.ContainsKey(edx.Name))
            {
                this.distances.Add(edx.Name, new Dictionary<string, double>());
            }

            // create distances

            for (int index = 0; index < this.objects.Count; index++)
            {
                var edy = this.objects[index];
                if (edx.Name == edy.Name)
                {
                    this.distances[edx.Name][edy.Name] = this.normalisationFactor / 2f;
                }
                else
                {
                    this.distances[edx.Name][edy.Name] = this.Distance(edx.transform.X, edx.transform.Y, edy.transform.X, edy.transform.Y);
                }
            }

            // remember all actions

            foreach (var action in edx.Actions)
            {
                if (!this.actions.ContainsKey(action.Id))
                {
                    this.actions.Add(action.Id, new List<EnvironmentObject>());
                }
                this.actions[action.Id].Add(edx);
            }


            // create the new gameobject


            // instantiate new simobject
            //var agent = new GameObject(edx.Id);
            //agent.transform.position = new Vector3(edx.X, edx.Y, 0);

            //// add simobject
            //var sim = agent.AddComponent<SimObject>();
            //sim.Icon = edx.Definition.Image;

            //this.objectMappings.Add(edx, agent);

            //Instantiate(agent);
        }

        public double Distance(float x1, float y1, float x2, float y2)
        {
            var xs = x1 - x2;
            var ys = y1 - y2;

            return (Math.Sqrt(xs * xs + ys * ys) / this.width) * normalisationFactor;
        }

        //public float UseObject(EnvironmentObject obj, string actionId)
        //{
        //    var remainingUsage = obj.Use(actionId);

        //    if (remainingUsage == 0)
        //    {
        //        this.RemoveObject(obj);
        //    }

        //    // return duration to use this object

        //    return obj.Definition.Actions.First(w => w.Id == actionId).Duration;
        //}

        //public float UseObject(string itemId, string actionId)
        //{
        //    var obj = this.objects.Find(w => w.Id == itemId);
        //    return this.UseObject(obj, actionId);
        //}

        //public void RemoveObject(string definitionId)
        //{
        //    var obj = this.objects.FirstOrDefault(w => w.Definition.Id == definitionId);
        //    if (!string.IsNullOrEmpty(obj.Id))
        //    {
        //        this.RemoveObject(obj);
        //    }
        //}

        public void RemoveObject(EnvironmentObject obj)
        {
            // remove from objects

            this.objects.Remove(obj);

            // remove from action providers

            foreach (var list in this.actions.Values)
            {
                list.Remove(obj);
            }
        }

        public bool TryGetValue(string itemId, out EnvironmentObject environmentData)
        {
            try
            {
                environmentData = this.objects.FirstOrDefault(w => w.Name == itemId);
            }
            catch (Exception)
            {
                environmentData = null;
            }
            return environmentData != null;

        }
    }
}
