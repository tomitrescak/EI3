
namespace Ei.Ontology
{
    using Ei.Runtime;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains a list of applicable organisation roles and a list of string conditions 
    /// </summary>
    public abstract class AccessCondition
    {
        // fields
        private readonly Expression expression;
        private readonly Institution ei;
        private readonly Group group;

        private System.Object lockThis = new System.Object();

        // properties

        public bool HasActivityParameter { get { return this.expression != null && this.expression.HasActivityParameters; } }
        public bool HasAgentParameter { get { return this.expression != null && this.expression.HasAgentParameters; } }

        // ctor

        public AccessCondition(Institution ei, Group group, Expression expression) {
            this.ei = ei;
            this.group = group;
            this.expression = expression;
        }


        // EXPRESSIONS

        public abstract bool CheckConditions(VariableState state);

        public void ApplyPostconditions(VariableState state, bool planningMode) {
            // we do not consider runtime expressions
            // runtime expressions contain function parameters, owners ...
            if (planningMode && this.expression.IsRuntimeExpression) {
                return;
            }

            // consider locking
            lock (lockThis) {
                this.expression.Evaluate(state, planningMode);
            }
        }

        /// <summary>
        /// Checks whether this Access condition applies to agent with give groups
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public bool AppliesTo(IEnumerable<Group> groups) {
            return this.group == null ||
                groups != null &&
                groups.Any(agentRole => IsInGroup(agentRole, this.group));
        }

        public override string ToString() {
            return (this.group == null
                        ? string.Empty
                        : this.group.ToString()) + " " +
                   (this.expression == null
                        ? string.Empty
                        : this.expression.ToString());
        }

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
}
