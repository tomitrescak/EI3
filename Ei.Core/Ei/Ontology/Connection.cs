using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Runtime;
using ActionBase = Ei.Ontology.Actions.ActionBase;
using Ei.Ontology.Actions;

namespace Ei.Ontology
{
    public class Connection
    {
        static int openId = 0;
        static string OpenId => "open" + openId++;
   
        #region Properties

        public string Id { get; private set; }
        public Access Access { get; private set; }

        // public AccessCondition[] GeneratedNestedEffects { get; set; }
        public AccessCondition[] ExpectedEffects { get; set; }

        //public AccessCondition[] BacktrackPreconditions { get; set; }
        //public AccessCondition[] BacktrackPostconditions { get; set; }

        public WorkflowPosition From { get; private set; }
        public WorkflowPosition To { get; private set; }

        public ActionBase Action { get; private set; }
        public bool OpenIn { get; private set; }
        public bool OpenOut { get; private set; }

        public int AllowLoops { get; set; }

        /// <summary>
        /// Combination of 
        /// </summary>
        // public Dictionary<string, object> MinSat { get { throw new NotImplementedException(); } }

        public int Cost { get { return 1; } }

        protected string TypeName { get { return this.GetType().Name; } }


        /// <summary>
        /// Gets or sets a value indicating whether agent can automatically flow through this Action
        /// or it has to provide input
        /// </summary>

       

        #endregion


        public Connection(string id, WorkflowPosition from, WorkflowPosition to) {
            this.Id = id;
            this.From = from;
            this.To = to;

            // set what type of connection it is
            if (from == null) {
                this.OpenIn = true;
            }

            if (to == null) {
                this.OpenOut = true;
            }

            if (from != null && to != null) {
                from.ConnectTo(this);
                to.ConnectFrom(this);
            }
        }

        public Connection(WorkflowPosition from, WorkflowPosition to, Connection conn) : this(conn.Id, from, to) {

            this.Action = conn.Action;
            this.Access = conn.Access;
            this.AllowLoops = conn.AllowLoops;
            this.ExpectedEffects = conn.ExpectedEffects;
        }

        public Connection(Institution ei, string id, WorkflowPosition from, WorkflowPosition to, ActionBase action) : this(id, from, to) {
            this.Action = action;
        }

        // constructor helpers

        public Connection Condition(AccessCondition condition) {
            if (this.Access == null) {
                this.Access = new Access();
            }
            this.Access.Add(condition);
            return this;
        }

        public Connection AddEffects(AccessCondition[] effects) {
            this.ExpectedEffects = effects != null && effects.Length > 0 ? effects : null;
            return this;
        }

        // methods

        public IActionInfo Pass(Governor agent, ParameterState parameters = null) {
            if (!this.CanPass(agent.Resources)) {
                return ActionInfo.FailedPreconditions;
            }

            // leave the original state
            if (this.From != null) {
                this.From.GetInstance(agent.Workflow.InstanceId).ExitPosition(agent);
            }

            if (this.Action != null) {
                var actionResult = this.Action.Perform(agent, this, parameters);
                if (actionResult.IsBreak) {
                    return ActionInfo.Ok;
                }
                if (actionResult.IsNotOk) {
                    return actionResult;
                }
            }

            // apply postconditions on connection
            if (this.Access != null) {
                this.Access.ApplyPostconditions(agent.Resources, agent.Workflow.Resources, parameters);
            }

            // open connections have To set to null, so we stay n the same state
            if (this.To == null) {
                return ActionInfo.Ok;
            }

            // flow to input port
            return this.To.EnterAgent(agent);
        }

        //        public bool CanPass(Governor agent, params ParameterInstance[] parameters)
        //        {
        //            return this.CanPass(agent.Groups, agent.Properties);
        //        }

        public bool CanPass(Governor.GovernorState state, Workflow.Store workflowState = null) {
            if (this.Access == null) {
                return true;
            }
            return this.Access.CanAccess(state, workflowState);

        }

        //public bool CanBacktrack(Group[] groups, Governor.GovernorVariableState state) {
        //    // TODO: Also check if this is a workflow action if agent can join the workflow or create a new instance

        //    return this.BacktrackPreconditions == null ||
        //           this.BacktrackPreconditions.CanAccess(groups, state);
        //}

        //        public bool CanPassBinary(Governor agent, bool backtrack)
        //        {
        //            return this.CanPassBinary(agent.Groups, agent.Properties, backtrack);
        //        }


        public override string ToString() {
            return string.Format("{0} --> {1} [{2}]", this.From == null ? "open" : this.From.Id, this.To == null ? "open" : this.To.Id, this.Action);
        }

        public string ToChainString() {
            return string.Format("{0} [{1}]", this.To == null ? "open" : this.To.Id, this.Action);
        }

        public Connection Clone(WorkflowPosition from, WorkflowPosition to) {
            return new Connection(from, to, this);
        }


        public void ApplyPostconditions(Governor.GovernorState state, Workflow.Store workflowState, ParameterState actionParameters, bool planningMode = false) {
            if (this.Access == null) {
                return;
            }
            this.Access.ApplyPostconditions(state, workflowState, actionParameters, planningMode);
        }


        //public void ApplyBacktrackPostconditions(Group[] groups, Governor.GovernorState state) {
        //    if (this.BacktrackPostconditions != null) {
        //        foreach (var postcondition in this.BacktrackPostconditions) {
        //            // check if arc is constrained to the agent role
        //            postcondition.ApplyPostconditions(state, null, true);
        //        }
        //    }
        //}

        public void ApplyExpectedEffects(Governor.GovernorState state, Workflow.Store workflowState, AccessCondition effect) {
            // we can either apply a single effect
            // or all of them

            if (effect != null) {
                state.ResetDirty();
                effect.ApplyPostconditions(state, workflowState, null, true);
                return;
            }


            if (this.ExpectedEffects != null) {
                state.ResetDirty();

                foreach (var postcondition in this.ExpectedEffects) {
                    // check if arc is constrained to the agent role
                    postcondition.ApplyPostconditions(state, workflowState, null, true);
                }
            }
        }

        //public void ApplyGeneratedBacktrackEffects(Governor.GovernorState state) {
        //    if (this.GeneratedNestedEffects != null) {
        //        foreach (var postcondition in this.GeneratedNestedEffects) {
        //            // check if arc is constrained to the agent role
        //            postcondition.ApplyPostconditions(state, null, true);
        //        }
        //    }
        //}


    }
}
