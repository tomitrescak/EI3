using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence.Actions
{
    public class ActionTimeoutDao : ActionDao
    {
        public override string GenerateConstructor(string holderClass) {
            return $"new ActionTimeout(\"{Id}\", ei)";
        }
    }
}
