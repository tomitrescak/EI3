namespace Ei.Persistence.Actions
{
    public class ActionJoinWorkflowDao : ActionDao
    {
        public string WorkflowId { get; set; }

        public GroupDao[] CreatedBy { get; set; }
    }
}
