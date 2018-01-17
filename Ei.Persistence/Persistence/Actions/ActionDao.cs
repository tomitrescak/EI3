using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence.Actions
{
    public abstract class ActionDao : ParametricEntityDao
    {
        public virtual string ParameterParentName => "ParameterState";
        public string ParameterClassName => this.Id.Substring(0, 1).ToUpper() + this.Id.Substring(1) + "ActionParameters";
        public string ActionType { get { return this.GetType().Name; } }

        public abstract string GenerateConstructor(string holderClass);

        public virtual string GenerateParameters() { return null; }
    }
}
