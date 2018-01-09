namespace Ei.Persistence
{
    public class StateDao : EntityDao
    {
        public bool Open { get; set; }

        public AccessConditionDao[] AllowEntry { get; set; }
        public AccessConditionDao[] DenyEntry { get; set; }

        public AccessConditionDao[] AllowExit { get; set; }
        public AccessConditionDao[] DenyExit { get; set; }
        
        public int Timeout { get; set; }
    }
}
