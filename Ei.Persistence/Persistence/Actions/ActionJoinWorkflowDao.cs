using Ei.Persistence.Templates;
using Newtonsoft.Json;

namespace Ei.Persistence.Actions
{
    public class ActionJoinWorkflowDao : ActionDao
    {
        public string WorkflowId { get; set; }

        [JsonIgnore]
        public override string ParameterParentName => "ActionJoinWorkflow.Parameters";

        // methods
        
        public override string GenerateConstructor(string holderClass) {
            return $"new ActionJoinWorkflow(\"{Id}\", ei, \"{WorkflowId}\", () => new {holderClass}.{ParameterClassName}())";
        }

        public override string GenerateParameters(string workflowClassName) {
            this.WorkflowClassName = workflowClassName;

            return CodeGenerator.Parameters(this);
        }
    }
}
