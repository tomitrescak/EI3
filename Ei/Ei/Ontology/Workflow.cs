

namespace Ei.Ontology
{
    using System;
    using System.Runtime.CompilerServices;
    using Ei.Logs;
    using Ei.Ontology.Transitions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Ei.Runtime;
    using ActionBase = Ei.Ontology.Actions.ActionBase;

    public abstract class Workflow: Entity, IStateProvider
    {
        public class WorkflowVariableState: VariableState
        {

            // fields

            private int agentCount;
            private VariableState last;
            private VariableState owner;
            private Workflow workflow;

            // properties

            public int AgentCount {
                get {
                    return agentCount;
                }

                set {
                    this.agentCount = value;
                }
            }

            public VariableState Last {
                get {
                    return last;
                }

                set {
                    this.last = value;
                }
            }

            public VariableState Owner {
                get {
                    return owner;
                }

                set {
                    this.owner = value;
                }
            }

            // ctors

            public WorkflowVariableState(Workflow workflow) {
                this.workflow = workflow;
            }

            // methods

            protected void NotifyParameterChanged(string name, object value) {
                this.workflow.NotifyParameterChanged(name, value);
            }
        }


        // fields
        private Institution institution;
        private WorkflowPosition state;
        private readonly List<Governor> agents;
        private readonly List<Transition> transitions;
        private readonly List<State> states;
        private List<State> openStates;

        // properties

        public WorkflowPosition State
        {
            get { return this.state; }
            set
            {
                if (this.Stateless)
                {
                    throw new ApplicationException("Stateles workflows cannot change ports");
                }

                this.state = value;

                // notify state that workflow entered it (start timer ...)
                this.state.EnterWorkflow();

                // log state change
                if (Log.IsInfo) Log.Info(this.Id, InstitutionCodes.WorkflowChangedState,
                    this.state.Id,
                    this.Id,
                    this.InstanceId.ToString());

                // notify all agents
                for (var i = this.Agents.Count - 1; i >= 0; i--)
                {
                    this.Agents[i].NotifyPortChange();
                }
            }
        }

        public Workflow Parent { get; }

        public int InstanceId { get; }

        public State Start { get; private set; }

        public State End { get; private set; }

        // abstract

        public abstract List<Connection> Connections { get; }

        public abstract WorkflowVariableState VariableState { get; }

        public abstract ReadOnlyCollection<Governor> Agents { get; } 

        public abstract bool Stateless { get; }

        public abstract bool Static { get; }

        public abstract ReadOnlyCollection<State> States { get;  }

        public abstract ReadOnlyCollection<ActionBase> Actions { get; }

        public abstract Access CreatePermissions { get; }

        // constructor

        public Workflow(Institution institution, Workflow parent, string id, string name, string description, int instanceId)
            : base(id, name, description)
        {
            this.institution = institution;
            this.InstanceId = instanceId;
            this.Parent = parent;

            // init states
            if (this.States != null)
            {
                this.openStates = this.States.Where(s => s.Open).ToList();
            }

            // init START

            this.Start = this.States.FirstOrDefault(s => s.IsStart); 

            // add start to open states

            if (this.Start == null)
            {
                throw new KeyNotFoundException($"Start element does not exists in workflow '{this.Name}'");
            }

            // init FINISH

            this.End = this.States.FirstOrDefault(s => s.IsEnd);
            
            // add end to open states

            if (this.End == null)
            {
                throw new KeyNotFoundException($"End element does not exists in workflow '{this.Name}'");
            }

            // connect open states
            // create connections for all open states
            // 1. completely open connections create loops in every open state
            // 2. connections open IN are connected FROM any open state
            // 3. connections open OUT are connected TO any open state 

            if (this.Connections != null)
            {
                foreach (var conn in this.Connections)
                {
                    if (conn.OpenIn && conn.OpenOut)
                    {
                        this.openStates.ForEach(w =>
                        {
                            var c = conn.Clone(w, w);
                            this.Connections.Add(c);
                        });
                    }
                    else if (conn.OpenIn)
                    {
                        this.openStates.ForEach(w => 
                        {
                            if (w != conn.To)
                            {
                                var c = conn.Clone(w, conn.To);
                                this.Connections.Add(c);
                            }
                        });
                    }
                    else if (conn.OpenOut)
                    {
                        this.openStates.ForEach(w =>
                        {
                            if (w != conn.From)
                            {
                                var c = conn.Clone(conn.From, w);
                                this.Connections.Add(c);
                            }
                        });
                    }
                }
            }

            

            // set the initial state to the output state of the start Action
            this.state = this.Start;
            
            // set the agent count to 0
            this.VariableState.AgentCount = 0;
            this.VariableState.Last = null;
            this.VariableState.Owner = null;
        }

        public abstract Workflow CreateInstance(Institution ei, Workflow parentWorkflow, int instanceId);

        public WorkflowInfo GetInfo(Governor agent)
        {
            return new WorkflowInfo(
                this.Id,
                this.InstanceId,
                this.Name, 
                this.VariableState.FilterByAccess(agent));
        }

        

        public bool Join(Governor governor)
        {
            var state = this.State as State;

            // check if agent can enter workflow
            if (state == null || !this.Stateless && !state.CanEnter(governor))
            {
                return false;
            }

            // add this agent to the list of players
            this.AddAgent(governor);

            // log action
            governor.NotifyEnteredWorkflow(this);

            // set governor's position
            state.EnterAgent(governor);

            // remember this agent
            this.VariableState.Last = governor.VariableState;

            return true;
        }

        public void Exit(Governor governor)
        {
            // exit from workflow
            this.RemoveAgent(governor);
        }

        public void NotifyParameterChanged(string parameterName, object newValue) {
            // log the result
            if (Log.IsInfo) Log.Info(this.Id, InstitutionCodes.WorkflowParameterChanged, this.Id, this.InstanceId.ToString(), parameterName, newValue.ToString());

            // notify all agents
            for (int i = this.agents.Count - 1; i >= 0; i--) {
                if (i < this.agents.Count) {
                    this.agents[i].NotifyWorkflowParameterChanged(this.Id, this.InstanceId, parameterName, newValue);
                }
            }
        }

        public void NotifyRoles(IEnumerable<Group> notifyRoles, string agentName, string activityId, VariableState parameters)
        {
            foreach (var agent in this.agents.ToArray())
            {
                if (agent.IsInGroup(notifyRoles))
                {
                    agent.NotifyActivity(this, agentName, activityId, parameters);
                }
            }
        }

        public void NotifyAgents(IEnumerable<string> notifyAgents, string name, VariableState parameters)
        {
            throw new NotImplementedException();
        }
//
//        public void Continue()
//        {
//            // continue execution if possible
//            this.Governor.Continue();
//        }

        public void RemoveAgent(Governor governor)
        {
            if (this.agents.Contains(governor))
            {
                this.agents.Remove(governor);
            }
            this.VariableState.AgentCount = this.agents.Count;
        }

        public void AddAgent(Governor governor)
        {
            this.agents.Add(governor);
            this.VariableState.AgentCount = this.agents.Count;
        }

        public WorkflowPosition FindPosition(string id)
        {
            WorkflowPosition position = this.states.FirstOrDefault(w => w.Id == id);
            if (position == null)
            {
                position = transitions.FirstOrDefault(w => w.Id == id);
            }
            if (position == null)
            {
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

        public bool CanAccess(WorkflowPosition from, WorkflowPosition to)
        {
            if (this.positions == null)
            {
                this.CreateAccessGrid();
            }
            return this.distance[this.positions[from.Id], this.positions[to.Id]] > 0;
        }

        private void CreateAccessGrid()
        {
            positions = new Dictionary<string, int>();

            var idx = 0;
            foreach (var state in this.states) this.positions.Add(state.Id, idx ++);
            foreach (var transition in this.transitions) this.positions.Add(transition.Id, idx++);

            var N = positions.Count;

            distance = new int[N, N];
            paths = new int[N, N];

            // Initialize route matrix

            for (int i = 0; i < N; i++)
            {
                for (int t = 0; t < N; t++)
                {
                    paths[i, t] = t;
                    distance[i, t] = 0;
                }
            }

            if (Log.IsDebug) Log.Debug(string.Join("\n", this.positions.Select(w => w.Value + ": " + w.Key).ToArray()));

            // initialise distance matrix

            for (var i = 0; i < this.Connections.Count; i++)
            {
                var from = this.positions[this.Connections[i].From.Id];
                var to = this.positions[this.Connections[i].To.Id];
                distance[from, to] = 1;
            } 

            // Floyd-Warshall alghoritm

            for (int v = 0; v < N; v++)
            {
                for (int u = 0; u < N; u++)
                {
                    if (u == v)
                        continue;

                    for (int k = 0; k < N; k++)
                    {
                        if (k == u || k == v)
                            continue;

                        if (distance[v, u] > distance[v, k] + distance[k, u])
                        {
                            distance[v, u] = distance[v, k] + distance[k, u];
                            paths[v, u] = k;
                        }
                    }
                }
            }
        }
    }
}
