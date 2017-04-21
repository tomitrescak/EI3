using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Ei.Ontology;
using Ei.Runtime.Planning.Heuristics;

namespace Ei.Runtime.Planning.Strategies
{
    public enum PlanStrategy
    {
        ForwardSearch,
        ForwardSearchWithBinaryPredicates,
        BackwardSearch
    }

    internal interface IStrategy
    {
        //Group[] Groups { get; }
        AStarNode InitialNode { get; }

        //IAgentState CloneProperties(IAgentState source);
        Connection[] ViableConnections(AStarNode node);
        void ApplyPostConditions(AStarNode node);
        void ApplyEffect(AStarNode node, AccessCondition effect);

        IStrategy CreateNested(AStarNode currentNode, Workflow workflow, Connection conn);
        bool ExpandEffects { get; }
        IHeuristics CreateHeuristicsForNestedSearch(AStarNode currentNode);
    }
}
