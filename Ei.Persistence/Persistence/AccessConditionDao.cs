namespace Ei.Persistence
{
    public struct PostconditionDao
    {
        public string Condition { get; set; }
        public string Action { get; set; }
    }

    public struct AccessConditionDao {
        public string Role { get; set; }
        public string Organisation { get; set; }
        public string Precondition { get; set; }
        public PostconditionDao[] Postconditions { get; set; }
    }
}
