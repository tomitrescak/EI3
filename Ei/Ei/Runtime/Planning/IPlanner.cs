using System.Collections.Generic;
using Ei.Ontology;

namespace Ei.Runtime.Planning
{
    interface IPlanner
    {
        List<Connection> CreatePlan(Governor agent, VariableState desiredState);
    }
}
