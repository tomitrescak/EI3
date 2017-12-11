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

        public class Instance : WorkflowPosition.PositionInstance
        {
            // fields
            private Timer timer;
            private State state;

            // constructor

            public Instance(int id, WorkflowPosition position) : base(id, position) {
                this.state = (State)position;
            }

            // methods

            public override void ExitPosition(Governor agent) {
                if (this.timer != null) {
                    //this.timer.Elapsed -= this.Timedout;
                    this.timer.Stop();
                }
            }

            public override IActionInfo EnterPosition() {
                for (var i = this.Agents.Count - 1; i >= 0; i--) {
                    // also remove all agents that can be removed
                    if (this.state.IsEnd && this.state.CanExit(this.Agents[i])) {
                        this.Agents[i].ExitWorkflow();
                    }
                }

                // handle timeout

                if (state.Timeout > 0) {
                    if (timer == null) {
                        timer = new Timer();
                        timer.AutoReset = false;
                        timer.Elapsed += this.Timedout;
                    }
                    timer.Stop();
                    timer.Interval = state.Timeout;
                    timer.Start();
                    timer.Enabled = true;

                    // if (Log.IsDebug) Log.Debug("[TIMER] " + (this.Timeout / 1000) + " sec");
                }

                return ActionInfo.Ok;
            }

            private void Timedout(object sender, ElapsedEventArgs elapsedEventArgs) {
                var workflow = state.workflow.GetInstance(this.Id);
                if (Log.IsInfo) Log.Info("State", InstitutionCodes.TimeOut, workflow.Id, workflow.InstanceId.ToString(), this.Id.ToString());

                // find timeout connection
                var connection = state.Outs.FirstOrDefault(w => w.Action is ActionTimeout);
                if (connection == null) {
                    throw new ApplicationException("There is no timeout connection!");
                }
                if (connection.Access == null || connection.Access.IsEmpty) {
                    throw new ApplicationException("Timeout connection cannot contain preconditions!");
                }
                if (connection.Access.HasActivityParameters || connection.Access.HasAgentParameters) {
                    throw new ApplicationException("Timeout connections cannot contain agent or action parameters!");
                }

                // apply postconditions

                connection.Access.ApplyPostconditions(workflow.Workflow.Institution.Resources, workflow.Resources);

                workflow.State = connection.To;
            }
        }


        // properties

        public bool IsStart { get; protected set; }
        public bool IsEnd { get; protected set; }

        public bool Open { get; }

        public Access EntryRules { get; }
        public Access ExitRules { get; }

        public int Timeout { get; }

        // ctor

        public State(string id, Workflow workflow) : base(id, workflow) { }

        public State(
            string id,
            string name,
            string description,
            Workflow workflow,
            bool open = false,
            int timeout = 0,
            Access entryRules = null,
            Access exitRules = null,
            bool isStart = false, 
            bool isEnd = false) : base(id, name, description, workflow) {
            this.Open = open;
            this.EntryRules = entryRules;
            this.ExitRules = exitRules;

            this.IsStart = isStart;
            this.IsEnd = isEnd;

            this.Timeout = timeout;
        }

        // public methods

        public override bool CanEnter(Governor agent) {
            // we add default values of parameters we are checking for
            return this.EntryRules.CanAccess(agent.Resources);
        }

        public override IActionInfo EnterAgent(Governor agent) {
            // base adds agent to the current list of agents
            base.EnterAgent(agent);

            // stateless part

            if (this.workflow.Stateless) {
                // if this is exit state remove agent
                if (this.IsEnd && this.CanExit(agent)) {
                    agent.ExitWorkflow();
                }

                if (this.Timeout > 0) {
                    throw new ApplicationException("Only statefull workflows can contain timeouts!");
                }
            }
            return ActionInfo.Ok;
        }


        public override bool CanExit(Governor agent) {
            // we add default values of parameters we are checking for
            return this.ExitRules == null || this.ExitRules.CanAccess(agent.Resources);
        }

        //        public override string ToString()
        //        {
        //            return string.Join("\n", this.connections.Select(w => w.ToString()).ToArray());
        //        }

        public override string ToString() {
            return this.Id;
        }

        // private methods

        public override PositionInstance CreateInstance(int id, WorkflowPosition position) {
            return new Instance(id, position);
        }
    }
}
