using Ei.Core.Ontology;

namespace Ei.Core.Agents
{
    public interface IAgentRemote
    {
        string Name { get; }
        Group[] Groups { get; }

    }
}
