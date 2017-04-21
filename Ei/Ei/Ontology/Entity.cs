namespace Ei.Ontology
{ 
    public class Entity 
    {
        public string Id { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        protected Entity() { }

        protected Entity(string id)
        {
            this.Id = id;
        }

        protected Entity(string id, string name, string description) : this(id)
        {
            this.Name = name;
            this.Description = description;
        }

        protected Entity(Entity entity): this(entity.Id, entity.Name, entity.Description)
        {
        }
    }
}
