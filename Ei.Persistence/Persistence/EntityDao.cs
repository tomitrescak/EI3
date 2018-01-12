namespace Ei.Persistence
{
    public class EntityDao
    {
        public virtual string ClassName => this.Name.ToId();

        public virtual string ParentClassName => null;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
