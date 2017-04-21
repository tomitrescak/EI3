using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;

namespace Ei.Runtime.Planning.Costs
{
    class UnitCostManager: ICostManager
    {
        readonly CostData costData;

        private static UnitCostManager instance;

        public UnitCostManager(float cost)
        {
            costData = new CostData(cost, null);
        }

        public static UnitCostManager Instance
        {
            get { return instance ?? (instance = new UnitCostManager(1)); }
        }

        public CostData ComputeCost(Governor agent, AStarNode fromNode, Connection toConnection)
        {
            return costData;
        }
    }
}
