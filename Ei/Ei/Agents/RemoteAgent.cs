using Ei.Runtime;

namespace Ei.Agents
{
    public class RemoteAgent : IRemoteAgent
    {
        public IGovernorCallbacks Callbacks { get; }
        public IAgentRemote Remote { get; set; }
        public string Organisation { get; }
        public string Username { get; }
        public string Password { get; }
        public string[][] Roles { get; }

        public RemoteAgent(IGovernorCallbacks callbacks, 
            string username, 
            string password, 
            string organisation = null,
            string[][] roles = null)
        {
            this.Callbacks = callbacks;
            this.Username = username;
            this.Password = password;
            this.Organisation = organisation;
            this.Roles = roles;
        }

    }
}
