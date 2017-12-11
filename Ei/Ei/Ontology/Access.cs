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

        public bool CanAccess(Governor.ResourceState agentState) {
            if (this.conditions.Count == 1) {
                return this.conditions[0].CanAccess(agentState);
            }
            return this.conditions.Any(c => c.CanAccess(agentState));
        }

        public void ApplyPostconditions(Governor.ResourceState agent, ResourceState parameters, bool planningMode = false) {
            if (this.conditions.Count == 1) {
                this.conditions[0].ApplyPostconditions(agent, parameters, planningMode);
                return;
            }
            this.conditions.Any(c => c.ApplyPostconditions(agent, parameters, planningMode));
        }

        public void ApplyPostconditions(Institution.ResourceState institutionState, Workflow.ResourceState workflowState) {
            if (this.conditions.Count == 1) {
                this.conditions[0].ApplyPostconditions(institutionState, workflowState);
                return;
            }
            this.conditions.Any(c => c.ApplyPostconditions(institutionState, workflowState));
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


        /// <summary>
        /// If organisations or roles are specified, access is limited only to them.
        /// If no organisation or roles are specified no-one can access this parameter.
        /// If organisations is "all" and roles is "all" anyone can access this parameter.
        /// </summary>
        /// <param name="agentOrganisationalRoles">Roles to check</param>
        /// <param name="agentState"></param>
        public abstract bool CanAccess(Governor.ResourceState agentState);

        public abstract bool ApplyPostconditions(Governor.ResourceState agent, ResourceState parameters, bool planningMode = false);

        public abstract bool ApplyPostconditions(Institution.ResourceState institutionState, Workflow.ResourceState workflowState);
    }


    public class AccessCondition<I, W, O, G, A> : AccessCondition
        where I : Institution.ResourceState
        where W : Workflow.ResourceState
        where O : ResourceState
        where G : ResourceState
        where A : ResourceState
    {

        public delegate bool Precondition(I institutionState, W workflowState, O organisationState, G groupState, A actionParameters = null);
        public delegate void Postcondition(I institutionState, W workflowState, O organisationState, G groupState, A actionParameters = null);

        #region class Condition
        public class Condition
        {
            private Precondition allow;
            private Precondition deny;
            private Postcondition action;

            public bool HasRuntimeParameters { get; private set; }

            internal Condition(Precondition allow, Precondition deny, Postcondition action, bool hasRuntimeParameters = false) {
                this.allow = allow;
                this.deny = deny;
                this.action = action;
                this.HasRuntimeParameters = hasRuntimeParameters;
            }

            internal bool CanAccess(Governor.ResourceState agentState) {
                if (this.allow == null && this.deny == null) {
                    return true;
                }

                if (this.allow != null) {
                    return this.CheckConditions(agentState, allow);
                }

                // check denied rolesp[\]
                if (this.deny != null) {
                    return !this.CheckConditions(agentState, deny);
                }

                return false;
            }

            internal bool Access(Governor.ResourceState agentState, ResourceState actionParameters, bool planningMode) {

                if (this.allow != null) {
                    return this.ApplyConditions(agentState, allow, actionParameters, planningMode);
                }

                // check denied rolesp[\]
                if (this.deny != null) {
                    return !this.ApplyConditions(agentState, deny, actionParameters, planningMode);
                }

                return this.ApplyConditions(agentState, null, actionParameters, planningMode);
            }

            internal bool Access(Institution.ResourceState eiState, Workflow.ResourceState workflowState) {

                if (this.allow != null) {
                    return this.ApplyConditions(eiState, workflowState, allow);
                }

                // check denied rolesp[\]
                if (this.deny != null) {
                    return !this.ApplyConditions(eiState, workflowState, deny);
                }

                return this.ApplyConditions(eiState, workflowState, null);
            }

            // private methods

            private bool ApplyConditions(Governor.ResourceState state, Precondition condition, ResourceState actionParameters, bool planningMode) {
                foreach (var group in state.Roles) {
                    if (group.Organisation is O && group.Role is G) {
                        // check first successful condition
                        if (condition == null || condition(
                                (I)state.Governor.Manager.Ei.Resources,
                                (W)state.Governor.Workflow.Resources,
                                (O)group.Organisation,
                                (G)group.Role,
                                actionParameters != null ? (A)actionParameters : null
                                )) {
                            // apply action
                            if (this.action != null && (!planningMode || !this.HasRuntimeParameters)) {
                                this.action(
                                    (I)state.Governor.Manager.Ei.Resources,
                                    (W)state.Governor.Workflow.Resources,
                                    (O)group.Organisation,
                                    (G)group.Role,
                                    actionParameters == null ? null : (A)actionParameters);
                            }

                            return true;
                        }
                    }
                }
                return false;
            }

            private bool ApplyConditions(Institution.ResourceState eiState, Workflow.ResourceState workflowState, Precondition condition) {
                // check first successful condition
                if (condition == null || condition((I)eiState, (W)workflowState, null, null, null)) {
                    // apply action
                    if (this.action != null) {
                        this.action((I)eiState, (W)workflowState, null, null, null);
                    }

                    return true;
                }
                return false;
            }

            private bool CheckConditions(Governor.ResourceState state, Precondition condition, ResourceState actionParameters = null) {
                foreach (var group in state.Roles) {
                    if (group.Organisation is O && group.Role is G) {
                        if (condition(
                            (I)state.Governor.Manager.Ei.Resources,
                            (W)state.Governor.Workflow.Resources,
                            (O)group.Organisation,
                            (G)group.Role,
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

        private List<Condition> conditions;

        // properties

        public bool IsEmpty { get { return this.conditions.Count == 0; } }

        // constructor

        public AccessCondition() {
            this.conditions = new List<Condition>();
        }

        // constructor helpers

        public AccessCondition<I, W, O, G, A> Allow(Precondition allow, Postcondition action = null) {
            this.conditions.Add(new Condition(allow, null, action));
            return this;
        }

        public AccessCondition<I, W, O, G, A> Action(Postcondition action = null) {
            this.conditions.Add(new Condition(null, null, action));
            return this;
        }

        public AccessCondition<I, W, O, G, A> Deny(Precondition deny, Postcondition action = null) {
            this.conditions.Add(new Condition(null, deny, action));
            return this;
        }

        // postconditions

        public override bool ApplyPostconditions(Governor.ResourceState agentState, ResourceState actionParameters, bool planningMode) {
            if (this.conditions.Count == 0) {
                return false;
            }
            return this.conditions.Any(c => c.Access(agentState, actionParameters, planningMode));
        }

        public override bool ApplyPostconditions(Institution.ResourceState institutionState, Workflow.ResourceState workflowState) {
            if (this.conditions.Count == 0) {
                return false;
            }
            return this.conditions.Any(c => c.Access(institutionState, workflowState));
        }

        // general access

        public override bool CanAccess(Governor.ResourceState agentState) {
            if (this.conditions.Count == 0) {
                return true;
            }
            return this.conditions.Any(c => c.CanAccess(agentState));
        }


    }

    public class AccessCondition<I, W> : AccessCondition<I, W, ResourceState, ResourceState, ResourceState>
        where I : Institution.ResourceState
        where W : Workflow.ResourceState
    {

    }
}
