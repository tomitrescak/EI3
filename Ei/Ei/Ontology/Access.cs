namespace Ei.Ontology
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Ei.Runtime;
    using System;

    public class Access
    {
        // fields

        private readonly ReadOnlyCollection<AccessCondition> allow;
        private readonly ReadOnlyCollection<AccessCondition> deny;

        // properties

        public bool IsEmpty { get { return this.allow == null && this.deny == null; } }
        public bool HasActivityParameter {
            get {
                return AccessCondition.CheckHasActivityParameters(this.allow) ||
                       AccessCondition.CheckHasActivityParameters(this.deny);
            }
        }
        public bool HasAgentParameter {
            get {
                return AccessCondition.CheckHasAgentParameters(this.allow) ||
                       AccessCondition.CheckHasAgentParameters(this.deny);
            }
        }

        // constructor

        public Access(Institution ei, AccessCondition[] allow, AccessCondition[] deny)
        {
            if (allow != null)
            {
                this.allow = Array.AsReadOnly(allow);
            }

            if (deny != null)
            {
                this.deny = Array.AsReadOnly(deny);
            }
        }

        /// <summary>
        /// If organisations or roles are specified, access is limited only to them.
        /// If no organisation or roles are specified no-one can access this parameter.
        /// If organisations is "all" and roles is "all" anyone can access this parameter.
        /// </summary>
        /// <param name="agentOrganisationalRoles">Roles to check</param>
        /// <param name="agentState"></param>
        public bool CanAccess(Group[] agentOrganisationalRoles, VariableState agentState)
        {
            if (this.allow == null && this.deny == null)
            {
                return true;
            }

            if (this.allow != null && this.allow.Count > 0)
            {
                return this.allow.Any(condition => condition.AppliesTo(agentOrganisationalRoles) && condition.CheckConditions(agentState));
            }

            // check denied roles
            if (this.deny != null && this.deny.Count > 0)
            {
                return this.deny.All(condition => !condition.AppliesTo(agentOrganisationalRoles) || !condition.CheckConditions(agentState));
            }

            return false;
        }

        public override string ToString()
        {
            if (this.IsEmpty) return null;

            var result = string.Empty;
            if (this.allow != null)
            {
                result +=  "Allow: " + string.Join("; ", this.allow.Select(w => w.ToString()).ToArray());
            }
            
            if (this.deny != null)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "\n";
                }
                result += "Deny: " + string.Join("; ", this.deny.Select(w => w.ToString()).ToArray());
            }
            return result;
        }
    }

    
}
