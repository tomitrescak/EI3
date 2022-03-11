using Ei.Core.Runtime;
using Ei.Core.Ontology;

namespace Ei.Core.Ontology.Actions
{
    public class ActionExitWorkflow : ActionBase
    {
        public ActionExitWorkflow() 
        {
        }

        protected override IActionInfo PerformAction(Governor performer, Connection connection, ParameterState parameters)
        {
            return performer.ExitWorkflow();
        }
    }
}
