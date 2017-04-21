using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Ontology.Transitions
{
    public abstract class SimpleTransition : Transition
    {
        protected SimpleTransition(string id, string name, string description, Workflow workflow) : base(id, name, description, workflow)
        {
        }

        public Connection In { get { return this.Ins[0]; } }
        public Connection Out { get { return this.Outs[0]; } }
    }
}
