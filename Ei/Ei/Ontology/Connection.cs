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
        #region Properties

        public string Id { get; private set; }
        public Access Preconditions { get; set; }
        public AccessCondition[] Postconditions { get; set; }
        public AccessCondition[] GeneratedNestedEffects { get; set; }

        public Access BacktrackPreconditions { get; set; }
        public AccessCondition[] BacktrackPostconditions { get; set; }
        public AccessCondition[] ExpectedEffects { get; set; }

        public WorkflowPosition From { get; private set; }
        public WorkflowPosition To { get; private set; }

        public ActionBase Action { get; private set; }
        public bool OpenIn { get; private set; }
        public bool OpenOut { get; private set; }

        public int AllowLoops { get; private set; }

        /// <summary>
        /// Combination of 
        /// </summary>
        public Dictionary<string, object> MinSat { get { throw new NotImplementedException(); } }

        public int Cost { get { return 1; } }

        protected string TypeName { get { return this.GetType().Name; } }
        

        /// <summary>
        /// Gets or sets a value indicating whether agent can automatically flow through this Action
        /// or it has to provide input
        /// </summary>

        public bool HasAgentParameters { get; private set; }
        public bool HasActivityParameters { get; private set; }

        #endregion
       

        public Connection(WorkflowPosition from, WorkflowPosition to)
        {
            this.From = from;
            this.To = to;

            // set what type of connection it is
            if (from == null)
            {
                this.OpenIn = true;
            }

            if (to == null)
            {
                this.OpenOut = true;
            }

            if (from != null && to != null)
            {
                from.ConnectTo(this);
                to.ConnectFrom(this);
            }
        }

        public Connection(WorkflowPosition from, WorkflowPosition to, Connection conn) : this(from, to)
        {
            this.Id = conn.Id;
            this.Action = conn.Action;
            this.Preconditions = conn.Preconditions;
            this.Postconditions = conn.Postconditions;
            this.BacktrackPreconditions = conn.BacktrackPreconditions;
            this.BacktrackPostconditions = conn.BacktrackPostconditions;
            this.ExpectedEffects = conn.ExpectedEffects;
            this.HasActivityParameters = conn.HasActivityParameters;
            this.HasAgentParameters = conn.HasAgentParameters;
            this.GeneratedNestedEffects = conn.GeneratedNestedEffects;
            this.AllowLoops = conn.AllowLoops;
        }

        public Connection(Institution ei, WorkflowPosition from, WorkflowPosition to, ActionBase action): this(from, to)
        {
            this.Action = action;

            // check all conditions
            // auto connections can only contain workflow wariables (w.*) not Action (this.*) nor agent variables (a.*)
            this.HasAgentParameters = this.Preconditions.HasAgentParameter ||
                AccessCondition.CheckHasAgentParameters(this.Postconditions);

            this.HasActivityParameters = this.Preconditions.HasActivityParameter ||
                AccessCondition.CheckHasActivityParameters(this.Postconditions);
        }

        public IActionInfo Pass(Governor agent, VariableState parameters = null)
        {
            if (!this.CanPass(agent.Groups, agent.VariableState))
            {
                return ActionInfo.FailedPreconditions;
            }

            // leave the original state
            if (this.From != null)
            {
                this.From.GetInstance(agent.Workflow.InstanceId).ExitPosition(agent);
            }

            if (this.Action != null)
            {
                var actionResult = this.Action.Perform(agent, this, parameters);
                if (actionResult.IsBreak)
                {
                    return ActionInfo.Ok;
                }
                if (actionResult.IsNotOk)
                {
                    return actionResult;
                }
            }

            // apply postconditions on connection
            agent.ApplyPostconditions(this.Postconditions);

            // open connections have To set to null, so we stay n the same state
            if (this.To == null)
            {
                return ActionInfo.Ok;
            }

            // flow to input port
            return this.To.EnterAgent(agent);
        }

        //        public bool CanPass(Governor agent, params ParameterInstance[] parameters)
        //        {
        //            return this.CanPass(agent.Groups, agent.Properties);
        //        }

        public bool CanPass(Group[] groups, VariableState state)
        {
            return this.Preconditions == null ||
                   this.Preconditions.CanAccess(groups, state);
        }

        public bool CanBacktrack(Group[] groups, VariableState state)
        {
            // TODO: Also check if this is a workflow action if agent can join the workflow or create a new instance

            return this.BacktrackPreconditions == null ||
                   this.BacktrackPreconditions.CanAccess(groups, state);
        }

        //        public bool CanPassBinary(Governor agent, bool backtrack)
        //        {
        //            return this.CanPassBinary(agent.Groups, agent.Properties, backtrack);
        //        }


        public override string ToString()
        {
            return string.Format("{0} --> {1} [{2}]", this.From == null ? "open" : this.From.Id, this.To == null ? "open" : this.To.Id, this.Action);
        }

        public string ToChainString()
        {
            return string.Format("{0} [{1}]", this.To == null ? "open" : this.To.Id, this.Action);
        }

        public Connection Clone(WorkflowPosition from, WorkflowPosition to)
        {
            return new Connection(from, to, this);
        }


        public void ApplyPostconditions(Governor agent, VariableState state)
        {
            this.ApplyPostconditions(agent.Groups, state, false);
        }

        public void ApplyPostconditions(Group[] groups, VariableState state, bool planningMode)
        {
            if (Postconditions != null && Postconditions.Length > 0)
            {
                foreach (var postcondition in this.Postconditions.Where(postcondition => postcondition.AppliesTo(groups)))
                {
                    postcondition.ApplyPostconditions(state, planningMode);
                }
            }
        }

        public void ApplyBacktrackPostconditions(Group[] groups, VariableState state)
        {
            if (this.BacktrackPostconditions != null)
            {
                foreach (var postcondition in this.BacktrackPostconditions)
                {
                    // check if arc is constrained to the agent role
                    if (postcondition.AppliesTo(groups))
                    {
                        postcondition.ApplyPostconditions(state, true);
                    }
                }
            }
        }

        public void ApplyExpectedEffects(Group[] groups, VariableState state, AccessCondition effect)
        {
            // we can either apply a single effect
            // or all of them

            if (effect != null)
            {
                state.ResetDirty();
                if (effect.AppliesTo(groups))
                {
                    effect.ApplyPostconditions(state, true);
                }
            }


            if (this.ExpectedEffects != null)
            {
                state.ResetDirty();

                foreach (var postcondition in this.ExpectedEffects)
                {
                    // check if arc is constrained to the agent role
                    if (postcondition.AppliesTo(groups))
                    {
                        postcondition.ApplyPostconditions(state, true);
                    }
                }
            }
        }

        public void ApplyGeneratedBacktrackEffects(Group[] groups, VariableState state)
        {
            if (this.GeneratedNestedEffects != null)
            {
                foreach (var postcondition in this.GeneratedNestedEffects)
                {
                    // check if arc is constrained to the agent role
                    if (postcondition.AppliesTo(groups))
                    {
                        postcondition.ApplyPostconditions(state, true);
                    }
                }
            }
        }


    }
}
