
//namespace Ei.Ontology
//{
//    using Ei.Runtime;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    public abstract class AccessCondition
//    {
//        public virtual bool HasActivityParameter { get { return false; } }
//        public virtual bool HasAgentParameter { get { return false; } }
//        public virtual bool IsRuntimeExpression { get { return false; } }

//        // abstract methods

//        public virtual bool CheckConditions(Governor.GovernorVariableState agent) { return true; }
//        public virtual void ApplyPostconditions(Governor.GovernorVariableState agent, bool testMode) { }
//        public virtual void ApplyPostconditions(VariableState ei, Workflow.WorkflowVariableState state) { }

//        // static methods

//        public static bool CheckHasAgentParameters(IEnumerable<AccessCondition> checkConditions) {
//            return checkConditions != null && checkConditions.Any(c => c.HasAgentParameter);
//        }

//        public static bool CheckHasActivityParameters(IEnumerable<AccessCondition> checkConditions) {
//            return checkConditions != null && checkConditions.Any(c => c.HasActivityParameter);
//        }
//    }

//    public abstract class AccessCondition<I, W> : AccessCondition
//        where I : Institution.InstitutionState
//        where W : Workflow.WorkflowVariableState
//    {
//        public override void ApplyPostconditions(VariableState ei, Workflow.WorkflowVariableState state) {
//            this.CheckPostconditions((I)ei, (W)state);
//        }
//        public abstract void CheckPostconditions(I institutionState, W workflowState);
//    }

//    /// <summary>
//    /// Contains a list of applicable organisation roles and a list of string conditions 
//    /// </summary>
//    public abstract class AccessCondition<I, W, O, G> : AccessCondition
//        where I : Institution.InstitutionState
//        where W : Workflow.WorkflowVariableState
//        where O : VariableState
//        where G : VariableState
//    {
//        // EXPRESSIONS
//        public override bool CheckConditions(Governor.GovernorVariableState state) {
//            var result = true;
//            foreach (var group in state.Roles) {
//                if (group.Organisation is O && group.Role is G) {
//                    if (this.CheckConditions(
//                        (I)state.Governor.Manager.Ei.VariableState,
//                        (W)state.Governor.Workflow.VariableState,
//                        (O)group.Organisation,
//                        (G)group.Role)) {

//                        return true;
//                    }
//                    else {
//                        // we still check the rest of the conditions
//                        result = false;
//                    }
//                }
//            }
//            return result;
//        }

//        public virtual bool CheckConditions(I institutionState, W workflowState, O organisationState, G roleState) { return true; }

//        public virtual void CheckPostconditions(I institutionState, W workflowState, O organisationState, G roleState) { }

//        public override void ApplyPostconditions(Governor.GovernorVariableState agent, bool planningMode) {
//            foreach (var group in agent.Roles) {
//                if (group.Organisation is O && group.Role is G) {
//                    this.ApplyPostconditions(
//                        (I) agent.Governor.Manager.Ei.VariableState,
//                        (W)agent.Governor.Workflow.VariableState,
//                        (O)group.Organisation,
//                        (G)group.Role,
//                        planningMode);
//                }
//            }
//        }

//        public void ApplyPostconditions(I institutionState, W workflowState, O organisationState, G roleState, bool planningMode) {
//            // we do not consider runtime expressions
//            // runtime expressions contain function parameters, owners ...
//            if (planningMode && this.IsRuntimeExpression) {
//                return;
//            }

//            // consider locking
//            this.CheckPostconditions(institutionState, workflowState, organisationState, roleState);
//        }


//    }
//}
