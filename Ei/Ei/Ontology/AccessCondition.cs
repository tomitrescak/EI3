
namespace Ei.Ontology
{
    using Ei.Runtime;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class AccessCondition
    {
        public virtual bool HasActivityParameter { get { return false; } }
        public virtual bool HasAgentParameter { get { return false; } }
        public virtual bool IsRuntimeExpression { get { return false; } }

        // static methods

        public static bool IsInGroup(Group checkRole, Group allowGroup) {
            return (allowGroup.Organisation == null || (checkRole.Organisation != null && checkRole.Organisation.Is(allowGroup.Organisation))) &&
                (allowGroup.Role == null || (checkRole.Role != null && checkRole.Role.Is(allowGroup.Role)));
        }

        public static bool IsInGroup(Group checkRole, IEnumerable<Group> allowGroups) {
            return allowGroups.Any(allowedOrgRole =>
                (allowedOrgRole.Organisation == null || (checkRole.Organisation != null && checkRole.Organisation.Is(allowedOrgRole.Organisation))) &&
                (allowedOrgRole.Role == null || (checkRole.Role != null && checkRole.Role.Is(allowedOrgRole.Role))));
        }

        public static bool CheckHasAgentParameters(IEnumerable<AccessCondition> checkConditions) {
            return checkConditions != null && checkConditions.Any(c => c.HasAgentParameter);
        }

        public static bool CheckHasActivityParameters(IEnumerable<AccessCondition> checkConditions) {
            return checkConditions != null && checkConditions.Any(c => c.HasActivityParameter);
        }
    }

    /// <summary>
    /// Contains a list of applicable organisation roles and a list of string conditions 
    /// </summary>
    public abstract class AccessCondition<I, W, G> : AccessCondition
        where I : VariableState 
        where W : VariableState
        where G : VariableState
    {
        // EXPRESSIONS

        public virtual bool CheckConditions(I institutionState, W workflowState, G agentState) { return true; }

        public virtual void CheckPostconditions(I institutionState, W workflowState, G agentState) { }

        public void ApplyPostconditions(I institutionState, W workflowState, G agentState, bool planningMode) {
            // we do not consider runtime expressions
            // runtime expressions contain function parameters, owners ...
            if (planningMode && this.IsRuntimeExpression) {
                return;
            }

            // consider locking
            this.CheckPostconditions(institutionState, workflowState, agentState);
        }

        
    }
}
