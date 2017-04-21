namespace Ei.Runtime
{
    public struct WorkflowInfo
    {
        public string Id { get; }

        public int InstanceId { get; }

        public string Name { get; }

        public VariableProperty[] Parameters { get; }

        public WorkflowInfo(string id, int instanceId, string name, VariableProperty[] parameters)
        {
            this.Id = id;
            this.InstanceId = instanceId;
            this.Name = name;
            this.Parameters = parameters;
        }
    }
}
