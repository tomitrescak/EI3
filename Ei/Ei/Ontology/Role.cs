
using System;
using System.Linq;
using Ei.Runtime;

namespace Ei.Ontology
{


    public abstract class Role : RelationalEntity
    {
        public Role(string id): base(id) { }

        public abstract ResourceState CreateState();
    }
}
