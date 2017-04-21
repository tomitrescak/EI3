using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Runtime;

namespace Ei.Ontology.Actions
{
    public class ActionExitWorkflow : ActionBase
    {
        public ActionExitWorkflow() 
        {
        }

        protected override IActionInfo PerformAction(Governor performer, Connection connection, ActionParameters parameters)
        {
            return performer.ExitWorkflow();
        }
    }
}
