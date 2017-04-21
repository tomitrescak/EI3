namespace Ei.Persistence
{
    public struct RelationDao
    {
        public string Type { get; set; }

        public string[] Relation { get; set; }

        public string Name { get; set; }

        public float DefaultValue { get; set; }
    }
}
