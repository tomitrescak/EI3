namespace Ei.Persistence.Transitions
{
    public class TransitionSplitDao : TransitionDao
    {
        public bool Shallow { get; set; }
        public string[][] Names { get; set; }
    }
}
