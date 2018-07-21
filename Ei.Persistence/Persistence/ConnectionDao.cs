namespace Ei.Persistence
{
    public class ConnectionDao
    {
        public string[] Join { get; set; }

        //public bool Stop { get; set; }
        public string Id { get; set; }
        public string Import { get; set; }

        public AccessConditionDao[] Allow { get; set; }
        public AccessConditionDao[] Deny { get; set; }
        public AccessConditionDao[] Postconditions { get; set; }
        public AccessConditionDao[] Effects { get; set; }

        public BacktrackDao Backtrack { get; set; }

        public AccessConditionDao[] GeneratedNestedEffects { get; set; }

        public string ActionId { get; set; }

        public int AllowLoops { get; set; }

        public override string ToString()
        {
            if (Join.Length == 2)
            {
                return string.Format("{0} -> {1}", Join[0], Join[1]);
            }
            return string.Format("{0} [{1}] -> {2} [{3}]", Join[0], Join[1], Join[2], Join[3]);
        }
    }
}
