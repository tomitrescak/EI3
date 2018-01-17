using Ei.Persistence.Templates;

namespace Ei.Persistence.Actions
{
    public class ActionJoinWorkflowDao : ActionDao
    {
        public string WorkflowId { get; set; }

        public override string ParameterParentName => "ActionJoinWorkflow.Parameters";

        public override string GenerateConstructor(string holderClass) {
            return $"new ActionJoinWorkflow(\"{Id}\", ei, \"{WorkflowId}\", () => new {holderClass}.{ParameterClassName}())";
        }

        public override string GenerateParameters(string workflowClassName) {
            this.WorkflowClassName = workflowClassName;

            return CodeGenerator.Parameters(this);
        }
    }
}
