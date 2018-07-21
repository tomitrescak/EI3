using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ei.Runtime;

namespace Ei.Ontology.Actions
{
    public abstract class ActionMessage: ActionBase
    {
        private VariableState parameters;

        // constructor

        public ActionMessage(string id, Institution ei, Group[] notifyGroups, List<string> notifyAgents)
            : base(ei, id)
        {
            if (notifyGroups != null) {
                this.NotifyRoles = new ReadOnlyCollection<Group>(notifyGroups);
            }
            if (notifyAgents != null) {
                this.NotifyAgents = new ReadOnlyCollection<string>(notifyAgents);
            }  
        }

        #region Properties
        public ReadOnlyCollection<Group> NotifyRoles { get; }

        public ReadOnlyCollection<string> NotifyAgents { get; }

        protected override IActionInfo PerformAction(Governor performer, Connection connection, VariableState parameters)
        {
            this.parameters = parameters;
            return ActionInfo.Ok;
        }

        protected override void Performed(Governor agent)
        {
            agent.LogAction(
                InstitutionCodes.ActionPerformed, 
                agent.Name, 
                this.Id, 
                this.parameters.ToString());

            // notify roles
            if (this.NotifyRoles != null && this.NotifyRoles.Count > 0)
            {
                agent.Workflow.NotifyRoles(this.NotifyRoles, agent.Name, this.Id, parameters);
            }
            // notify agents
            if (this.NotifyAgents != null && this.NotifyAgents.Count > 0)
            {
                agent.Workflow.NotifyAgents(this.NotifyAgents, this.Name, parameters);
            }
        }

        #endregion
    }
}
