using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ei.Core.Runtime;
using Ei.Logs;
using Ei.Core.Ontology;
using Ei.Core.Ontology.Actions;
using Ei.Core.Ontology.Transitions;

namespace Ei.Core.Ontology
{
    using ActionBase = ActionBase;

    public abstract class Workflow : Entity {

        #region class WorkflowState
        public class Store : ResourceState {

            // private Workflow workflow;

            // properties

            public int AgentCount { get; set; }
            public Governor.GovernorState Last { get; set; }
            public Governor.GovernorState Owner { get; set; }

            // abstract implementation

            public override List<VariableInstance> FilterByAccess(Governor governor) {
                return new List<VariableInstance> {
                    new VariableInstance("AgentCount", this.AgentCount.ToString()),
                    new VariableInstance("Owner", this.Owner?.ToString()),
                    new VariableInstance("Last", this.Last?.ToString()),
                };
            }

            // methods

            protected void NotifyParameterChanged(string name, object value) {
                this.NotifyParameterChanged(name, value);
            }

            public virtual Store Clone(Store store = null) {
                if (store == null) {
                    store = new Store();
                }

                store.AgentCount = this.AgentCount;
                store.Last = this.Last;
                store.Owner = this.Owner;

                return store;
            }
        }
        #endregion

        #region class Instance
        public class Instance : IStateProvider {
            private WorkflowPosition state;
            private readonly List<Governor> agents;
            private Workflow workflow;
            private Store resources;

            // helper properties

            public Workflow Workflow { get { return this.workflow; } }

            public WorkflowPosition Start { get { return this.workflow.Start; } }

            public string Id { get { return this.workflow.Id; } }

            public string Name { get { return this.workflow.Name; } }

            public bool Stateless { get { return this.workflow.Stateless; } }

            public ReadOnlyCollection<ActionBase> Actions { get { return this.workflow.Actions; } }

            public ReadOnlyCollection<Connection> Connections { get { return this.workflow.Connections; } }

            // properties

            public Store Resources { get { return this.resources; } }

            public ReadOnlyCollection<Governor> Agents { get; }

            public Instance Parent { get; }

            public int InstanceId { get; }

            public WorkflowPosition State {
                get { return this.state; }
                set {
                    if (this.workflow.Stateless) {
                        throw new ApplicationException("Stateles workflows cannot change ports");
                    }

                    this.state = value;

                    // notify state that workflow entered it (start timer ...)
                    this.state.GetInstance(this.InstanceId).EnterPosition();

                    // log state change
                    if (Log.IsInfo) Logger.Info(null, "Workflow", InstitutionCodes.WorkflowChangedState,
                        this.state.Id,
                        this.workflow.Id,
                        this.InstanceId.ToString());

                    // notify all agents
                    for (var i = this.Agents.Count - 1; i >= 0; i--) {
                        this.Agents[i].NotifyPortChange();
                    }
                }
            }

            // constructor

            public Instance(int id, Workflow workflow, Instance parent) {
                this.InstanceId = id;
                this.Parent = parent;
                this.workflow = workflow;

                this.agents = new List<Governor>();
                this.Agents = new ReadOnlyCollection<Governor>(this.agents);

                // set the initial state to the output state of the start Action
                this.state = this.workflow.Start;

                // init state
                this.resources = this.workflow.CreateStore();
                this.Resources.AgentCount = 0;
                this.Resources.Last = null;
                this.Resources.Owner = null;
            }

            // methods

            public WorkflowInfo GetInfo(Governor agent) {
                return new WorkflowInfo(
                    this.workflow.Id,
                    this.InstanceId,
                    this.Name,
                    this.Resources.FilterByAccess(agent));
            }

            public void Close() {
                for (var i = this.Agents.Count - 1; i >= 0; i--) {
                    // also remove all agents that can be removed
                    this.Agents[i].ExitWorkflow();
                }
                this.Workflow.RemoveInstance(this.InstanceId);
            }

            public bool Join(Governor governor) {
                var state = this.State as State;

                // check if agent can enter workflow
                if (state == null || !this.workflow.Stateless && !state.CanEnter(governor)) {
                    return false;
                }

                // add this agent to the list of players
                this.AddAgent(governor);

                // log action
                governor.NotifyEnteredWorkflow(this);

                // set governor's position
                state.EnterAgent(governor);

                // remember this agent
                this.Resources.Last = governor.Resources;

                return true;
            }

            public void Exit(Governor governor) {
                // exit from workflow
                this.RemoveAgent(governor);
            }

            public void NotifyParameterChanged(string parameterName, object newValue) {
                // log the result
                if (Log.IsInfo) Logger.Info(null, "Workflow", InstitutionCodes.WorkflowParameterChanged, this.Id, this.InstanceId.ToString(), parameterName, newValue.ToString());

                // notify all agents
                for (int i = this.agents.Count - 1; i >= 0; i--) {
                    if (i < this.agents.Count) {
                        this.agents[i].NotifyWorkflowParameterChanged(this.Id, this.InstanceId, parameterName, newValue);
                    }
                }
            }

            public void NotifyRoles(IEnumerable<Group> notifyRoles, string agentName, string activityId, ParameterState parameters) {
                foreach (var agent in this.agents.ToArray()) {
                    if (agent.IsInGroup(notifyRoles)) {
                        agent.NotifyActivity(this, agentName, activityId, parameters);
                    }
                }
            }

            public void NotifyAgents(IEnumerable<string> notifyAgents, string name, ParameterState parameters) {
                throw new NotImplementedException();
            }
            //
            //        public void Continue()
            //        {
            //            // continue execution if possible
            //            this.Governor.Continue();
            //        }

            public void RemoveAgent(Governor governor) {
                if (this.agents.Contains(governor)) {
                    this.agents.Remove(governor);
                }
                this.Resources.AgentCount = this.agents.Count;
            }

            public void AddAgent(Governor governor) {
                this.agents.Add(governor);
                this.Resources.AgentCount = this.agents.Count;
            }

            public WorkflowPosition FindPosition(string id) {
                WorkflowPosition position = this.workflow.States.FirstOrDefault(w => w.Id == id);
                if (position == null) {
                    position = this.workflow.Transitions.FirstOrDefault(w => w.Id == id);
                }
                if (position == null) {
                    throw new Exception("Position not found: " + id);
                }
                return position;
            }


            Dictionary<string, int> positions;
            private int[,] distance;
            private int[,] paths;

            /////////////////////////////////////////////////////////
            /// FLOYD WARSHALL STUFF 
            /////////////////////////////////////////////////////////

            public bool CanAccess(WorkflowPosition from, WorkflowPosition to) {
                if (this.positions == null) {
                    this.CreateAccessGrid();
                }
                return this.distance[this.positions[from.Id], this.positions[to.Id]] > 0;
            }

            private void CreateAccessGrid() {
                positions = new Dictionary<string, int>();

                var idx = 0;
                foreach (var state in this.workflow.States) this.positions.Add(state.Id, idx++);
                foreach (var transition in this.workflow.Transitions) this.positions.Add(transition.Id, idx++);

                var N = positions.Count;

                distance = new int[N, N];
                paths = new int[N, N];

                // Initialize route matrix

                for (int i = 0; i < N; i++) {
                    for (int t = 0; t < N; t++) {
                        paths[i, t] = t;
                        distance[i, t] = 0;
                    }
                }

                if (Log.IsDebug) Log.Debug(null, "Workflow", string.Join("\n", this.positions.Select(w => w.Value + ": " + w.Key).ToArray()));

                // initialise distance matrix

                for (var i = 0; i < this.Connections.Count; i++) {
                    var from = this.positions[this.Connections[i].From.Id];
                    var to = this.positions[this.Connections[i].To.Id];
                    distance[from, to] = 1;
                }

                // Floyd-Warshall alghoritm

                for (int v = 0; v < N; v++) {
                    for (int u = 0; u < N; u++) {
                        if (u == v)
                            continue;

                        for (int k = 0; k < N; k++) {
                            if (k == u || k == v)
                                continue;

                            if (distance[v, u] > distance[v, k] + distance[k, u]) {
                                distance[v, u] = distance[v, k] + distance[k, u];
                                paths[v, u] = k;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveInstance(int instanceId) {
            this.instances.Remove(instanceId);
        }
        #endregion

        // fields
        // private bool initialised;
        private Institution institution;
        private List<State> openStates;
        private readonly Dictionary<int, Instance> instances;
        private readonly List<Connection> connections;
        private readonly List<State> states;
        private readonly List<Transition> transitions;
        private readonly List<ActionBase> actions;
        private Access joinPermissions;
        private Access createPermissions;
        //private Access joinPermissionsOnWorkflowState;
        //private Access joinPermissionsOnAgentState;

        private int lastIndex;

        // properties

        public Institution Institution { get { return this.institution; } }

        public State Start { get; private set; }

        public State End { get; private set; }

        public ReadOnlyCollection<Connection> Connections { get; private set; }

        public ReadOnlyCollection<State> States { get; private set; }

        public ReadOnlyCollection<Transition> Transitions { get; private set; }

        public ReadOnlyCollection<ActionBase> Actions { get; private set; }

        // abstract

        public Access JoinPermissions { get { return this.joinPermissions; } }

        public Access CreatePermissions { get { return this.createPermissions; } }

        public abstract bool Stateless { get; }

        public abstract bool Static { get; }

        public abstract Workflow.Store CreateStore();

        // constructor

        public Workflow(Institution institution, string id) : base(id) {
            this.institution = institution;
            this.lastIndex = 0;
            this.instances = new Dictionary<int, Instance>();

            this.connections = new List<Connection>();
            this.Connections = new ReadOnlyCollection<Connection>(this.connections);
            this.states = new List<State>();
            this.States = new ReadOnlyCollection<State>(this.states);
            this.transitions = new List<Transition>();
            this.Transitions = new ReadOnlyCollection<Transition>(this.transitions);
            this.actions = new List<ActionBase>();
            this.Actions = new ReadOnlyCollection<ActionBase>(actions);
 
        }

        public Access AddJoinPermissions(AccessCondition condition) {
            if (this.joinPermissions == null) {
                this.joinPermissions = new Access();
            }
            this.joinPermissions.Add(condition);
            return this.joinPermissions;
        }

        public Access AddCreatePermissions(AccessCondition condition) {
            if (this.createPermissions == null) {
                this.createPermissions = new Access();
            }
            this.createPermissions.Add(condition);
            return this.createPermissions;
        }

        public void AddStates(params State[] states) {
            this.states.AddRange(states);
        }

        public void AddTransitions(params Transition[] transitions) {
            this.transitions.AddRange(transitions);
        }

        public void AddActions(params ActionBase[] actions) {
            this.actions.AddRange(actions);
        }

        public WorkflowPosition GetPosition(string id) {
            if (string.IsNullOrEmpty(id)) {
                return null;
            }
            WorkflowPosition position = this.States.FirstOrDefault(s => s.Id == id);
            if (position == null) {
                position = this.Transitions.FirstOrDefault(t => t.Id == id);
                if (position == null) {
                    throw new Exception($"Position '{id}' does not exist");
                }
                return position;
            }
            return position;
        }

        public ActionBase GetAction(string id) {
            if (string.IsNullOrEmpty(id)) {
                return null;
            }
            ActionBase action = this.Actions.FirstOrDefault(s => s.Id == id);
            if (action == null) {
               throw new Exception($"Action '{id}' does not exist");
            }
            return action;
        }

        public Connection Connect(string id, WorkflowPosition from, WorkflowPosition to, ActionBase action = null, int allowLoops = 0) {
            var conn = new Connection(this.Institution, id, from, to, action);
            conn.AllowLoops = allowLoops;
            this.connections.Add(conn);
            return conn;
        }

        public void Init() {


            // init states
            if (this.States != null) {
                this.openStates = this.States.Where(s => s.Open).ToList();
            }

            // init START

            this.Start = this.States.FirstOrDefault(s => s.IsStart);

            // add start to open states

            if (this.Start == null) {
                throw new KeyNotFoundException($"Start element does not exists in workflow '{this.Name}'");
            }

            // init FINISH

            this.End = this.States.FirstOrDefault(s => s.IsEnd);

            // add end to open states

            if (this.End == null) {
                throw new KeyNotFoundException($"End element does not exists in workflow '{this.Name}'");
            }

            // connect open states
            // create connections for all open states
            // 1. completely open connections create loops in every open state
            // 2. connections open IN are connected FROM any open state
            // 3. connections open OUT are connected TO any open state 

            if (this.Connections != null) {
                for (var i = this.connections.Count - 1; i >= 0; i--) {
                    var conn = this.Connections[i];
                    if (conn.OpenIn && conn.OpenOut) {
                        this.openStates.ForEach(w => {
                            var c = conn.Clone(w, w);
                            this.connections.Add(c);
                        });
                    }
                    else if (conn.OpenIn) {
                        this.openStates.ForEach(w => {
                            if (w != conn.To) {
                                var c = conn.Clone(w, conn.To);
                                this.connections.Add(c);
                            }
                        });
                    }
                    else if (conn.OpenOut) {
                        this.openStates.ForEach(w => {
                            if (w != conn.From) {
                                var c = conn.Clone(conn.From, w);
                                this.connections.Add(c);
                            }
                        });
                    }
                }
            }
        }

        public Workflow.Instance GetInstance(int id) {
            if (this.instances.ContainsKey(id)) {
                return this.instances[id];
            }
            return null;
        }

        public Workflow.Instance StartWorkflow(Workflow.Instance parent = null) {
            // make sure it is initialised

            var newId = this.lastIndex++;
            var instance = this.CreateInstance(newId, parent);
            this.instances.Add(newId, instance);
            return instance;
        }

        public virtual Workflow.Instance CreateInstance(int id, Workflow.Instance parentWorkflow) {
            return new Instance(id, this, parentWorkflow);
        }


    }
}
