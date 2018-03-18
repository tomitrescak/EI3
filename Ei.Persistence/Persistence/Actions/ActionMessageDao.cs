using System.Linq;
using Ei.Persistence.Templates;

namespace Ei.Persistence.Actions
{
    public class ActionMessageDao : ActionDao
    {
        public GroupDao[] NotifyGroups { get; set; }
        public string[] NotifyAgents { get; set; }

        public override string GenerateConstructor(string holderClass) {
            string groups = "null";
            if (this.NotifyGroups != null && this.NotifyGroups.Length > 0) {
                groups = $"new Group[] {{ { string.Join(",", this.NotifyGroups.Select(g => $"ei.GroupById(\"{g.OrganisationId}\", \"{g.RoleId}\")" )) } }}"; 
            }

            string agents = "null";
            if (this.NotifyAgents != null && this.NotifyAgents.Length > 0) {
                agents = string.Join(", ", agents.Select(a => "\"" + a + "\""));
            }
            return $"new ActionMessage(\"{Id}\", ei, () => new {holderClass}.{ParameterClassName}(), { groups }, { agents })";
        }

        public override string GenerateParameters(string workflowClassName) {
            return CodeGenerator.Parameters(this);
        }


    }
}
