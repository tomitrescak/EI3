using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Ei.Logs;
using Ei.Ontology.Actions;
using Ei.Runtime.Planning;
using Ei.Runtime.Planning.Costs;
using Ei.Runtime.Planning.Heuristics;
using Ei.Runtime.Planning.Strategies;
using ActionBase = Ei.Ontology.Actions.ActionBase;

namespace Ei.Runtime
{
    using Ei.Ontology;

    public class Governor : IGovernor, IStateProvider
    {
        #region struct Context
        public struct Context
        {
            public Connection Connection { get; }
            public Workflow.Instance Workflow { get; }

            public Context(Connection connection, Workflow.Instance wf) {
                this.Connection = connection;
                this.Workflow = wf;
            }
        }
        #endregion

        public class ResourceState
        {
            public struct Group
            {
                public Runtime.ResourceState Organisation;
                public Runtime.ResourceState Role;
            } 
            private List<Group> groups;
            public int CreatedInstanceId;
            public string Name;
            public Governor Governor;       

            public List<Group> Roles { get { return this.groups; } }

            public ResourceState(Governor agent) {
                this.Name = agent.Name;
                this.Governor = agent;

                this.groups = new List<Group>();

                var addedRoles = new Dictionary<Role, Runtime.ResourceState>();

                foreach (var group in agent.Groups) {
                    if (!addedRoles.ContainsKey(group.Role)) {
                        addedRoles.Add(group.Role, group.Role.CreateState());
                    }
                    this.groups.Add(new Group { Organisation = group.Organisation.CreateState(), Role = addedRoles[group.Role] });
                }
            }

            public Runtime.ResourceState FindProvider(string name) {
                if (this.groups.Count == 1) {
                    return this.groups[0].Role;
                }
                return this.groups.Find(f => f.Role.Descriptors.Any(r => r.Name == name)).Role;
            }

            public GoalState[] ToGoalState(bool onlyDirty = false) {
                if (this.groups.Count == 1) {
                    return this.groups[0].Role.ToGoalState(onlyDirty);
                }
                return this.groups.SelectMany(s => s.Role.ToGoalState(onlyDirty)).ToArray();
            }

            public void ResetDirty() {
                foreach (var item in this.groups) {
                    item.Role.ResetDirty();
                }
            }

            public Runtime.ResourceState Clone(Runtime.ResourceState state) {
                throw new NotImplementedException();
                // return this.Clone(state as GovernorVariableState);
            }

            public ResourceState Clone(ResourceState clone = null) {
                throw new NotImplementedException();
                //if (clone == null) { clone = new GovernorVariableState(this.Governor); }
                //clone.CreatedInstanceId = this.CreatedInstanceId;
                //return clone;
            }
        }

        #region Fields
        private WorkflowPosition position;
        private IGovernorCallbacks callbacks;
        private Context currentContext;
        private Stack<Context> contextStack;
        #endregion

        #region Properties
        public ResourceState Resources { get; private set; }

        public string Name { get; set; }
        public Group[] Groups { get; private set; }
        public Workflow.Instance Workflow { get { return this.currentContext.Workflow; } }

        public int CloneCount { get; private set; }
        public List<Governor> Clones { get; private set; }

        // clone related properties
        public Governor Parent { get; private set; }

        public bool ShallowClone { get; private set; }

        public Planner Planner { get; private set; }

        public WorkflowPosition Position {
            get {
                if (currentContext.Workflow == null) {
                    return null;
                }
                return this.currentContext.Workflow.Stateless ? this.position : this.currentContext.Workflow.State;
            }
            set {
                this.position = value;
                //                Console.WriteLine("[{0}] flowed to {1}.{2}",
                //                    this.Name,
                //                    this.Workflow != null ? this.Workflow.Id : "{}",
                //                    this.Position != null ? this.Position.Id : "{}");
            }
        }

        #endregion


        public InstitutionManager Manager { get; private set; }

        public Governor() {
        }

        private Governor(Governor governor, int clones, string suffix, bool shallowClone) : this() {
            this.Name = governor.Name + suffix;
            this.Groups = governor.Groups;
            this.Parent = governor;
            this.CloneCount = clones;
            this.ShallowClone = shallowClone;
            this.Manager = governor.Manager;

            // position will be changed by the split 
            this.position = null;

            // callbacks will be received to the same origin
            this.callbacks = governor.callbacks;

            // clone the stack
            this.contextStack = new Stack<Context>(governor.contextStack.Reverse());
            this.currentContext = governor.currentContext;

            // clone properties
            if (shallowClone) {
                this.Resources = governor.Resources;
            }
            else {
                this.Resources = governor.Resources.Clone();

            }
        }

        public Governor Clone(int clones, string suffix, bool shallowClone) {
            var clone = new Governor(this, clones, suffix, shallowClone);
            if (this.Clones == null) {
                this.Clones = new List<Governor>();
            }
            this.Clones.Add(clone);
            return clone;
        }

        internal void Init(
            IGovernorCallbacks callbacks,
            string name,
            Group[] roles,
            InstitutionManager manager) {
            this.callbacks = callbacks;
            this.Name = name;
            this.Groups = roles;
            this.Manager = manager;

            this.contextStack = new Stack<Context>();
            this.Resources = new ResourceState(this);
        }

        // runtime

        public void Start() {
            this.Resources = new ResourceState(this);
            this.EnterInstitution();
            this.EnterWorkflow(null, this.Manager.MainWorkflow);
        }

        // public methods

        public void Continue(string cloneName = null) {
            if (string.IsNullOrEmpty(cloneName)) {
                this.Continue();
            }

            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            if (result.IsOk) {
                clone.Continue();
            }
        }

        public IActionInfo ExitWorkflow(string cloneName) {
            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            return result.IsOk ? clone.ExitWorkflow() : result;
        }

        public IActionInfo ExitWorkflow() {
            if (this.Workflow.Parent == null && this.Parent != null) {
                throw new InstitutionException("Clone cannot leave the institution");
            }

            if (!this.Position.CanExit(this)) {
                return ActionInfo.Failed;
            }

            // exit from workflow
            this.Workflow.Exit(this);

            // change context to parent
            var connection = this.currentContext.Connection;
            var oldWorkflow = this.Workflow;

            this.currentContext = this.contextStack.Count > 0 ? this.contextStack.Pop() : new Context();
            var newWorkflow = currentContext.Workflow == null ? null : this.currentContext.Workflow;

            this.LogAction(
                InstitutionCodes.ExitedWorkflow,
                this.Name,
                oldWorkflow.Id,
                oldWorkflow.Name,
                newWorkflow != null ? newWorkflow.Id : null,
                newWorkflow != null ? newWorkflow.Name : null);

            this.callbacks.ExitedWorkflow(
                oldWorkflow.Id,
                oldWorkflow.Name,
                newWorkflow != null ? newWorkflow.Id : null,
                newWorkflow != null ? newWorkflow.Name : null);

            // flow to output node of the context workflow node
            if (connection != null) {
                connection.To.EnterAgent(this);
            }

            // if agent exists from the main workflow it exists also from the institution

            if (this.Workflow == null) {
                this.ExitInstitution();
            }

            return ActionInfo.Ok;
        }

        public IActionInfo Move(string cloneName, string positionId) {
            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            return result.IsOk ? clone.Move(positionId) : result;
        }

        public IActionInfo Move(string positionId, Runtime.ResourceState parameters = null) {
            var target = this.FindPosition(positionId);
            if (target.Length == 0) {
                return ActionInfo.StateNotReachable;
            }

            // perform move
            var connection = target[0];
            if (connection.Action != null) {
                return new ActionInfo(InstitutionCodes.InvalidMove);
            }

            // pass this arc
            return connection.Pass(this, parameters);
        }

        public IActionInfo PerformAction(string cloneName, string actionId, VariableInstance[] parameters) {
            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            return result.IsOk ? clone.PerformAction(actionId, parameters) : result;
        }

        //        public IActionInfo PerformAction(string activityId, params ParameterInstance[] parameters)
        //        {
        //        }

        public IActionInfo PerformAction(string activityId, VariableInstance[] parameters = null) {
            // find actions
            var action = this.Workflow.Actions.FirstOrDefault(w => w.Id == activityId);
            if (action == null) {
                return new ActionInfo(InstitutionCodes.Failed, "Action does not exist!");
            }

            // otherwise we have to find a feasible connection
            // check if Action can be performed
            var connections = this.FindConnection(activityId);
            if (connections.Length == 0) {
                return ActionInfo.FailedPreconditions;
            }

            if (Log.IsInfo) Log.Info(this.Name, "Performing " + activityId);

            // parse parameters
            var parsedParameters = action.ParseParameters(parameters);

            // perform this Action
            var result = connections[0].Pass(this, parsedParameters);

            if (!result.IsAcceptable) {
                this.LogAction(
                    InstitutionCodes.ActivityFailed,
                    this.Name,
                    this.Name,
                    activityId,
                    parameters == null ? null : parameters.ToString());

                this.NotifyActivityFailed(this.Workflow, this.Name, activityId, parsedParameters);
            }
            else {
                this.NotifyActivity(this.Workflow, this.Name, activityId, parsedParameters);
            }
            return result;
        }

        public IActionInfo PerformAction(ActionBase action, Runtime.ResourceState parameters = null) {


            // otherwise we have to find a feasible connection
            // check if Action can be performed
            var activityId = action.Id;
            var connections = this.FindConnection(activityId);
            if (connections.Length == 0) {
                return ActionInfo.FailedPreconditions;
            }

            if (Log.IsInfo) Log.Info(this.Name, "Performing " + activityId);

            // perform this Action
            var result = connections[0].Pass(this, parameters);

            if (!result.IsAcceptable) {
                this.LogAction(
                    InstitutionCodes.ActivityFailed,
                    this.Name,
                    this.Name,
                    activityId,
                    parameters == null ? null : parameters.ToString());

                this.NotifyActivityFailed(this.Workflow, this.Name, activityId, parameters);
            }
            else {
                this.NotifyActivity(this.Workflow, this.Name, activityId, parameters);
            }
            return result;
        }

        public WorkflowInfo[] GetWorkflowInfos(string cloneName, string activityId) {
            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            return result.IsOk ? clone.GetWorkflowInfos(activityId) : new WorkflowInfo[0];
        }

        public WorkflowInfo[] GetWorkflowInfos(string activityId) {

            var connections = this.FindConnection(activityId);
            if (connections.Length == 0) {
                // return empty set
                return new WorkflowInfo[0];
            }

            // check type
            var activity = connections[0].Action as ActionJoinWorkflow;
            if (activity == null) {
                // this isnot a workflow Action
                return new WorkflowInfo[0];
            }

            return activity.GetWorkflows(this);
        }

        public ActionBase[] FeasibleActions(string cloneName) {
            Governor clone;
            var result = this.FindClone(cloneName, out clone);
            return result.IsOk ? clone.FeasibleActions() : new ActionBase[0];
        }

        public ActionBase[] FeasibleActions() {
            // check for open activities
            return this.Position.ViableConnections(this).Select(w => w.Action).ToArray();
        }

        // internal methods

        internal void EnterWorkflow(Connection connection, Workflow.Instance workflow) {
            // change context
            if (this.currentContext.Workflow != null) {
                this.contextStack.Push(this.currentContext);
            }
            this.currentContext = new Context(connection, workflow);

            // change to workflow state
            // if workflow is stateless we set the current state to the start state
            // if workflow is not stateless we change to current state
            if (workflow.Join(this)) {
                // set properties of the current workflow
                // this.VariableState.WorkflowVariableState = workflow.VariableState;
            }
            else {
                this.LogAction(
                    InstitutionCodes.WorkflowEntryFailed,
                    this.Name,
                    workflow.Id,
                    workflow.Name);
                this.contextStack.Pop();
            }
        }

        internal bool IsInGroup(IEnumerable<Group> notifyRoles) {
            return this.Groups.Any(w => Access.IsInGroup(w, notifyRoles));
        }

        internal void Join(List<Governor> inagents) {
            // clear clones
            this.Clones.Clear();

            // consolidate parameters if it is a deep clone
            if (!inagents[0].ShallowClone) {
                // TODO: Resolve consolidation
                //                foreach (var key in this.Properties["a"].Keys)
                //                {
                //                    var param = this.Properties["a"].GetParameter(key);
                //                    this.Properties["a"].SetParameter(
                //                        param.Consolidate(inagents.Select(w => w.Properties["a"].GetParameter(key))));
                //                }
            }
        }

        // private methods

        private void EnterInstitution() {
            var ei = this.Manager.Ei;

            // notify agent
            this.callbacks.EnteredInstitution(ei.Id, ei.Name);

            // notify listeners
            this.LogAction(InstitutionCodes.EnteredInstitution, this.Name, this.Manager.Ei.Id, this.Manager.Ei.Name);
        }

        private void ExitInstitution() {
            if (this.Parent != null) {
                throw new InstitutionException("Clone cannot leave the institution");
            }

            var ei = this.Manager.Ei;

            // remove agent
            Manager.RemoveGovernor(this);

            // notify agent
            this.callbacks.ExitedInstitution(ei.Id, ei.Name);

            // notify listeners
            this.LogAction(InstitutionCodes.ExitedInstitution, this.Name, this.Manager.Ei.Id, this.Manager.Ei.Name);

            // nullify 
            this.position = null;
            this.currentContext = new Context();
        }

        private Connection[] FindPosition(string positionId) {
            return this.Position.ViableConnections(this).Where(w => w.To.Id == positionId).ToArray();
        }

        private Connection[] FindConnection(string activityId) {
            var connections = this.Position.ViableConnections(this, activityId);
            if (connections.Length > 1) {
                throw new ApplicationException("Ambiguous actions, more than one action is available: " + activityId);
            }
            return connections;
        }

        public void LogAction(InstitutionCodes code, params string[] parameters) {
            if (Log.IsInfo) Log.Info(this.Name, code, parameters);
        }

        private IActionInfo FindClone(string cloneName, out Governor clone) {
            clone = null;
            if (this.Clones == null || this.Clones.Count == 0) {
                return ActionInfo.AgentNotCloned;
            }
            if (this.Clones.All(w => w.Name != cloneName)) {
                return new ActionInfo(InstitutionCodes.CloneDoesNotExist, cloneName);
            }
            clone = this.Clones.Find(w => w.Name == cloneName);
            return ActionInfo.Ok;
        }

        //public void ApplyPostconditions(AccessCondition[] postconditions) {

        //    if (postconditions != null && postconditions.Length > 0) {
        //        foreach (var postcondition in postconditions) {
        //            //if (postcondition.AppliesTo(this.Groups)) {
        //                postcondition.ApplyPostconditions(this.VariableState, false);
        //            //}
        //        }
        //    }
        //}

        public IActionInfo Continue() {
            // we may have exited the institution
            if (this.Position == null) {
                return ActionInfo.Ok;
            }

            // filter output arcs
            var viableArcs = this.Position.ViableTransitions(this.Groups, this.Resources);

            if (viableArcs.Length == 0) {
                return ActionInfo.Ok;
            }

            // we flow only through transitions,if there are more than on possibility, throw error
            if (viableArcs.Length > 1) {
                throw new ApplicationException("Ambiguous connection! More than one possible transition from: " + this.Position.Id);
            }

            var arc = viableArcs[0];

            var result = arc.Pass(this);
            if (result.IsNotOk) {
                throw new ApplicationException("Transitioning arc failed! " + arc);
            }

            // enter transition or state
            return result;
        }

        // notifications

        internal void NotifyWaitForDecision() {
            this.callbacks.WaitingForDecision();
            this.LogAction(InstitutionCodes.WaitingForDecision, this.Name, this.Workflow.Id, this.Position.Id);
        }

        internal void NotifyEnteredWorkflow(Workflow.Instance workflow) {
            this.LogAction(
                    InstitutionCodes.EnteredWorkflow,
                    this.Name,
                    workflow.Id,
                    workflow.Name);

            this.callbacks.EnteredWorkflow(this.Name, workflow.Id, workflow.Name);
        }

        internal void NotifyPortChange() {
            this.callbacks.ChangedPosition(this.Name, this.Workflow.Id, this.Workflow.InstanceId, this.Position.Id);
        }

        public void NotifyParameterChanged(string parameterName, object parameterValue) {
            this.NotifyAgentParameterChanged(parameterName, parameterValue);
        }

        internal void NotifyAgentParameterChanged(string parameterName, object parameterValue) {
            this.callbacks.NotifyAgentParameterChanged(this.Name, parameterName, parameterValue);
        }

        internal void NotifyWorkflowParameterChanged(string workflowId, int workflowInstanceId, string parameterName, object value) {
            this.callbacks.NotifyWorkflowParameterChanged(this.Name, workflowId, workflowInstanceId, parameterName, value);
        }

        internal void NotifyActivity(Workflow.Instance workflow, string agentName, string activityId, Runtime.ResourceState parameters) {
            this.callbacks.NotifyActivity(this.Name, workflow.Id, workflow.InstanceId, agentName, activityId, parameters);
            //Console.WriteLine("[CB NotifyActivity] {0},{1},{2},{3},{4}", workflow.Id, workflow.InstanceId, agentName, actionId, string.Join(";", parameters.Select(i => i.ToString()).ToArray()));
        }

        internal void NotifyActivityFailed(Workflow.Instance workflow, string agentName, string activityId, Runtime.ResourceState parameters) {
            this.callbacks.NotifyActivityFailed(this.Name, workflow.Id, workflow.InstanceId, agentName, activityId, parameters);
            //Console.WriteLine("[CB NotifyActivity] {0},{1},{2},{3},{4}", workflow.Id, workflow.InstanceId, agentName, actionId, string.Join(";", parameters.Select(i => i.ToString()).ToArray()));
        }

        internal void NotifySplit(Governor[] clones, bool shallowClone) {
            this.LogAction(InstitutionCodes.Split, this.Name, clones.Length.ToString(), shallowClone.ToString());
            this.callbacks.Split(clones, shallowClone);
        }

        internal void NotifyJoin() {
            this.LogAction(InstitutionCodes.Join, this.Name);
            this.callbacks.Joined();
        }

        public bool Is(Governor agent) {
            if (agent == this) {
                return true;
            }
            return this.Parent != null && this.Parent.Is(agent);
        }

        // static helpers

        public struct GoalDescription
        {
            public Connection Connection;
            public float Ratio;

            public GoalDescription(Connection connection, float ratio = 1) {
                this.Connection = connection;
                this.Ratio = ratio;
            }
        }

        public static List<GoalDescription> FindGoals(Workflow workflow, Governor.ResourceState agentState, Group[] groups, GoalState[] goalState, int maxGoals = int.MaxValue) {
            var result = new List<GoalDescription>();

            // browse all connections and 
            // if one of them positively changes current state to the desired state
            // we assume that this is the connection we want to consider
            foreach (var conn in workflow.Connections) {
                var state = agentState.Clone();

                // apply postconditions
                conn.ApplyPostconditions(state, null, true);

                // check 100% fulfilling goals

                //                if (goalState.All(goal => goal.IsValid(state.GetParameter(goal.Name))))
                //                {
                //                    result.Add(new GoalDescription(conn));
                //                    continue;
                //                }

                // check approximating goals

                var maxRatio = CalculateRatio(goalState, agentState, state);
                if (maxRatio >= 1) {
                    result.Add(new GoalDescription(conn, maxRatio));
                }

                // connection can contain EFFECTS (which are all postconditions from the contained workflow)
                if (conn.GeneratedNestedEffects != null) {
                    // test each effect separately

                    foreach (var effect in conn.GeneratedNestedEffects) {
                        state = agentState.Clone();
                        effect.ApplyPostconditions(state, null, true);

                        maxRatio = CalculateRatio(goalState, agentState, state);
                        if (maxRatio >= 1) {
                            result.Add(new GoalDescription(conn, maxRatio));
                        }

                        //                        if (goalState.All(goal => goal.IsValid(state.GetParameter(goal.Name))))
                        //                        {
                        //                            result.Add(new GoalDescription(conn));
                        //                            break;
                        //                        }
                    }
                }

                // we can limit how many goals we want to return

                if (result.Count == maxGoals) {
                    break;
                }
            }

            if (result.Count == 0) {
                throw new Exception(string.Format("It is not possible to satisfy goal '{0}' in the current workflow", string.Join("; ", goalState.Select(w => w.ToString()).ToArray())));
            }

            return result;
        }

        private static float CalculateRatio(GoalState[] goals, Governor.ResourceState startState, Governor.ResourceState changedState) {
            var maxRatio = 0f;
            foreach (var goal in goals) {
                var ratio = goal.GetDeltaRatio(startState, changedState);
                if (ratio == -1) {
                    return -1;
                }
                if (ratio > maxRatio) {
                    maxRatio = ratio;
                }
            }
            return maxRatio;
        }

        public List<AStarNode> PlanAction(string actionName, PlanStrategy strategy, ICostManager costManager = null) {
            if (costManager == null) {
                costManager = UnitCostManager.Instance;
            }

            var planner = new Planner(this);
            var h = new StaticHeuristics(actionName);

            this.Planner = planner;

            var state = this.Resources.Clone();

            switch (strategy) {
                case PlanStrategy.ForwardSearch:
                    return planner.Plan(h, new ForwardSearch(this.Position, state, this.Groups), costManager);
                //case PlanStrategy.BackwardSearch:
                //    var startConnection = this.Workflow.Connections.First(w => w.Action != null && w.Action.Id == actionName);
                //    var startState = state.ToGoalState();
                //    var bh = new ResourceBasedHeuristics(startState);

                //    // TODO: Use floyd-warshall from workflow to make sure that action is accessible

                //    return planner.Plan(bh, new BackwardSearch(this.Workflow, state, this.Groups, this.Position, startConnection, startState), costManager);
            }

            throw new NotImplementedException("Strategy not implemented: " + strategy);
        }

        public List<AStarNode> PlanGoalState(GoalState[] goals, PlanStrategy strategy, ICostManager costManager = null) {
            if (costManager == null) {
                costManager = UnitCostManager.Instance;
            }

            var planner = new Planner(this);
            var state = this.Resources.Clone();

            this.Planner = planner;

            if (Log.IsDebug) Log.Debug("========= PLANNING: " + string.Join(";", goals.Select(w => w.Name + ":" + w.Value.ToString()).ToArray()));

            switch (strategy) {
                case PlanStrategy.ForwardSearch:
                    var s = new ForwardSearch(this.Position, state, this.Groups);
                    var h = new ResourceBasedHeuristics(goals);
                    return planner.Plan(h, s, costManager);
                //case PlanStrategy.BackwardSearch:
                //    var goal = Governor.FindGoals(this.Workflow.Workflow, state, Groups, goals, 1);

                //    var bs = new BackwardSearch(this.Workflow, state, this.Groups, this.Position, goal[0].Connection, goals);
                //    var bh = new ResourceBasedHeuristics(state.ToGoalState());
                //    return planner.Plan(bh, bs, costManager);
            }

            throw new NotImplementedException("Strategy not implmented: " + strategy);

        }
    }

    public class InstitutionException : Exception
    {
        public InstitutionException(string message) : base(message) {
        }
    }
}
