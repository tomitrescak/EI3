using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Ei.Core.Ontology.Actions;
using Ei.Logs;
using Ei.Core.Ontology;
using Ei.Planning.Memory;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Strategies;
using Ei.Simulation.Planning;
using UnityEngine;
using Newtonsoft.Json;
using Ei.Simulation.Behaviours.Actuators;
using Ei.Simulation.Behaviours.Reasoners;

namespace Ei.Simulation.Behaviours
{
    public enum AgentState
    {
        Idle,
        Resting,
        Planning,
        ExecutingPlan
    }

    public class SimulationCallbacks : GovernorCallbacks
    {
        public SimulationCallbacks(SimulationAgent agent) : base(agent)
        {
        }

        public override void ExitedInstitution(string id, string name)
        {
            GameObject.Destroy(this.owner.gameObject);
        }
    }

    public class SimulationAgent : UnityEngine.MonoBehaviour, INotifyPropertyChanged
    {
       

        // static fields

        //[JsonIgnore]
        //public static int TotalPlans;
        //[JsonIgnore]
        //public static int FailedPlans;
        //[JsonIgnore]
        //public static long ResponseTimes;

        protected static readonly System.Random random = new System.Random();
        
        private static int index;


        // properties

        public int ReasoningIntervalInMs = 50; 
        public string[][] Groups;
        // public string[] FreeTimeGoalDefinition;

        // fields

        // private float destinationX;
        // private float destinationY;
        private List<AStarNode> plan;
        private AgentState state;
        // private GoalState[][] freeTimeGoals;
        
        // private NavigationBase navigation;
        private SimulationProject project;
        
        private Actuator actuator;
        private Reasoner reasoner;

        private float timeSinceLastReasoning;
        private float reasoningIntervalInSec;

        // protected SimulationTimer timer;
        // protected AgentEnvironment environment;
        protected bool authorised;

        public static double RandomInterval(double from, double to) {
            return random.Next((int)(from * 1000), (int)(to * 1000)) / 1000d;
        }

        // constructor

        public SimulationAgent() {
            
            // this.name = name ?? (groups[0][0] + "_" + index++);
            
            // this.PlanHistory = new List<PlanHistory>();
            this.Callbacks = new SimulationCallbacks(this);

            this.reasoningIntervalInSec = this.ReasoningIntervalInMs / 1000f;
            this.timeSinceLastReasoning = this.reasoningIntervalInSec + 1;
            

            //            var timer = new Timer();
            //            timer.Interval = 50;
            //            timer.Elapsed += TimerOnElapsed;
            //            timer.AutoReset = true;
            //            timer.Start();


            // speed modifier calculates how many pixels agent can pass in one second
            // that is the speed expressed as S = px/sec
            // as a result we can calculate, that if agent needs to pass N pixels it will take agent N / S seconds


            // this.Properties = new List<AgentProperty>();
        }

        // properties

        [JsonIgnore]
        public string Name => this.gameObject.name;

        //[JsonIgnore]
        //public string CurrentGoalAction { get; set; }

        // public List<AgentProperty> Properties { get; private set; }

        //[JsonIgnore]
        //public List<AStarNode> Plan {
        //    get => this.plan;
        //    protected set {
        //        this.plan = value;
        //        this.OnPropertyChanged("Plan");
        //    }
        //}

        //[JsonIgnore]
        //public List<PlanHistory> PlanHistory { get; private set; }

        //[JsonIgnore]
        //public GoalState[][] FreeTimeGoals {
        //    get {
        //        if (this.freeTimeGoals == null && this.FreeTimeGoalDefinition != null) {
        //            this.freeTimeGoals = new GoalState[FreeTimeGoalDefinition.Length][];
        //            for (var i = 0; i < FreeTimeGoalDefinition.Length; i++) {
        //                this.FreeTimeGoals[i] = GoalState.ParseStringGoals(this.Governor, FreeTimeGoalDefinition[i]);
        //            }
        //        }
        //        return this.freeTimeGoals;
        //    }
        //}

        [JsonIgnore]
        public Governor Governor { get; set; }

        protected IGovernorCallbacks Callbacks { get; set; }

        //[JsonIgnore]
        //public AgentState State {
        //    get => this.state;
        //    set {
        //        this.state = value;
        //        this.OnPropertyChanged("State");
        //    }
        //}

        #region Behaviours
        

        // public void UpdateOnUi(Action action)
        //{
        //    action();

           
        //}

        public virtual void Start()
        {
            this.project = FindObjectOfType<SimulationProject>();
            this.actuator = GetComponent<Actuator>();
            if (this.actuator == null)
            {
                throw new Exception("Agent needs an actuator");
            }
            this.reasoner = GetComponent<Reasoner>();
            if (this.reasoner == null)
            {
                throw new Exception("Agent needs a reasoner");
            }

            //this.navigation = GetComponent<NavigationBase>();         
            //this.timer = FindObjectOfType<SimulationTimer>();
            //this.environment = FindObjectOfType<AgentEnvironment>();

            this.Connect();

        }

        public void Update()
        {
            if (!this.authorised)
            {
                return;
            }

            // Console.WriteLine(Time.deltaTime + " " + this.timeSinceLastReasoning);

            // we reason only in specified intervals
            this.timeSinceLastReasoning += Time.deltaTime;
            if (this.timeSinceLastReasoning > this.reasoningIntervalInSec)
            {
                this.timeSinceLastReasoning = 0;
                this.reasoner.Reason();
            }

        }
        #endregion

        // virtual methods

        protected virtual bool Connected() {
            return true;
        }

        // methods

        public void Connect() {

            this.authorised = false;
            InstitutionManager manager = this.project.Manager;
            string organisation = this.project.Organisation;
            string password = this.project.Password;

            Governor governor;
            var result = manager.Connect(
                this.Callbacks, 
                organisation, 
                this.gameObject.name, 
                password, 
                this.Groups, 
                out governor);

            this.Governor = governor;

            if (result != InstitutionCodes.Ok) {
                Log.Error(this.gameObject.name, $"[ERROR] Could not connect to insitutiton:  {result.ToString()}");
                this.gameObject.Enabled = false;
                return;
            }

            // initialise agent

            this.authorised = true;

            if (this.Connected()) {

                // announce all parameters
                foreach (var group in this.Governor.Resources.Groups) {
                    var role = group.Role;
                    var pars = role.FilterByAccess(this.Governor);
                    foreach (var param in pars) {
                        this.Callbacks.NotifyAgentParameterChanged(this.Governor.Name, param.Name, param.Value);
                    }
                }

                // automatically start participation in the institution
                this.Governor.Continue();
            }

            
        }


        //private static int tid;
        //protected void CreatePlan(Governor agent, GoalState[] goal, string goalType = null) {
        //    this.State = AgentState.Planning;

        //    // var t = new Thread(() => FindPlan(agent, goal, goalType));
        //    // t.Name = "Thread_" + tid++;
        //    // t.Start();

        //    FindPlan(agent, goal, goalType);
        //}

        //private void UpdatePlan() {

        //}

        //const bool useCache = true;
       

        //private void ContinuePlan(Governor agent) {
        //    Log.Debug(agent.Name, "[WF] Continuing plan ...");

        //    this.ContinuePlan(agent, true);
        //}

        //protected void ContinuePlan(Governor agent, bool removeFirst) {
        //    this.State = AgentState.ExecutingPlan;

        //    //Debug.WriteLine("[{0}] Executing {1} ({2}): {3}", agent.Name, this.plan.Count, this.plan.Count > 0 ? this.plan[0].Arc.ToString() : "FINISH", agent.Properties.GetParameterValue("a.Pots"));

        //    // remove all unnecessary nodes
        //    while (this.Plan.Count > 0 && (this.Plan[0] == null || this.Plan[0].Arc == null || this.Plan[0].Arc.Action == null)) {
        //        var item = this.Plan[0];
        //        if (item.Arc.From != null && item.Arc.To != null && item.Arc.From.Id != item.Arc.To.Id) {
        //            this.Governor.Move(item.Arc.To.Id);
        //        }
        //        //this.Plan.RemoveAt(0);
        //        this.UpdateOnUi(() => {
        //            try {
        //                this.Plan.RemoveAt(0);
        //                this.OnPropertyChanged("Plan");
        //            } catch { }
        //        });
        //    }


        //    if (this.Plan.Count > 0) {
        //        // go to next planned destination
        //        var itemId = this.Plan[0].CostData;


        //        // check whether it is an environmental object or object with no location
        //        // item id is null for example for workflow actions

        //        if (itemId == null || this.environment.NoLocationInfo(itemId) != null) {
        //            Log.Debug(agent.Name, "[CP] No travel needed ...");

        //            this.ExecuteNextPlanStep(agent);
        //        } else {
        //            // travel to destination of the object
        //            EnvironmentData obj;
        //            if (this.environment.TryGetValue(itemId, out obj)) {
        //                this.destinationX = obj.X;
        //                this.destinationY = obj.Y;

        //                Log.Debug(agent.Name, "[CP] Moving to destination ...");

        //                this.MoveToDestination(this.destinationX, this.destinationY);
        //            } else {
        //                this.FailPlan(agent, "Missing resouce: " + itemId);
        //            }
        //        }
        //    } else {
        //        this.FinishPlan(agent);
        //    }
        //}

        //private void FinishPlan(Governor agent) {
        //    this.UpdateOnUi(() => this.PlanHistory[0].Result = "Finished");

        //    Log.Debug(agent.Name, "[CP] Plan finished ...");

        //    this.State = AgentState.Idle;

        //    // check if we have not finished. in that case workflow is null
        //    if (this.Governor.Workflow != null)
        //    {
        //        this.actionQueue.Add(new ActionItem(Time.time, this.Reason));
        //    }
        //}

        //private void FailPlan(Governor agent, string reason) {
        //    this.UpdateOnUi(() => {
        //        this.PlanHistory[0].Result = "Failed";
        //        this.PlanHistory[0].Failed = true;
        //    });

        //    // object is no longer there, so fail the plan!
        //    Log.Error(agent.Name, "Plan failed!: " + reason);

        //    // TODO: Dangerous, should not happen, or remove agent when it happens
        //    if (agent.Workflow == null) {
        //        return;
        //    }

        //    // return to the top workflow
        //    while (agent.Workflow.Parent != null) {
        //        agent.ExitWorkflow();
        //    }

        //    this.Plan.Clear();

        //    this.State = AgentState.Idle;
        //    this.actionQueue.Add(new ActionItem(0, this.RandomWalk));
        //}

        //protected void RandomWalk() {
        //    this.MoveToDestination(this.environment.RandomX, this.environment.RandomY);
        //}

        //protected virtual void MoveToDestination(float x, float y) {
        //    if (Math.Abs(this.transform.X - x) < 0.00001 && Math.Abs(this.transform.Y - y) < 0.00001) {
        //        this.MovedToLocation();
        //        return;
        //    }

        //    this.destinationX = x;
        //    this.destinationY = y;

        //    this.navigation.MoveToDestination(this.destinationX, this.destinationY);
        //}


        // callback methods

        //public virtual void MovedToLocation() {
        //    this.actionQueue.Add(new ActionItem(Time.time, this.Reason));
        //}

        // helpers

        //        public static void RunAfter(System.Action action, float milliseconds)
        //        {
        //            var dispatcherTimer = new System.Timers.Timer() { Interval = milliseconds };
        //            dispatcherTimer.AutoReset = false;
        //            dispatcherTimer.Elapsed += (sender, args) =>
        //            {
        //                action();
        //            };
        //            dispatcherTimer.Start();
        //        }

        // property changed handler

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
