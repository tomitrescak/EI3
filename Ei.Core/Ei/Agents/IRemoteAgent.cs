using Ei.Core.Runtime;

namespace Ei.Core.Agents
{
    public interface IRemoteAgent
    {
        IGovernorCallbacks Callbacks { get; }
        IAgentRemote Remote { get; set; }

        string Organisation { get; }
        string Username { get; }
        string Password { get; }
        string[][] Roles { get; }
    }
}
