using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;

namespace Ei.Agents
{
    public interface IAgentRemote
    {
        string Name { get; }
        Group[] Groups { get; }

    }
}
