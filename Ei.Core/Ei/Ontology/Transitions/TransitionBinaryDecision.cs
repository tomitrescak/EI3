

using Ei.Core.Ontology;

namespace Ei.Core.Ontology.Transitions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Core.Runtime;

    public class TransitionBinaryDecision : Transition
    {
        // properties

        public Access Decision { get; set; }

        // constructor

        public TransitionBinaryDecision(string id, string name, string description, Access decisions, Workflow workflow)
            : base(id, name, description, workflow)
        {
            this.Decision = decisions;
        }

        // methods

        public void Validate()
        {
            // can have only two output ports!!!
        }

        protected override IActionInfo Perform(Governor governor)
        {
            throw new System.NotImplementedException();
        }

    }
}
