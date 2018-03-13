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
        protected ActionWithParameters(Institution ei, string id, ParameterState parameters) : base(ei, id) {
           
        }
    }
}
