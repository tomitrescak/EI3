using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ei.Runtime;

namespace Ei.Ontology.Transitions
{
    public abstract class Transition : WorkflowPosition
    {
        
        protected Transition(string id, string name, string description, Workflow workflow) : base(id, name, description, workflow) {
        }

        protected abstract IActionInfo Perform(Governor governor);

        public override IActionInfo EnterAgent(Governor agent)
        {

            // make agent enter int ocurrent state
            base.EnterAgent(agent);

            // perform transition
            var result = this.Perform(agent);

            // try to continue
            if (result.IsOk)
            {
                return agent.Continue();
            }

            // return current result
            return result;
        }
    }
}
