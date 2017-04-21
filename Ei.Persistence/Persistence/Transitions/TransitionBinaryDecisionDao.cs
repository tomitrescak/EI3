namespace Ei.Persistence.Transitions
{
    public class TransitionBinaryDecisionDao : TransitionDao
    {
        public AccessConditionDao[] Decision { get; set; }
    }
}
