using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Runtime;

namespace Ei.Ontology.Actions
{
    public class ActionStartWorkflow : ActionBase
    {

        private readonly ActionJoinWorkflow joinAction;
        private readonly ResourceState parameters;

        public ActionStartWorkflow(string id, Institution ei, ActionJoinWorkflow joinAction, ResourceState parameters) : base(ei, id)
        {
            this.joinAction = joinAction;
            this.parameters = parameters;
        }

        protected override IActionInfo PerformAction(Governor agent, Connection connection, ResourceState parameters)
        {
            // launch a new instance of the workflow
            var wf = this.joinAction.Create(agent, this.parameters);

            // set parameter
            agent.Resources.CreatedInstanceId = wf.InstanceId;

            return ActionInfo.Ok;
        }
    }
}
