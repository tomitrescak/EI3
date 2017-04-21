using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ei.Logs;
using Ei.Ontology.Transitions;
using Ei.Runtime;
using Ei.Runtime.Planning;

namespace Ei.Ontology
{
    public abstract class WorkflowPosition : Entity
    {
        // fields
        protected Workflow workflow;
        private readonly List<Connection> ins;
        private readonly List<Connection> outs;
        private readonly List<Governor> agents;
        private ReadOnlyCollection<Governor> readAgents;

        // properties

        public ReadOnlyCollection<Governor> Agents { get
        {
            return this.workflow.Stateless ? this.readAgents : this.workflow.Agents; 
        } }
        public ReadOnlyCollection<Connection> Ins;
        public ReadOnlyCollection<Connection> Outs;

        // ctor

        protected WorkflowPosition(string id, string name, string description, Workflow workflow) : base(id, name, description)
        {
            this.workflow = workflow;

            this.agents = new List<Governor>();
            this.readAgents = new ReadOnlyCollection<Governor>(this.agents);

            this.ins = new List<Connection>();
            this.Ins = new ReadOnlyCollection<Connection>(this.ins);

            this.outs = new List<Connection>();
            this.Outs = new ReadOnlyCollection<Connection>(this.outs);
        }

        public void ConnectTo(Connection connection)
        {
            this.outs.Add(connection);
        }

        public void ConnectFrom(Connection connection)
        {
            this.ins.Add(connection);
        }

        // finding connections

        public Connection[] ViableTransitions(Governor agent)
        {
            return this.ViableTransitions(agent.Groups, agent.VariableState);
        }

        public Connection[] ViableTransitions(Group[] groups, VariableState state)
        {
            return this.outs.Where(w => w.To is Transition && w.CanPass(groups, state)).ToArray();
        }

        public Connection[] ViableConnections(Governor agent)
        {
            return this.ViableConnections(agent.Groups, agent.VariableState);
        }

        public Connection[] ViableConnections(Group[] groups, VariableState state)
        {
            return this.outs.Where(w => w.CanPass(groups, state)).ToArray();
        }

        public Connection[] ViableInputs(Group[] groups, VariableState state)
        {
            return this.ins.Where(w => w.CanBacktrack(groups, state)).ToArray();
        }

        public Connection[] ViableConnections(Governor agent, string actionId, VariableState parameters)
        {
            return this.ViableConnections(agent.Groups, agent.VariableState, actionId, parameters);
        }

        public Connection[] ViableConnections(Group[] groups, VariableState state, string actionId, VariableState parameters)
        {
            return this.outs.Where(w => w.CanPass(groups, state) && w.Action != null && w.Action.Id == actionId).ToArray();
        }

        public virtual bool CanEnter(Governor agent)
        {
            return false;
        }

        public virtual IActionInfo EnterWorkflow()
        {
            return ActionInfo.Ok;
        }

        public virtual IActionInfo EnterAgent(Governor agent)
        {
            if (this.workflow.Stateless)
            {
                // add agent to the list
                this.agents.Add(agent);

                // leave current position
                if (agent.Position != null)
                {
                    agent.Position.Exit(agent);
                }

                agent.Position = this;

                // action is logged only in stateless mode
                // in state full mode the change of position is handled by workflow
                agent.LogAction(InstitutionCodes.ChangedState,
                    agent.Name,
                    this.Id,
                    this.workflow.Id,
                    this.workflow.InstanceId.ToString());

                agent.NotifyPortChange();

                return ActionInfo.Ok;
            }

            // statefull workflow

            this.workflow.State = this;

            // handle timeout
            return ActionInfo.OkButDoNotContinue;
        }

        public virtual bool CanExit(Governor agent)
        {
            return false;
        }

        public virtual IActionInfo Exit(Governor agent)
        {
            try
            {
                if (this.agents.Contains(agent))
                {
                    this.agents.Remove(agent);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(agent.Name, "Waring exiting workflow: " + ex.Message);
                
            }
            return ActionInfo.OkButDoNotContinue;
        }


        public virtual void ExitPosition(Governor agent)
        {
        }
    }
}
