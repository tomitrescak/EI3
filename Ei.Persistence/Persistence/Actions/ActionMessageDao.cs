using Ei.Persistence.Templates;

namespace Ei.Persistence.Actions
{
    public class ActionMessageDao : ActionDao
    {
        public GroupDao[] NotifyGroups { get; set; }
        public string[] NotifyAgents { get; set; }

        public override string GenerateConstructor(string holderClass) {
            return $"new ActionMessage(\"{Id}\", ei, () => new {holderClass}.{ParameterClassName}())";
        }

        public override string GenerateParameters(string workflowClassName) {
            return CodeGenerator.Parameters(this);
        }


    }
}
