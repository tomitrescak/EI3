
using Ei.Runtime;

namespace Ei.Ontology
{
    public abstract class Organisation: RelationalEntity
    {
        // constructor

        public Organisation(string id) : base(id) { }

        public Organisation(string id, string name, string description) : base(id, name, description)
        {
        }

        public abstract VariableState CreateState();

    }
}
