using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Persistence.Actions;
using Ei.Persistence.Transitions;

namespace Ei.Persistence
{
    public class GlobalsDao
    {
        public ActionDao[] Actions { get; set; }

        public ConnectionDao[] Connections { get; set; }

        public List<FunctionDao> Functions { get; set; }
    }
}
