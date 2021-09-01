using System.Collections.Generic;
using Ei.Core.Runtime;
using Ei.Core.Ontology;

namespace Ei.Core.Runtime.Planning
{
    interface IPlanner
    {
        List<Connection> CreatePlan(Governor agent, ResourceState desiredState);
    }
}
