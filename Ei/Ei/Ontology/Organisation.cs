
using Ei.Runtime;

namespace Ei.Ontology
{
    public class Organisation: RelationalEntity
    {
        // constructor

        public Organisation(string id) : base(id) { }

        public Organisation(string id, string name, string description) : base(id, name, description)
        {
        }

    }
}
