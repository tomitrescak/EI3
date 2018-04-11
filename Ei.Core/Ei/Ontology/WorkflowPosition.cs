using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ei.Core.Ontology;
using Ei.Core.Ontology.Transitions;
using Ei.Logs;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;

namespace Ei.Core.Ontology
{
    public abstract class WorkflowPosition : Entity
    {
        #region class PositionInstance
        public abstract class PositionInstance
        {
            private int id;
            protected WorkflowPosition position;
            private readonly List<Governor> agents;
            private ReadOnlyCollection<Governor> readAgents;

            // properties

            public int Id { get { return this.id; } }

            public ReadOnlyCollection<Governor> Agents {
                get {
                    return this.position.workflow.Stateless ? this.readAgents : this.position.workflow.GetInstance(id).Agents;
                }
            }

            // constructor

            protected PositionInstance(int id, WorkflowPosition position) {
                this.id = id;
                this.position = position;
                this.agents = new List<Governor>();
                this.readAgents = new ReadOnlyCollection<Governor>(this.agents);
            }

            // methods

            public void AddAgent(Governor agent) {
                this.agents.Add(agent);
            }

            public void RemoveAgent(Governor agent) {
                this.agents.Remove(agent);
            }

            public virtual IActionInfo EnterPosition() {
                return ActionInfo.Ok;
            }

            public virtual void ExitPosition(Governor agent) {
            }
        }
        #endregion

        // fields
        protected Workflow workflow;
        private Dictionary<int, PositionInstance> instances;

        private readonly List<Connection> ins;
        private readonly List<Connection> outs;

        public ReadOnlyCollection<Connection> Ins;
        public ReadOnlyCollection<Connection> Outs;

        // ctor

        protected WorkflowPosition(string id, Workflow workflow) : this(id, null, null, workflow) {
        }

        protected WorkflowPosition(string id, string name, string description, Workflow workflow) : base(id, name, description) {
            this.workflow = workflow;
            this.instances = new Dictionary<int, PositionInstance>();

            this.ins = new List<Connection>();
            this.Ins = new ReadOnlyCollection<Connection>(this.ins);

            this.outs = new List<Connection>();
            this.Outs = new ReadOnlyCollection<Connection>(this.outs);
        }

        public Workflow Workflow => this.workflow;

        // methods

        public void ConnectTo(Connection connection) {
            this.outs.Add(connection);
        }

        public void ConnectFrom(Connection connection) {
            this.ins.Add(connection);
        }

        // finding connections

        public Connection[] ViableTransitions(Governor.GovernorState agent) {
            return this.ViableTransitions(agent.Governor.Groups, agent);
        }

        public Connection[] ViableTransitions(Group[] groups, Governor.GovernorState state) {
            return this.outs.Where(w => w.To is Transition && w.CanPass(state)).ToArray();
        }

        public Connection[] ViableConnections(Governor agent) {
            return this.ViableConnections(agent.Groups, agent.Resources);
        }

        public Connection[] ViableConnections(Group[] groups, Governor.GovernorState state) {
            return this.outs.Where(w => w.CanPass(state)).ToArray();
        }

        //public Connection[] ViableInputs(Group[] groups, Governor.GovernorVariableState state) {
        //  return this.ins.Where(w => w.CanBacktrack(state)).ToArray();
        //}

        public Connection[] ViableConnections(Governor agent, string actionId) {
            return this.ViableConnections(agent.Groups, agent.Resources, actionId);
        }

        public Connection[] ViableConnections(Group[] groups, Governor.GovernorState state, string actionId) {
            return this.outs.Where(w => w.CanPass(state) && w.Action != null && w.Action.Id == actionId).ToArray();
        }

        public virtual bool CanEnter(Governor agent) {
            return false;
        }

        public PositionInstance GetInstance(int id) {
            if (!this.instances.ContainsKey(id)) {
                this.instances.Add(id, this.CreateInstance(id, this));
            }
            return this.instances[id];
        }

        public abstract PositionInstance CreateInstance(int id, WorkflowPosition position);

        public virtual IActionInfo EnterAgent(Governor agent) {
            var workflowId = agent.Workflow.InstanceId;
            var state = this.GetInstance(workflowId);

            if (this.workflow.Stateless) {
                // add agent to the list
                state.AddAgent(agent);

                // leave current position
                if (agent.Position != null) {
                    agent.Position.Exit(agent);
                }

                agent.Position = this;

                // action is logged only in stateless mode
                // in state full mode the change of position is handled by workflow
                agent.LogAction(InstitutionCodes.ChangedState,
                    agent.Name,
                    this.Id,
                    this.workflow.Id,
                   workflowId.ToString());

                agent.NotifyPortChange();

                return ActionInfo.Ok;
            }

            // statefull workflow

            agent.Workflow.State = this;

            // handle timeout
            return ActionInfo.OkButDoNotContinue;
        }

        public virtual bool CanExit(Governor agent) {
            return false;
        }

        public virtual IActionInfo Exit(Governor agent) {
            var state = this.GetInstance(agent.Workflow.InstanceId);
            try {
                if (state.Agents.Contains(agent)) {
                    state.RemoveAgent(agent);
                }
            }
            catch (Exception ex) {
                Log.Warning(agent.Name, "Warning exiting workflow: " + ex.Message);

            }
            return ActionInfo.OkButDoNotContinue;
        }



    }
}
