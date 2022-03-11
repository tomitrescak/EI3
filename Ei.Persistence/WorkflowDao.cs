﻿
using Newtonsoft.Json;

namespace Ei.Persistence
{
    using Ei.Persistence.Actions;
    using Ei.Persistence.Templates;
    using Ei.Persistence.Transitions;

    public class WorkflowDao : ParametricEntityDao
    {
        [JsonIgnore]
        public override string ClassName => this.Name.ToId() + "Workflow";
        
        public string Import { get; set; }

        public bool Stateless { get; set; }

        public bool Static { get; set; }

        public StateDao[] States { get; set; }

        public ActionDao[] Actions { get; set; }

        public TransitionDao[] Transitions { get; set; }

        public ConnectionDao[] Connections { get; set; }

        public AccessConditionDao[] AllowCreate { get; set; }

        public AccessConditionDao[] AllowJoin { get; set; }

        public override string GenerateCode() {
            return CodeGenerator.Workflow(this);
        }

    }
}