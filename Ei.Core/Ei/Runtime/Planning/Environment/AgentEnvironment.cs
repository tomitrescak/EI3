using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Ei.Core.Ontology.Actions;
using Action = Ei.Core.Ontology.Actions.ActionBase;

namespace Ei.Core.Runtime.Planning.Environment
{
    public class AgentEnvironment
    {
        // private int period = 0;

        private AgentEnvironmentDefinition definition;

        private readonly Dictionary<string, Dictionary<string, double>> distances;
        public Dictionary<string, Dictionary<string, double>> Distances { get { return distances; } }

        private readonly Dictionary<string, List<EnvironmentData>> actions;
        public Dictionary<string, List<EnvironmentData>> Actions {
            get {
                return actions;
            }
        }

        private readonly List<EnvironmentData> objects;
        public List<EnvironmentData> Objects {
            get {
                return objects;
            }
        }

        protected Dictionary<string, EnvironmentDataAction> ActionsWithNoLocation { get; private set; }

        private int width;
        private int height;

        private int normalisationFactor;

        private readonly Random rnd = new Random();

        // properties

        public int RandomX {
            get {
                return (int)(rnd.NextDouble() * (this.width - 50));
            }
        }

        public int RandomY {
            get {
                return (int)(rnd.NextDouble() * (this.height - 50));
            }
        }

        // events

        public event Action<AgentEnvironment, EnvironmentData> ObjectAdded;
        public event Action<AgentEnvironment, EnvironmentData> ObjectRemoved;

        private readonly Queue<System.Action> actionQueue;
        private readonly AutoResetEvent actionStop;

        // constructors

        private AgentEnvironment(int width, int height)
        {
            this.distances = new Dictionary<string, Dictionary<string, double>>();
            this.actions = new Dictionary<string, List<EnvironmentData>>();
            this.objects = new List<EnvironmentData>();

            this.width = width;
            this.height = height;

            this.actionQueue = new Queue<System.Action>(1000);
            this.actionStop = new AutoResetEvent(false);

            var thread = new Thread(ProcessQueue);
            thread.IsBackground = true;
            thread.Start();
        }

        public AgentEnvironment(EnvironmentData[] environment, int width, int height, int normalisationFactor) : this(width, height)
        {
            this.Init(environment, normalisationFactor);
        }

        public AgentEnvironment(AgentEnvironmentDefinition definition) : this(definition.Width, definition.Height)
        {
            this.definition = definition;
        }

        // methods

        private void ProcessQueue()
        {
            while (true)
            {
                while (this.actionQueue.Count > 0)
                {
                    // execute action
                    this.actionQueue.Dequeue()();
                }

                // wait for next impulse
                this.actionStop.WaitOne();
            }
        }

        public void Start()
        {
            if (this.definition == null)
            {
                throw new Exception("You need environment definition to evolve!");
            }

            //            if (this.definition.Elements != null && this.definition.Elements.Length > 0)
            //            {
            //                var timer = new Timer(2000);
            //                timer.Elapsed += Evolve;
            //                timer.Enabled = true;
            //                timer.AutoReset = true;
            //            }
        }

        public void OpenEnvironment()
        {
            // add actions with no location
            if (definition.ActionsWithNoLocation != null)
            {
                this.ActionsWithNoLocation = new Dictionary<string, EnvironmentDataAction>();
                foreach (var act in definition.ActionsWithNoLocation)
                {
                    this.ActionsWithNoLocation.Add(act.Id, act);
                }
            }

            // initialise seeds

            var data = new List<EnvironmentData>();

            if (definition.Elements != null)
            {
                foreach (var def in definition.Elements)
                {
                    // initialise actions

                    foreach (var action in def.Actions)
                    {
                        if (!this.actions.ContainsKey(action.Id))
                        {
                            this.actions.Add(action.Id, new List<EnvironmentData>());
                        }
                    }

                    // initialise objects


                    for (var i = 0; i < def.Seed; i++)
                    {
                        // initiate element at a random position
                        data.Add(new EnvironmentData(def, this.RandomX, this.RandomY));
                    }
                }
            }

            this.Init(data.ToArray(), 10);
        }

        public EnvironmentDataAction NoLocationInfo(string actionId)
        {
            return this.ActionsWithNoLocation != null ? this.ActionsWithNoLocation.FirstOrDefault(w => w.Key == actionId).Value : null;
        }

        public void AddNoLocationAction(string id, int destroyAfter = 0, float duration = 0)
        {
            if (this.ActionsWithNoLocation == null)
            {
                this.ActionsWithNoLocation = new Dictionary<string, EnvironmentDataAction>();
            }

            this.ActionsWithNoLocation.Add(id, new EnvironmentDataAction { Id = id, DestroyAfter = destroyAfter, Duration = duration });
        }


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

        private void Init(EnvironmentData[] environment, int normalisationFactor)
        {
            this.normalisationFactor = normalisationFactor;

            // browse all elements and build all the tables

            foreach (var edx in environment)
            {
                this.AddObject(edx);
            }
        }

        //        private void AddObject(EnvironmentDataDefinition def)
        //        {
        //            this.AddObject();
        //        }

        public void AddObject(string id)
        {
            this.AddObject(id, this.RandomX, this.RandomY);
        }

        public void AddObject(string id, int x, int y, Dictionary<string, VariableInstance[]> parameters = null, Governor owner = null)
        {
            var definition = this.definition.Elements.First(w => w.Id == id);
            var data = new EnvironmentData(definition, x, y, parameters, owner);

            // add object to canvas

            this.AddObject(data);
        }

        private void AddObject(EnvironmentData edx)
        {
            this.actionQueue.Enqueue(() => this.QueueAddObject(edx));
            this.actionStop.Set();
        }

        public void QueueAction(System.Action action)
        {
            this.actionQueue.Enqueue(action);
            this.actionStop.Set();
        }

        private void QueueAddObject(EnvironmentData edx)
        {
            // rememebr all objects

            this.objects.Add(edx);

            // prepare distance record

            if (!this.distances.ContainsKey(edx.Id))
            {
                this.distances.Add(edx.Id, new Dictionary<string, double>());
            }

            // create distances

            for (int index = 0; index < this.objects.Count; index++)
            {
                var edy = this.objects[index];
                if (edx.Id == edy.Id)
                {
                    this.distances[edx.Id][edy.Id] = this.normalisationFactor / 2f;
                }
                else
                {
                    this.distances[edx.Id][edy.Id] = this.Distance(edx.X, edx.Y, edy.X, edy.Y);
                }
            }

            // remember all actions

            foreach (var action in edx.Definition.Actions)
            {
                if (!this.actions.ContainsKey(action.Id))
                {
                    this.actions.Add(action.Id, new List<EnvironmentData>());
                }
                this.actions[action.Id].Add(edx);
            }


            if (this.ObjectAdded != null) {
                this.ObjectAdded(this, edx); // notify
            }
        }

        public double Distance(int x1, int y1, int x2, int y2)
        {
            var xs = x1 - x2;
            var ys = y1 - y2;

            return (Math.Sqrt(xs * xs + ys * ys) / this.width) * normalisationFactor;
        }

        public float UseObject(EnvironmentData obj, string actionId)
        {
            var remainingUsage = obj.Use(actionId);

            if (remainingUsage == 0)
            {
                this.RemoveObject(obj);
            }

            // return duration to use this object

            return obj.Definition.Actions.First(w => w.Id == actionId).Duration;
        }

        public float UseObject(string itemId, string actionId)
        {
            var obj = this.objects.Find(w => w.Id == itemId);
            return this.UseObject(obj, actionId);
        }

        public void RemoveObject(string definitionId)
        {
            var obj = this.objects.FirstOrDefault(w => w.Definition.Id == definitionId);
            if (!string.IsNullOrEmpty(obj.Id))
            {
                this.RemoveObject(obj);
            }
        }

        private void RemoveObject(EnvironmentData obj)
        {
            this.actionQueue.Enqueue(() => this.QueueRemoveObject(obj));
            this.actionStop.Set();
        }

        private void QueueRemoveObject(EnvironmentData obj)
        {
            // remove from objects

            this.objects.Remove(obj);

            // remove from action providers

            foreach (var list in this.actions.Values)
            {
                list.Remove(obj);
            }

            if (this.ObjectRemoved != null) {
                this.ObjectRemoved(this, obj); // notify
            }
        }

        public bool TryGetValue(string itemId, out EnvironmentData environmentData)
        {
            try
            {
                environmentData = this.objects.FirstOrDefault(w => w.Id == itemId);
            }
            catch (Exception)
            {
                environmentData = new EnvironmentData();
            }
            return !string.IsNullOrEmpty(environmentData.Id);

        }
    }
}
