namespace Ei.Persistence.Actions
{
    public class StartWorkflowPropertyDao
    {
        public string Name { get; set; }
        public string Value { get; set; }    
    }


    public class ActionStartWorkflowDao : ActionDao
    {
        public string Path { get; set; }

        public override string GenerateConstructor(string holderClass) {
            return $"new ActionStartWorkflow(\"{Id}\", ei)";
        }
    }
}
