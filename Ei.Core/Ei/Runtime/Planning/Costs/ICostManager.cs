using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Ontology;

namespace Ei.Core.Runtime.Planning.Costs
{
    public struct CostData
    {
        public float Cost;
        public string Data;

        public CostData(float cost, string data)
        { 
            this.Cost = cost;
            this.Data = data;
        }

    } 
    public interface ICostManager
    {
        CostData ComputeCost(Governor agent, AStarNode fromNode, Connection toConnection);
    }

}
