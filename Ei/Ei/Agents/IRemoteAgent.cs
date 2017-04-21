using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Ei.Runtime;

namespace Ei.Agents
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
