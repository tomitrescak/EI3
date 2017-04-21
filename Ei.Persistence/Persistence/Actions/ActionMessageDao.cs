namespace Ei.Persistence.Actions
{
    public class ActionMessageDao : ActionDao
    {
        public GroupDao[] NotifyGroups { get; set; }
        public string[] NotifyAgents { get; set; }
    }
}
