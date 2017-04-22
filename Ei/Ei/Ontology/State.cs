using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using Ei.Logs;
using Ei.Ontology.Actions;
using Ei.Runtime;

namespace Ei.Ontology
{
    public class State : WorkflowPosition
    {
        // fields
        private Timer timer;

        // properties

        public bool IsStart { get; }
        public bool IsEnd { get; }

        public bool Open { get; }

        public Access EntryRules { get; }
        public Access ExitRules { get; }

        public int Timeout { get; }

        // ctor

        public State(Institution ei,
            string id,
            string name,
            string description,
            Workflow workflow,
            bool open,
            int timeout,
            Access entryRules,
            Access exitRules,
            bool isStart, bool isEnd): base(id, name, description, workflow)
        {
            this.Open = open;
            this.EntryRules = entryRules;
            this.ExitRules = exitRules;

            this.IsStart = isStart;
            this.IsEnd = isEnd;

            this.Timeout = timeout;
        }

        // public methods

        public override bool CanEnter(Governor agent)
        {
            // we add default values of parameters we are checking for
            return this.EntryRules.CanAccess(agent.Groups, agent.VariableState);
        }

        public override void ExitPosition(Governor agent)
        {
            if (this.timer != null)
            {
                //this.timer.Elapsed -= this.Timedout;
                this.timer.Stop();
            }
        }

        public override IActionInfo EnterAgent(Governor agent)
        {
            // base adds agent to the current list of agents
            base.EnterAgent(agent);

            // stateless part

            if (this.workflow.Stateless)
            {
                // if this is exit state remove agent
                if (this.IsEnd && this.CanExit(agent))
                {
                    agent.ExitWorkflow();
                }

                if (this.Timeout > 0)
                {
                    throw new ApplicationException("Only statefull workflows can contain timeouts!");
                }
            }
            return ActionInfo.Ok;
        }

        public override IActionInfo EnterWorkflow()
        {
            for (var i = this.Agents.Count - 1; i >= 0; i--)
            {
                // also remove all agents that can be removed
                if (this.IsEnd && this.CanExit(this.Agents[i]))
                {
                    this.Agents[i].ExitWorkflow();
                }
            }

            // handle timeout

            if (this.Timeout > 0)
            {
                if (timer == null)
                {
                    timer = new Timer();
                    timer.AutoReset = false;
                    timer.Elapsed += Timedout;
                }
                timer.Stop();
                timer.Interval = this.Timeout;
                timer.Start();
                timer.Enabled = true;

                // if (Log.IsDebug) Log.Debug("[TIMER] " + (this.Timeout / 1000) + " sec");
            }

            return ActionInfo.Ok;
        }


        public override bool CanExit(Governor agent)
        {
            // we add default values of parameters we are checking for
            return this.ExitRules.CanAccess(agent.Groups, agent.VariableState);
        }

//        public override string ToString()
//        {
//            return string.Join("\n", this.connections.Select(w => w.ToString()).ToArray());
//        }

        public override string ToString()
        {
            return this.Id;
        }

        // private methods

        private void Timedout(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (Log.IsInfo) Log.Info("State", InstitutionCodes.TimeOut, this.workflow.Id, this.workflow.InstanceId.ToString(), this.Id);

            // find timeout connection
            var connection = this.Outs.FirstOrDefault(w => w.Action is ActionTimeout);
            if (connection == null)
            {
                throw new ApplicationException("There is no timeout connection!");
            }
            if (!connection.Preconditions.IsEmpty)
            {
                throw new ApplicationException("Timeout connection cannot contain preconditions!");
            }
            if (connection.HasActivityParameters || connection.HasAgentParameters)
            {
                throw new ApplicationException("Timeout connections cannot contain agent or action parameters!");
            }

            // apply postconditions
            if (connection.Postconditions != null)
            {
                foreach (var postcondition in connection.Postconditions)
                {
                    postcondition.ApplyPostconditions(this.workflow.VariableState, false);
                }
            }

            this.workflow.State = connection.To;
        }
    }
}
