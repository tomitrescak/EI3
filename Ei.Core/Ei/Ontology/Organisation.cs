
using Ei.Runtime;

namespace Ei.Ontology
{
    public abstract class Organisation : RelationalEntity {
        // constructor
        private ResourceState resources;

        public Organisation(string id) : base(id) { }

        public Organisation(string id, string name, string description) : base(id, name, description) {
        }

        public virtual ResourceState Resources {
            get {
                if (this.resources == null) {
                    this.resources = this.CreateState();
                }
                return this.resources;
            }
        }

        protected abstract ResourceState CreateState();

    }
}
