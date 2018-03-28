using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using Ei.Logs;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Planning.Memory;
using Ei.Runtime;
using Ei.Runtime.Planning;
using Ei.Runtime.Planning.Costs;
using Ei.Runtime.Planning.Environment;
using Ei.Runtime.Planning.Strategies;

namespace Ei.Simulator.Core
{
    public enum AgentState
    {
        Idle,
        Resting,
        Planning,
        ExecutingPlan
    }

    public abstract class SimulationAgent : INotifyPropertyChanged
    {
        public static int TotalPlans;
        public static int FailedPlans;
        public static long ResponseTimes;

        public string CurrentGoalAction { get; set; }

        protected static readonly Random random = new Random();
        private static int index;

        private float destinationX;
        private float destinationY;

        private readonly double speedPxPerSecond;
        private List<AStarNode> plan;

        private string[] freeTimeGoalDefinition;
        private GoalState[][] freeTimeGoals;
        private AgentState state;
        private string name;
        private byte[] color;

        private static bool planning = false;

        public static double RandomInterval(double from, double to) {
            return random.Next((int)(from * 1000), (int)(to * 1000)) / 1000d;
        }

        // constructor

        protected SimulationAgent(string role, string[] freeTimeGoals = null, string name = null) {
            this.freeTimeGoalDefinition = freeTimeGoals;
            this.X = Project.Current.Environment.RandomX;
            this.Y = Project.Current.Environment.RandomY;
            this.name = name == null ? (role + "_" + index++) : name;
            this.PlanHistory = new List<PlanHistory>();

            //            var timer = new Timer();
            //            timer.Interval = 50;
            //            timer.Elapsed += TimerOnElapsed;
            //            timer.AutoReset = true;
            //            timer.Start();


            // speed modifier calculates how many pixels agent can pass in one second
            // that is the speed expressed as S = px/sec
            // as a result we can calculate, that if agent needs to pass N pixels it will take agent N / S seconds

            this.speedPxPerSecond = Project.Current.AgentSpeed * // base speed (e.g. 5 km/h = 1.47 m/s)
                (86440 / Project.Current.DayLengthInSeconds) * // how many real seconds in one simulated second
                RandomInterval(Project.Current.SpeedDiversity[0], Project.Current.SpeedDiversity[1]);
            this.speedPxPerSecond = this.speedPxPerSecond / Project.Current.MetersPerPixel;

            this.Properties = new List<AgentProperty>();
        }

        // properties

        public string Name {
            get { return this.name; }
            set {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public IAgentView View { get; set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public byte[] Color {
            get { return this.color; }
            set {
                this.color = value;
                this.OnPropertyChanged("Color");
            }
        }

        public List<AgentProperty> Properties { get; private set; }

        public List<AStarNode> Plan {
            get { return this.plan; }
            protected set {
                this.plan = value;
                this.OnPropertyChanged("Plan");
            }
        }

        public List<PlanHistory> PlanHistory { get; private set; }

        public GoalState[][] FreeTimeGoals {
            get {
                if (this.freeTimeGoals == null && this.freeTimeGoalDefinition != null) {
                    this.freeTimeGoals = new GoalState[freeTimeGoalDefinition.Length][];
                    for (var i = 0; i < freeTimeGoalDefinition.Length; i++) {
                        this.FreeTimeGoals[i] = GoalState.ParseStringGoals(this.Governor, freeTimeGoalDefinition[i]);
                    }
                }
                return this.freeTimeGoals;
            }
        }

        protected Governor Governor { get; set; }

        protected PhysiologyAgentCallbacks Callbacks { get; set; }

        public AgentState State {
            get { return this.state; }
            set {
                this.state = value;
                this.OnPropertyChanged("State");
            }
        }

        // virtual methods

        protected virtual bool Connected() {
            return true;
        }

        // methods

        public void Connect(InstitutionManager manager, string organisation, string password, string[][] groups) {
            Governor governor;
            var result = manager.Connect(this.Callbacks, organisation, this.Name, password, groups, out governor);
            this.Governor = governor;

            if (result != InstitutionCodes.Ok) {
                Log.Error(this.Name, result.ToString());
                return;
            }

            // initialise agent

            if (this.Connected()) {

                // announce all parameters
                foreach (var group in this.Governor.Resources.Groups) {
                    var role = group.Role;
                    var pars = role.FilterByAccess(this.Governor);
                    foreach (var param in pars) {
                        this.Callbacks.NotifyAgentParameterChanged(this.Governor.Name, param.Name, param.Value);
                    }
                }

                this.View.RunAfter(() => Reason(), 0.1f);
            }
        }

        // agent methods

        protected abstract void Reason();

        private static int tid;
        protected void CreatePlan(Governor agent, GoalState[] goal, string goalType = null) {
            this.State = AgentState.Planning;

            var t = new Thread(() => FindPlan(agent, goal, goalType));
            t.Name = "Thread_" + tid++;
            t.Start();

            // FindPlan(agent, goal, goalType);
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
                    this.View.UpdateOnUi(() => {
                        //this.PlanHistory.Insert(0, new PlanHistory {
                        //    StartString = Project.Current.SimulatedTimeString,
                        //    Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                        //    Result = "Stored",
                        //    GoalType = goalType
                        //});
                        this.PlanHistory = new List<Core.PlanHistory>() {
                            new PlanHistory {
                                StartString = Project.Current.SimulatedTimeString,
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
                                TravelCostManager.FindClosestAction(Project.Current.Environment, this.Governor, (int)this.X, (int)this.Y, s.Arc.Action.Id, action).objectId;
                        action = node.CostData;
                        return node;
                    }).ToList();
                    this.ContinuePlan(agent, false);

                    Log.Debug(this.Name, " ##### REUSING #####");
                    return;
                }
            }

            // we only allow 1 planning thread
            if (planning) {
                Log.Debug(this.Name, "Waiting for free planner session ...");
                this.View.RunAfter(() => { this.Reason(); }, 1);
                return;
            }
            Log.Debug(this.Name, " $$$$$$$ PLANNING $$$$$$$");
            planning = true;

            Debug.WriteLine("Creating new plan");
            TotalPlans++;

            //            Debug.WriteLine("[{0}] Planning: {1}", agent.Name, agent.Properties.GetParameterValue("a.Pots"));

            Log.Debug(agent.Name, "Generating plan for: " + string.Join(";", goal.Select(w => w.ToString()).ToArray()));

            // add to list history
            this.View.UpdateOnUi(() => {
                // TODO: Show
                //this.PlanHistory.Insert(0, new PlanHistory {
                //    StartString = Project.Current.SimulatedTimeString,
                //    Goals = string.Join(";", goal.Select(w => w.ToString()).ToArray()),
                //    Result = "Planning",
                //    GoalType = goalType
                //});

                this.PlanHistory = new List<Core.PlanHistory> { new PlanHistory {
                    StartString = Project.Current.SimulatedTimeString,
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
                    new TravelCostManager(agent, Project.Current.Environment, (int)this.X, (int)this.Y));

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
                this.View.UpdateOnUi(() => {
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
                    this.View.UpdateOnUi(() => {
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
                this.View.UpdateOnUi(() => {
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
                        if (Project.Current.Environment.NoLocationInfo(itemId) != null) {
                            if (this.PerformAction(agent, planItem)) {
                                waitTime = Project.Current.Environment.NoLocationInfo(itemId).Duration;
                            }
                        } else {
                            // this is environmental action
                            EnvironmentData obj;

                            //lock (Project.Current.Environment.Objects)
                            {
                                // object may have eventually disappeared, so we need to replan

                                if (!Project.Current.Environment.TryGetValue(itemId, out obj)) {
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
                                    waitTime = Project.Current.Environment.UseObject(obj, planItem.Arc.Action.Id);
                                }
                            }
                        }

                        if (waitTime > 0) {
                            waitTime = (float)Project.Current.CalculateDuration(waitTime);

                            Log.Debug(agent.Name, "[WF] Continuing plan after: " + waitTime);
                            this.View.RunAfter(() => ContinuePlan(agent), waitTime);
                            return;
                        }
                    }
                    Log.Debug(agent.Name, "[WF] Continuing plan ...");
                    this.View.RunAfter(() => ContinuePlan(agent), 0.1f); // continue after tenth of a second

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
                this.View.UpdateOnUi(() => {
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

                if (itemId == null || Project.Current.Environment.NoLocationInfo(itemId) != null) {
                    Log.Debug(agent.Name, "[CP] No travel needed ...");

                    this.ExecuteNextPlanStep(agent);
                } else {
                    // travel to destination of the object
                    EnvironmentData obj;
                    if (Project.Current.Environment.TryGetValue(itemId, out obj)) {
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
            this.View.UpdateOnUi(() => this.PlanHistory[0].Result = "Finished");

            Log.Debug(agent.Name, "[CP] Plan finished ...");

            this.State = AgentState.Idle;
            this.Reason();
        }

        private void FailPlan(Governor agent, string reason) {
            this.View.UpdateOnUi(() => {
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
            this.MoveToDestination(Project.Current.Environment.RandomX, Project.Current.Environment.RandomY);
        }

        protected virtual void MoveToDestination(float x, float y) {
            if (Math.Abs(this.X - x) < 0.00001 && Math.Abs(this.Y - y) < 0.00001) {
                this.MovedToLocation();
                return;
            }

            this.destinationX = x;
            this.destinationY = y;

            this.View.MoveToLocation(this.destinationX, this.destinationY, this.speedPxPerSecond);
        }


        // callback methods

        public virtual void MovedToLocation() {
            this.X = this.destinationX;
            this.Y = this.destinationY;

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
