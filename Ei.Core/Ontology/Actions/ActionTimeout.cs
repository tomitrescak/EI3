using Ei.Core.Runtime;

namespace Ei.Core.Ontology.Actions
{
    using Core.Runtime;
    public class ActionTimeout : ActionBase
    {
        public ActionTimeout(string id, Institution ei) : base(ei, id)
        {
        }

        protected override IActionInfo PerformAction(Governor performer, Connection connection, ParameterState parameters)
        {
            return ActionInfo.Ok;
        }
    }
}
