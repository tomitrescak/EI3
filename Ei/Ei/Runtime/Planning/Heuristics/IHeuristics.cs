using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;

namespace Ei.Runtime.Planning.Heuristics
{
    public enum PlanHeuristics
    {
        Static,
        Resources
    }

    internal interface IHeuristics
    {
        float Calculate(AStarNode from);
        bool CheckGoal(AStarNode currentNode);
    }
}
