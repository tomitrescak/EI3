namespace Ei.Persistence
{
    public class EntityDao : IGenerator
    {
        public virtual string ClassName => this.Name.ToId();

        public virtual string ParentClassName => null;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual string GenerateCode() { throw new System.NotImplementedException("This entity does not know how to generate code: " + this.Id + " " + this.Name); }
    }
}
