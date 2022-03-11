using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Runtime;
using static Ei.Core.Ontology.Institution;

namespace Ei.Core.Ontology.Actions
{
    public abstract class ActionWithParameters : ActionBase
    {
        protected ActionWithParameters(Institution ei, string id, ParameterState parameters) : base(ei, id) {
           
        }
    }
}
