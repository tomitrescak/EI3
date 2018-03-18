namespace Ei.Persistence.Actions
{
    public class ActionStartAgentDao : ActionDao
    {
        public override string GenerateConstructor(string holderClass) {
            return $"new ActionStartAgent(\"{Id}\", ei)";
        }
    }
}
