using Ei.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Ei.Ontology.Institution;

namespace Ei.Ontology.Actions
{
    public abstract class ActionWithParameters : ActionBase
    {
        private ParameterState parameters;

        protected ActionWithParameters(Institution ei, string id, ParameterState parameters) : base(ei, id) {
            this.parameters = parameters;
        }

        public override ParameterState ParseParameters(VariableInstance[] properties) {
            if (this.parameters == null) {
                return null;
            }
            return parameters.Parse(properties);
        }
    }
}
