namespace Ei.Ontology
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Ei.Runtime;
    using System;

    public class Access
    {
        // static methods

        public static bool IsInGroup(Group checkRole, IEnumerable<Group> allowRoles) {
            return allowRoles.Any(allowedOrgRole =>
                (allowedOrgRole.Organisation == null || (checkRole.Organisation != null && checkRole.Organisation.Is(allowedOrgRole.Organisation))) &&
                (allowedOrgRole.Role == null || (checkRole.Role != null && checkRole.Role.Is(allowedOrgRole.Role))));
        }

        // fields

        private List<AccessCondition> conditions;

        // ctor

        public Access() {
            this.conditions = new List<AccessCondition>();
        }

        // properties

        public virtual bool HasAgentParameters { get { return this.conditions.Any(c => c.HasAgentParameters); } }
        public virtual bool HasActivityParameters { get { return this.conditions.Any(c => c.HasActivityParameters); } }
        public virtual bool HasRuntimeParameters { get { return this.conditions.Any(c => c.HasRuntimeParameters); } }

        public bool IsEmpty { get { return this.conditions.Count == 0; } }

        public bool HasPreconditions { get { return this.conditions.Any(c => c.HasPreconditions); } }

        public bool CanAccess(Governor.GovernorState agentState, Workflow.Store workflow, ParameterState parameters = null) {
            if (this.conditions.Count == 1) {
                return this.conditions[0].CanAccess(agentState, workflow, parameters);
            }
            return this.conditions.Any(c => c.CanAccess(agentState, workflow, parameters));
        }

        public void ApplyPostconditions(Governor.GovernorState agent, Workflow.Store workflowState, ParameterState parameters, bool planningMode = false) {
            if (this.conditions.Count == 1) {
                this.conditions[0].ApplyPostconditions(agent, workflowState, parameters, planningMode);
                return;
            }
            this.conditions.ForEach(c => c.ApplyPostconditions(agent, workflowState, parameters, planningMode));
        }

        public void ApplyPostconditions(Institution.InstitutionState institutionState, Workflow.Store workflowState) {
            if (this.conditions.Count == 1) {
                this.conditions[0].ApplyPostconditions(institutionState, workflowState);
                return;
            }
            this.conditions.ForEach(c => c.ApplyPostconditions(institutionState, workflowState));
        }

        public Access Add(AccessCondition condition) {
            this.conditions.Add(condition);
            return this;
        }
    }

    public abstract class AccessCondition
    {
        // properties

        public virtual bool HasAgentParameters { get { return false; } }
        public virtual bool HasActivityParameters { get { return false; } }
        public virtual bool HasRuntimeParameters { get { return false; } }

        public abstract bool HasPreconditions { get; }


        /// <summary>
        /// If organisations or roles are specified, access is limited only to them.
        /// If no organisation or roles are specified no-one can access this parameter.
        /// If organisations is "all" and roles is "all" anyone can access this parameter.
        /// </summary>
        /// <param name="agentOrganisationalRoles">Roles to check</param>
        /// <param name="agentState"></param>
        public abstract bool CanAccess(Governor.GovernorState agentState, Workflow.Store workflowState, ParameterState parameters);

        public abstract void ApplyPostconditions(Governor.GovernorState agent, Workflow.Store workflowState, ParameterState parameters, bool planningMode = false);

        public abstract void ApplyPostconditions(Institution.InstitutionState institutionState, Workflow.Store workflowState);
    }


    public class AccessCondition<I, W, O, R, A> : AccessCondition
        where I : Institution.InstitutionState
        where W : Workflow.Store
        where O : SearchableState
        where R : SearchableState
        where A : ParameterState
    {

        public delegate bool Precondition(I institutionState, W workflowState, Governor.GovernorState governorState, O organisationState, R roleState, A actionParameters = null);
        public delegate void Postcondition(I institutionState, W workflowState, Governor.GovernorState governorState, O organisationState, R groupState, A actionParameters = null);

        #region class Condition
        public class Condition
        {
            private Precondition condition;

            public bool HasRuntimeParameters { get; private set; }

            public bool HasPreconditions { get { return this.condition != null; } }

            internal Condition(Precondition allow, bool hasRuntimeParameters = false) {
                this.condition = allow;
                this.HasRuntimeParameters = hasRuntimeParameters;
            }

            public bool CanAccess(Governor.GovernorState state, Workflow.Store workflowState, ParameterState actionParameters = null) {
                if (this.condition == null) {
                    return true;
                }


                foreach (var group in state.Groups) {
                    if (group.Organisation is O && group.Role is R) {
                        if (condition(
                            (I)state.Governor.Manager.Ei.Resources,
                            (W)workflowState,
                            state,
                            (O)group.Organisation,
                            (R)group.Role,
                            actionParameters != null ? (A)actionParameters : null
                            )) {

                            return true;
                        }
                    }
                }
                return false;
            }
        }
        #endregion

        #region class ConditionalAction
        public class ConditionalAction
        {
            private Precondition condition;
            private Postcondition action;

            public bool HasRuntimeParameters { get; private set; }

            public bool HasPreconditions { get { return this.condition != null; } }

            internal ConditionalAction(Precondition allow, Postcondition action = null, bool hasRuntimeParameters = false) {
                this.condition = allow;
                this.action = action;
                this.HasRuntimeParameters = hasRuntimeParameters;
            }

            internal bool Access(Governor.GovernorState agentState, Workflow.Store workflowState, ParameterState actionParameters, bool planningMode) {

                if (this.condition != null) {
                    return this.ApplyConditions(agentState, workflowState, condition, actionParameters, planningMode);
                }

                return this.ApplyConditions(agentState, workflowState, null, actionParameters, planningMode);
            }

            internal bool Access(Institution.InstitutionState eiState, Workflow.Store workflowState) {

                if (this.condition != null) {
                    return this.ApplyConditions(eiState, workflowState, condition);
                }

                return this.ApplyConditions(eiState, workflowState, null);
            }

            // private methods

            private bool ApplyConditions(Governor.GovernorState state, Workflow.Store workflowState, Precondition condition, ParameterState actionParameters, bool planningMode) {
                foreach (var group in state.Groups) {
                    if (group.Organisation is O && group.Role is R) {
                        // check first successful condition
                        if (condition == null || condition(
                                (I)state.Governor.Manager.Ei.Resources,
                                (W)workflowState,
                                state,
                                (O)group.Organisation,
                                (R)group.Role,
                                actionParameters != null ? (A)actionParameters : null
                                )) {
                            // apply action
                            if (this.action != null && (!planningMode || !this.HasRuntimeParameters)) {
                                this.action(
                                    (I)state.Governor.Manager.Ei.Resources,
                                    (W) workflowState,
                                    state,
                                    (O)group.Organisation,
                                    (R)group.Role,
                                    actionParameters == null ? null : (A)actionParameters);
                            }

                            return true;
                        }
                    }
                }
                return false;
            }

            private bool ApplyConditions(Institution.InstitutionState eiState, Workflow.Store workflowState, Precondition condition) {
                // check first successful condition
                if (condition == null || condition((I)eiState, (W)workflowState, null, null, null)) {
                    // apply action
                    this.action?.Invoke((I)eiState, (W)workflowState, null, null, null);

                    return true;
                }
                return false;
            }
        }
        #endregion

        private List<Condition> preConditions;
        private List<ConditionalAction> postConditions;

        // properties

        public bool IsEmpty { get { return this.postConditions.Count == 0; } }

        public override bool HasPreconditions { get { return this.postConditions.Any(c => c.HasPreconditions); } }

        // constructor

        public AccessCondition() {
        }

        // constructor helpers

        public AccessCondition<I, W, O, R, A> Allow(Precondition allow) {
            if (this.preConditions == null) {
                this.preConditions = new List<Condition>();
            }

            this.preConditions.Add(new Condition(allow));
            return this;
        }

        public AccessCondition<I, W, O, R, A> Action(Postcondition action = null) {
            if (this.postConditions == null) {
                this.postConditions = new List<ConditionalAction>();
            }
            this.postConditions.Add(new ConditionalAction(null, action));
            return this;
        }

        public AccessCondition<I, W, O, R, A> Action(Precondition allow, Postcondition action = null) {
            if (this.postConditions == null) {
                this.postConditions = new List<ConditionalAction>();
            }
            this.postConditions.Add(new ConditionalAction(allow, action));
            return this;
        }

        // postconditions

        public override void ApplyPostconditions(Governor.GovernorState agentState, Workflow.Store workflowState, ParameterState actionParameters, bool planningMode) {
            if (this.postConditions != null) {
                this.postConditions.ForEach(c => c.Access(agentState, workflowState, actionParameters, planningMode));
            }
        }

        public override void ApplyPostconditions(Institution.InstitutionState institutionState, Workflow.Store workflowState) {
            if (this.postConditions != null) {
                this.postConditions.ForEach(c => c.Access(institutionState, workflowState));
            }
        }

        // general access

        public override bool CanAccess(Governor.GovernorState agentState, Workflow.Store workflowState, ParameterState parameters) {
            if (this.preConditions == null) {
                return true;
            }
            return this.preConditions.Any(c => c.CanAccess(agentState, workflowState, parameters));
        }


 

    }

    public class AccessCondition<I, W> : AccessCondition<I, W, SearchableState, SearchableState, ParameterState>
        where I : Institution.InstitutionState
        where W : Workflow.Store
    {

    }
}
