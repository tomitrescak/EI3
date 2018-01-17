using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence.Actions
{
    public abstract class ActionDao : ParametricEntityDao
    {
        [JsonIgnore]
        public virtual string ParameterParentName => "ParameterState";

        [JsonIgnore]
        public string ParameterClassName => this.Id.Substring(0, 1).ToUpper() + this.Id.Substring(1) + "ActionParameters";

        [JsonIgnore]
        public string WorkflowClassName { get; set; }

        [JsonIgnore]
        public string ActionType { get { return this.GetType().Name; } }

        // methods

        public abstract string GenerateConstructor(string holderClass);

        public virtual string GenerateParameters(string workflowClassName) { return null; }
    }
}
