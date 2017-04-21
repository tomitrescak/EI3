using Ei.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Ontology.Actions
{
    public abstract class ActionParameters: VariableState
    {
        public abstract void Parse(ActionParameter1[] parameters);
        public abstract void Apply(VariableState institutionState, VariableState workflowState, VariableState agentState);
    }
}
