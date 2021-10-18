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

namespace Ei.Simulation.Behaviours
{
    public enum AgentState
    {
        Idle,
        Resting,
        Planning,
        ExecutingPlan
    }

    public abstract class SimulationAgent : UnityEngine.MonoBehaviour, INotifyPropertyChanged
    {
        #region struct ActionItem
        private struct ActionItem
        {
            public float startTime;
            public Action action;

            public ActionItem(float startTime, Action action)
            {
                this.startTime = startTime;
                this.action = action;
            }
        } 
        #endregion

        // static fields

        [JsonIgnore]
        public static int TotalPlans;
        [JsonIgnore]
        public static int FailedPlans;
        [JsonIgnore]
        public static long ResponseTimes;

        protected static readonly System.Random random = new System.Random();
        
        private static int index;
        private static bool planning = false;

        // properties

        public string[][] Groups;
        public string[] FreeTimeGoalDefinition;

        // fields

        private float destinationX;
        private float destinationY;
        private List<AStarNode> plan;
        private AgentState state;
        private GoalState[][] freeTimeGoals;
        private List<ActionItem> actionQueue;
        private NavigationBase navigation;
        private AgentEnvironment environment;
        private SimulationProject project;
        protected SimulationTimer timer;
        protected bool authorised;

        public static double RandomInterval(double from, double to) {
            return random.Next((int)(from * 1000), (int)(to * 1000)) / 1000d;
        }

        // constructor

        protected SimulationAgent() {
            
            // this.name = name ?? (groups[0][0] + "_" + index++);
            
            this.PlanHistory = new List<PlanHistory>();
            this.Callbacks = new GovernorCallbacks(this);
            this.actionQueue = new List<ActionItem>();

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

        [JsonIgnore]
        public string CurrentGoalAction { get; set; }

        // public List<AgentProperty> Properties { get; private set; }

        [JsonIgnore]
        public List<AStarNode> Plan {
            get => this.plan;
            protected set {
                this.plan = value;
                this.OnPropertyChanged("Plan");
            }
        }

        [JsonIgnore]
        public List<PlanHistory> PlanHistory { get; private set; }

        [JsonIgnore]
        public GoalState[][] FreeTimeGoals {
            get {
                if (this.freeTimeGoals == null && this.FreeTimeGoalDefinition != null) {
                    this.freeTimeGoals = new GoalState[FreeTimeGoalDefinition.Length][];
                    for (var i = 0; i < FreeTimeGoalDefinition.Length; i++) {
                        this.FreeTimeGoals[i] = GoalState.ParseStringGoals(this.Governor, FreeTimeGoalDefinition[i]);
                    }
                }
                return this.freeTimeGoals;
            }
        }

        protected Governor Governor { get; set; }

        protected IGovernorCallbacks Callbacks { get; set; }

        [JsonIgnore]
        public AgentState State {
            get => this.state;
            set {
                this.state = value;
                this.OnPropertyChanged("State");
            }
        }

        #region Behaviours
        public void RunAfter(Action action, float waitTimeInSeconds)
        {
            this.actionQueue.Add(new ActionItem(Time.time + waitTimeInSeconds, action));
        }

        public void UpdateOnUi(Action action)
        {
            action();
        }

        public virtual void Start()
        {
            this.navigation = GetComponent<NavigationBase>();
            
            this.project = FindObjectOfType<SimulationProject>();
            this.timer = FindObjectOfType<SimulationTimer>();
            this.environment = FindObjectOfType<AgentEnvironment>();

        }

        public void Update()
        {
            if (!this.authorised || this.navigation.Navigating)
            {
                return;
            }

            // execute all actions in queue that can be executed at this time
            for (var i = actionQueue.Count - 1; i >= 0; i--)
            {
                var item = actionQueue[i];
                if (item.startTime <= Time.time)
                {
                    item.action();
                    actionQueue.RemoveAt(i);
                }

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

                this.RunAfter(() => Reason(), 0.1f);
            }
        }

        // agent methods

        protected abstract void Reason();

        private static int tid;
        protected void CreatePlan(Governor agent, GoalState[] goal, string goalType = null) {
            this.State = AgentState.Planning;

            // var t = new Thread(() => FindPlan(agent, goal, goalType));
            // t.Name = "Thread_" + tid++;
            // t.Start();

            FindPlan(agent, goal, goalType);
        }

        private void UpdatePlan() {

        }

        const bool useCache = true;

        private void FindPlan(Governor agent, GoalState[] goal, string goalType) {
            // TODO: Dangerous! Should not happen ...
            if (agent.Workflow == null) {
                return;
            }

            // make sure that agent is in the parent workflow
            while (agent.Workflow.Parent != null) {
                Log.Info("Exiting workflow due to planning ...");
                agent.ExitWorkflow();
            }

            // Debug.WriteLine(string.Format("Agent {0} in {1} plans for {2}", agent.Name, agent.Position, string.Join(", ", goal.Select(g => g.Name + ": " + g.Value).ToArray())));
            // try to locate plan in the memory
            var groupString = string.Join(",", agent.Groups.Select(g => g.Organisation.Id + "|" + g.Role.Id).ToArray());

            if (useCache) {
                var storedPlan = PlanMemory.FindPlan(agent, groupString, agent.Position, goal);

                // Debug.WriteLine("######: " + storedPlan);
                // Debug.WriteLine(PlanMemory.Plans.Count);

                if (storedPlan != null) {
                    // Debug.WriteLine("REUSING stored plan!");

                    // notify ui about the new plan
                    this.UpdateOnUi(() => {
                        //this.PlanHistory.Insert(0, new PlanHistory {
                        //    StartString = project.SimulatedTimeString,
                        //    Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                        //    Result = "Stored",
                        //    GoalType = goalType
                        //});
                        this.PlanHistory = new List<PlanHistory>() {
                            new PlanHistory {
                                StartString = this.timer.SimulatedTimeString,
                                Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                                Result = "Stored",
                                GoalType = goalType
                            }
                        };

                        this.OnPropertyChanged("PlanHistory");
                    });

                    // create new plan and add necessary cost data with items as close as possible to each other

                    this.State = AgentState.ExecutingPlan;
                    // clone found plan TODO: possibly remove when will recreate plans to each individual agent
                    string action = null;
                    this.Plan = storedPlan.Select(s => {
                        var node = new AStarNode(s.Arc);
                        node.CostData =
                            s.Arc == null || s.Arc.Action == null || s.Arc.Action.Id == null ?
                                null :
                                TravelCostManager.FindClosestAction(this.environment, this.Governor, (int)this.transform.X, (int)this.transform.Y, s.Arc.Action.Id, action).objectId;
                        action = node.CostData;
                        return node;
                    }).ToList();
                    this.ContinuePlan(agent, false);

                    Log.Debug(this.gameObject.name, " ##### REUSING #####");
                    return;
                }
            }

            // we only allow 1 planning thread
            if (planning) {
                Log.Debug(this.gameObject.name, "Waiting for free planner session ...");
                this.RunAfter(() => { this.Reason(); }, 1);
                return;
            }
            Log.Debug(this.gameObject.name, " $$$$$$$ PLANNING $$$$$$$");
            planning = true;

            Debug.WriteLine("Creating new plan");
            TotalPlans++;

            //            Debug.WriteLine("[{0}] Planning: {1}", agent.Name, agent.Properties.GetParameterValue("a.Pots"));

            Log.Debug(agent.Name, "Generating plan for: " + string.Join(";", goal.Select(w => w.ToString()).ToArray()));

            // add to list history
            this.UpdateOnUi(() => {
                // TODO: Show
                //this.PlanHistory.Insert(0, new PlanHistory {
                //    StartString = project.SimulatedTimeString,
                //    Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                //    Result = "Planning",
                //    GoalType = goalType
                //});

                this.PlanHistory = new List<PlanHistory> { new PlanHistory {
                    StartString = this.timer.SimulatedTimeString,
                    Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                    Result = "Planning",
                    GoalType = goalType
                } };

                if (this.PlanHistory.Count > 30) {
                    this.PlanHistory.RemoveAt(30);
                }

                this.OnPropertyChanged("PlanHistory");
            });

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<AStarNode> plan = null;
            try {
                plan = agent.PlanGoalState(goal,
                    PlanStrategy.ForwardSearch,
                    new TravelCostManager(agent, this.environment, (int) this.transform.X, (int)this.transform.Y));

                // remember the initial node
                this.PlanHistory[0].InitialNode = new List<AStarNode>() { agent.Planner.InitialNode };

                if (plan != null) {
                    Log.Info(agent.Name, string.Format("Plan with length {0} generated in {1} seconds", plan == null ? 0 : plan.Count, (sw.ElapsedMilliseconds / 1000)));
                    this.PlanHistory[0].Result = "InProgress";

                    //                    this.PlanHistory[0].Message = "\nGenerated Plan: " + 
                    //                        string.Join(" -> ", plan.Select(w => (w.Arc == null || w.Arc.Action == null) ? string.Empty : w.Arc.Action.Id).ToArray()) + "\n" +
                    //                        agent.Planner.Message.ToString();
                    this.PlanHistory[0].GeneratedPlan = plan;
                }


                //                Debug.WriteLine("[{0}] Plan generated: {1}", agent.Name, agent.Properties.GetParameterValue("a.Pots"));
            } catch (PlanException ex) {
                this.UpdateOnUi(() => {
                    this.PlanHistory[0].Result = "Exception";
                    this.PlanHistory[0].Message = agent.Planner.Message.ToString();
                    this.PlanHistory[0].Failed = true;
                    this.OnPropertyChanged("PlanHistory");
                });



                Log.Error(agent.Name, ex.Message);
                this.RandomWalk();
                return;
            } finally {
                sw.Stop();
                ResponseTimes += sw.ElapsedMilliseconds;
            }

            if (plan == null) {
                FailedPlans++;

                //this.View.UpdateOnUi(() => {
                try {
                    this.PlanHistory[0].Result = "Not Found";
                    this.PlanHistory[0].Message = agent.Planner.Message.ToString();
                    this.PlanHistory[0].Failed = true;
                    this.OnPropertyChanged("PlanHistory");
                } catch { }
                //});

                planning = false;
                Log.Debug(agent.Name, "Plan not found!");

                Thread.Sleep(2000);

                this.RandomWalk();
                this.State = AgentState.Idle;
            } else {
                this.State = AgentState.ExecutingPlan;
                this.Plan = new List<AStarNode>(plan);

                // storedPlan = PlanMemory.FindPlan(agent, groupString, agent.Position, goal);

                // Debug.WriteLine("Adding: " + goal[0].Name + " : " + goal[0].Value + " --- " + storedPlan);
                PlanMemory.AddPlan(groupString, agent.Position, goal, plan);
                planning = false;
                this.ContinuePlan(agent, false);
            }

            // free planning mananger

        }

        static object locker = new object();
        protected void ExecuteNextPlanStep(Governor agent) {
            lock (locker) {
                // rmove empty actions

                while (this.Plan.Count > 0 && (this.Plan[0].Arc == null || this.Plan[0].Arc.Action == null)) {
                    //this.Plan.RemoveAt(0);
                    this.UpdateOnUi(() => {
                        this.Plan.RemoveAt(0);
                        this.OnPropertyChanged("Plan");
                    });

                }

                // we may have finished the plan

                if (this.Plan.Count == 0) {
                    this.FinishPlan(agent);
                    return;
                }

                var planItem = this.Plan[0];

                // remove this plan item
                this.UpdateOnUi(() => {
                    this.Plan.RemoveAt(0);
                    this.OnPropertyChanged("Plan");
                });

                if (planItem.Arc.Action is ActionExitWorkflow) {
                    Log.Debug(agent.Name, "[WF] exit workflow");

                    try {
                        var result = agent.ExitWorkflow();
                        Log.Debug(agent.Name, "Workflow exit: " + result.Code);

                        // workflow exit immediatelly continues the plan
                        this.ContinuePlan(agent);
                    } catch (InstitutionException ex) {
                        this.FailPlan(agent, "Action failed: " + ex.Message);
                    }
                } else if (planItem.Arc.Action is ActionMessage ||
                      planItem.Arc.Action is ActionJoinWorkflow) {
                    Log.Debug(agent.Name, "[WF] Perform action");

                    // first perform action in the institution
                    // use the object related to the action

                    var itemId = planItem.CostData;

                    // use the object related to this action

                    if (string.IsNullOrEmpty(itemId)) {
                        if (this.PerformAction(agent, planItem)) {
                            this.ContinuePlan(agent);
                        }
                        return;
                    }

                    if (!string.IsNullOrEmpty(itemId)) {
                        Log.Debug(agent.Name, "[WF] Find related ...");

                        float waitTime = 0;
                        //EnvironmentDataAction action;

                        // if it is a simple action that does not generate any interactio simply wait a defined interwal
                        if (this.environment.NoLocationInfo(itemId) != null) {
                            if (this.PerformAction(agent, planItem)) {
                                waitTime = this.environment.NoLocationInfo(itemId).Duration;
                            }
                        } else {
                            // this is environmental action
                            EnvironmentData obj;

                            //lock (project.Environment.Objects)
                            {
                                // object may have eventually disappeared, so we need to replan

                                if (!this.environment.TryGetValue(itemId, out obj)) {
                                    this.FailPlan(agent, "Missing resource " + itemId);
                                    return;
                                }

                                //var action = obj.Definition.Actions.First(w => w.Id == planItem.Arc.Action.Id);

                                VariableInstance[] pars = null;
                                if (obj.Parameters != null) {
                                    obj.Parameters.TryGetValue(planItem.Arc.Action.Id, out pars);
                                }

                                // object exists, so execute the object in the institution and use this object
                                if (this.PerformAction(agent, planItem, pars)) {
                                    Log.Debug(agent.Name, "[WF] Using object and sleeping: " + waitTime);
                                    waitTime = this.environment.UseObject(obj, planItem.Arc.Action.Id);
                                }
                            }
                        }

                        if (waitTime > 0) {
                            waitTime = (float)this.timer.CalculateDuration(waitTime);

                            Log.Debug(agent.Name, "[WF] Continuing plan after: " + waitTime);
                            this.RunAfter(() => ContinuePlan(agent), waitTime);
                            return;
                        }
                    }
                    Log.Debug(agent.Name, "[WF] Continuing plan ...");
                    this.RunAfter(() => ContinuePlan(agent), 0.1f); // continue after tenth of a second

                } else {
                    throw new NotImplementedException("This is not implemented!");
                }
            }
        }

        private bool PerformAction(Governor agent, AStarNode planItem, params VariableInstance[] parameters) {
            try {
                var result = agent.PerformAction(planItem.Arc.Action.Id, parameters);
                Log.Info(agent.Name, "Action '" + planItem.Arc.Action.Id + "' perform: " + result.Code);
                if (result.IsOk) {
                    return true;
                }
                this.FailPlan(agent, "Action failed: " + result.Code);
                return false;
            } catch (InstitutionException ex) {
                this.FailPlan(agent, "Action failed: " + ex.Message);
                return false;
            }
        }

        private void ContinuePlan(Governor agent) {
            Log.Debug(agent.Name, "[WF] Continuing plan ...");

            this.ContinuePlan(agent, true);
        }

        protected void ContinuePlan(Governor agent, bool removeFirst) {
            //Debug.WriteLine("[{0}] Executing {1} ({2}): {3}", agent.Name, this.plan.Count, this.plan.Count > 0 ? this.plan[0].Arc.ToString() : "FINISH", agent.Properties.GetParameterValue("a.Pots"));

            // remove all unnecessary nodes
            while (this.Plan.Count > 0 && (this.Plan[0] == null || this.Plan[0].Arc == null || this.Plan[0].Arc.Action == null)) {
                var item = this.Plan[0];
                if (item.Arc.From != null && item.Arc.To != null && item.Arc.From.Id != item.Arc.To.Id) {
                    this.Governor.Move(item.Arc.To.Id);
                }
                //this.Plan.RemoveAt(0);
                this.UpdateOnUi(() => {
                    try {
                        this.Plan.RemoveAt(0);
                        this.OnPropertyChanged("Plan");
                    } catch { }
                });
            }


            if (this.Plan.Count > 0) {
                // go to next planned destination
                var itemId = this.Plan[0].CostData;


                // check whether it is an environmental object or object with no location
                // item id is null for example for workflow actions

                if (itemId == null || this.environment.NoLocationInfo(itemId) != null) {
                    Log.Debug(agent.Name, "[CP] No travel needed ...");

                    this.ExecuteNextPlanStep(agent);
                } else {
                    // travel to destination of the object
                    EnvironmentData obj;
                    if (this.environment.TryGetValue(itemId, out obj)) {
                        this.destinationX = obj.X;
                        this.destinationY = obj.Y;

                        Log.Debug(agent.Name, "[CP] Moving to destination ...");

                        this.MoveToDestination(this.destinationX, this.destinationY);
                    } else {
                        this.FailPlan(agent, "Missing resouce: " + itemId);
                    }
                }
            } else {
                this.FinishPlan(agent);
            }
        }

        private void FinishPlan(Governor agent) {
            this.UpdateOnUi(() => this.PlanHistory[0].Result = "Finished");

            Log.Debug(agent.Name, "[CP] Plan finished ...");

            this.State = AgentState.Idle;
            this.Reason();
        }

        private void FailPlan(Governor agent, string reason) {
            this.UpdateOnUi(() => {
                this.PlanHistory[0].Result = "Failed";
                this.PlanHistory[0].Failed = true;
            });

            // object is no longer there, so fail the plan!
            Log.Error(agent.Name, "Plan failed!: " + reason);

            // TODO: Dangerous, should not happen, or remove agent when it happens
            if (agent.Workflow == null) {
                return;
            }

            // return to the top workflow
            while (agent.Workflow.Parent != null) {
                agent.ExitWorkflow();
            }

            this.Plan.Clear();

            this.State = AgentState.Idle;
            this.RandomWalk();
        }

        protected void RandomWalk() {
            this.MoveToDestination(this.environment.RandomX, this.environment.RandomY);
        }

        protected virtual void MoveToDestination(float x, float y) {
            if (Math.Abs(this.transform.X - x) < 0.00001 && Math.Abs(this.transform.Y - y) < 0.00001) {
                this.MovedToLocation();
                return;
            }

            this.destinationX = x;
            this.destinationY = y;

            this.navigation.MoveToDestination(this.destinationX, this.destinationY);
        }


        // callback methods

        public virtual void MovedToLocation() {
            this.Reason();
        }



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
