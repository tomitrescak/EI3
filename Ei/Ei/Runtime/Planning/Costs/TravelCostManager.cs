using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime.Planning.Environment;

namespace Ei.Runtime.Planning.Costs
{
    public class TravelCostManager : ICostManager
    {
        static readonly CostData UnitCostData = new CostData(1, null);

        private readonly AgentEnvironment agentEnvironment;
        private readonly int agentX;
        private readonly int agentY;
        private readonly Governor agent;

        public TravelCostManager(Governor agent, AgentEnvironment agentEnvironment, int agentX, int agentY)
        {
            this.agentEnvironment = agentEnvironment;
            this.agentX = agentX;
            this.agentY = agentY;
            this.agent = agent;

        }

        public CostData ComputeCost(Governor agent, AStarNode fromNode, Connection toConnection)
        {
            if (toConnection.Action == null)
            {
                return UnitCostData;
            }

            // find the last performed action

            var from = fromNode.CostData;
            if (from == null)
            {
                var checkNode = fromNode.Parent;
                while (checkNode != null && from == null)
                {
                    from = checkNode.CostData;
                    checkNode = checkNode.Parent;
                }
            }

            if (toConnection.Action is ActionJoinWorkflow && this.agentEnvironment.NoLocationInfo(toConnection.Action.Id) != null)
            {
                var allCosts = new List<double>();
                ComputeWorkflowCost(agent, fromNode, from, toConnection, allCosts, fromNode.Resources);
                return new CostData(allCosts.Count == 0 ? float.PositiveInfinity : (float)allCosts.Average(), toConnection.Action.Id);
            }

            return this.ComputeCost(agent, fromNode, from, toConnection.Action.Id);


        }

        private void ComputeWorkflowCost(Governor agent, AStarNode fromNode, string from, Connection toConnection, List<double> allCosts, Governor.ResourceState state)
        {
            // compute average distance
            var wf = ((ActionJoinWorkflow)toConnection.Action).TestWorkflow;


            //var totalCost = 0d;
            foreach (var connection in wf.Connections)
            {
                if (connection.Action != null && connection.CanPass(state))
                {
                    if (connection.Action is ActionMessage)
                    {
                        var cost = this.ComputeCost(agent, fromNode, from, connection.Action.Id).Cost;
                        if (!float.IsPositiveInfinity(cost))
                        {
                            allCosts.Add(cost);
                        }
                    }

                    if (connection.Action is ActionJoinWorkflow)
                    {
                        ComputeWorkflowCost(agent, fromNode, from, connection, allCosts, state);
                    }
                }
            }
        }

        private CostData ComputeCost(Governor governor, AStarNode currentNode, string fromObjectId, string toAction)
        {
            // if from action has no cost, we need to find a previous node that contains an environment element, 
            // so that we know the last position 
            // if no such node exists, we assume agent position (from is null)

            if (!string.IsNullOrEmpty(fromObjectId) && this.agentEnvironment.NoLocationInfo(fromObjectId) != null)
            {
                fromObjectId = null;

                var parent = currentNode.Parent;
                while (fromObjectId == null && parent != null)
                {
                    if (!string.IsNullOrEmpty(parent.CostData) &&
                        this.agentEnvironment.Actions.ContainsKey(parent.CostData))
                    {
                        fromObjectId = parent.CostData;
                    }
                    else
                    {
                        parent = parent.Parent;
                    }
                }
            }

            // find all objects that provide the required action
            if (!this.agentEnvironment.Actions.ContainsKey(toAction))
            {
                var data = this.agentEnvironment.NoLocationInfo(toAction);

                if (data == null)
                {
                    throw new ApplicationException("There is no object in the environment that does: " + toAction);
                }
                return new CostData(data.Duration, data.Id);

            }

            var objects = this.agentEnvironment.Actions[toAction];
            var minDistance = double.MaxValue;
            string minObject = null;


            if (string.IsNullOrEmpty(fromObjectId))
            {
                // we search the action based on agent's position 
                for (var index = 0; index < objects.Count; index++)
                {
                    if (index >= objects.Count) break;

                    var obj = objects[index];

                    // check the owner, we need to make sure that owned object are not considered
                    if (obj.Owner != null && !obj.Owner.Is(agent))
                    {
                        continue;
                    }

                    var distance = this.agentEnvironment.Distance(this.agentX, this.agentY, obj.X, obj.Y);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minObject = obj.Id;
                    }
                }
            }
            else
            {
                // find the object that is the closest to the previous action
                for (var index = 0; index < objects.Count; index++)
                {
                    if (index >= objects.Count) break;

                    if (!this.agentEnvironment.Distances.ContainsKey(fromObjectId))
                    {
                        throw new InstitutionException("There is no environmental action: " + fromObjectId);
                    }

                    var obj = objects[index];
                    // distance is kept one way or another
                    double distance = this.agentEnvironment.Distances[fromObjectId].ContainsKey(obj.Id)
                        ? this.agentEnvironment.Distances[fromObjectId][obj.Id]
                        : this.agentEnvironment.Distances[obj.Id][fromObjectId];

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minObject = obj.Id;
                    }
                }
            }


            return new CostData((float)minDistance, minObject);
        }
    }
}
