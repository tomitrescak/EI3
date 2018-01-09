
using System;
using System.Linq;
using Ei.Runtime;

namespace Ei.Ontology
{


    public abstract class Role : RelationalEntity
    {
        private ResourceState resources;

        public Role(string id): base(id) { }

        public abstract ResourceState CreateState();

        public virtual ResourceState Resources {
            get {
                if (this.resources == null) {
                    this.resources = this.CreateState();
                }
                return this.resources;
            }
        }
    }
}
