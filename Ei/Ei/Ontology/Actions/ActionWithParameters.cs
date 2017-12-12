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
        private Runtime.ResourceState parameters;

        protected ActionWithParameters(Institution ei, string id, Runtime.ResourceState parameters) : base(ei, id) {
            this.parameters = parameters;
        }

        public override Runtime.ResourceState ParseParameters(VariableInstance[] properties) {
            if (this.parameters == null) {
                return null;
            }
            return parameters.Clone().Parse(properties);
        }
    }
}
