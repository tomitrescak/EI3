using Ei.Persistence.Templates;

namespace Ei.Persistence.Actions
{
    public class ActionJoinWorkflowDao : ActionDao
    {
        public string WorkflowId { get; set; }

        public override string ParameterParentName => "ActionJoinWorkflow.Parameters";

        public override string GenerateConstructor(string holderClass) {
            return $"new ActionJoinWorkflow(\"{Id}\", ei, \"{WorkflowId}\")";
        }

        public override string GenerateParameters() {
            return CodeGenerator.Parameters(this);
        }
    }
}
