
using Ei.Core.Runtime;

namespace Ei.Core.Ontology
{
    public abstract class Organisation : RelationalEntity {
        // constructor
        private SearchableState resources;

        public Organisation(string id) : base(id) { }

        public Organisation(string id, string name, string description) : base(id, name, description) {
        }

        public virtual SearchableState Resources {
            get {
                if (this.resources == null) {
                    this.resources = this.CreateState();
                }
                return this.resources;
            }
        }

        protected abstract SearchableState CreateState();

    }
}
