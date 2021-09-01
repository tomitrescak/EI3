using System.Collections.Generic;

namespace Ei.Core.Runtime
{
    public struct WorkflowInfo
    {
        public string Id { get; }

        public int InstanceId { get; }

        public string Name { get; }

        public List<VariableInstance> Parameters { get; }

        public WorkflowInfo(string id, int instanceId, string name, List<VariableInstance> parameters)
        {
            this.Id = id;
            this.InstanceId = instanceId;
            this.Name = name;
            this.Parameters = parameters;
        }
    }
}
