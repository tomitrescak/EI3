using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Persistence.Templates;

namespace Ei.Persistence.Actions
{
    public class ActionTimeoutDao : ActionDao
    {
        public override string GenerateConstructor(string holderClass) {
            return $"new ActionTimeout(\"{Id}\", ei)";
        }
        
        public override string GenerateParameters(string workflowClassName) {
            return CodeGenerator.Parameters(this);
        }
    }
}
