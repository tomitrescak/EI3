using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Runtime;

namespace Ei.Ontology.Actions
{
    public class ActionStartWorkflow : ActionWithParameters
    {

        private readonly ActionJoinWorkflow joinAction;

        public ActionStartWorkflow(string id, Institution ei, ActionJoinWorkflow joinAction, ParameterState createParameters) : base(ei, id, createParameters)
        {
            this.joinAction = joinAction;
        }

        protected override IActionInfo PerformAction(Governor agent, Connection connection, ParameterState parameters)
        {
            var result = parameters.Validate();
            if (result != null) {
                return new ActionInfo(InstitutionCodes.InvalidParameters, result);
            }

            // launch a new instance of the workflow
            var wf = this.joinAction.Create(agent, parameters);
            if (wf == null) {
                return ActionInfo.AccessDenied;
            }

            // set parameter
            agent.Resources.CreatedInstanceId = wf.InstanceId;

            return ActionInfo.Ok;
        }
    }
}
