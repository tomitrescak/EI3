namespace Ei.Core.Ontology
{
    public enum OrganisationRelationType
    {
        Parent,
        Child,
        Inclusion,
        Exclusion,
        Dynamic
    }

    public class OrganisationRelation
    {
        public Organisation Relation { get; }
        public string Name { get; }
        public float Value { get; set; }

        public OrganisationRelation(Organisation relation, string name = null, float defaultValue = 0)
        {
            this.Relation = relation;
            this.Name = name;
            this.Value = defaultValue;
        }
    }
}
