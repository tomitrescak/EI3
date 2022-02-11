using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Ontology;
using Ei.Core.Ontology.Actions;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Costs;
using Ei.Simulation.Behaviours.Sensors;

namespace Ei.Simulation.Behaviours
{
    public class TravelCostManager : ICostManager
    {
        static readonly CostData UnitCostData = new CostData(1, null);

        private readonly AgentEnvironment agentEnvironment;
        private readonly int agentX;
        private readonly int agentY;
        private readonly Governor agent;
        private Sensor sensor;

        public TravelCostManager(Governor agent, AgentEnvironment agentEnvironment, Sensor sensor, int agentX, int agentY) {
            this.agentEnvironment = agentEnvironment;
            this.agentX = agentX;
            this.agentY = agentY;
            this.agent = agent;
            this.sensor = sensor;

        }

        public CostData ComputeCost(Governor agent, AStarNode fromNode, Connection toConnection) {
            if (toConnection.Action == null) {
                return UnitCostData;
            }

            // find the last performed action

            var from = fromNode.CostData;
            if (from == null) {
                var checkNode = fromNode.Parent;
                while (checkNode != null && from == null) {
                    from = checkNode.CostData;
                    checkNode = checkNode.Parent;
                }
            }

            if (toConnection.Action is ActionJoinWorkflow && this.agentEnvironment.NoLocationInfo(toConnection.Action.Id) != null) {
                var allCosts = new List<double>();
                ComputeWorkflowCost(agent, fromNode, from, toConnection, allCosts, fromNode.Resources);
                return new CostData(allCosts.Count == 0 ? float.PositiveInfinity : (float)allCosts.Average(), toConnection.Action.Id);
            }

            return this.ComputeCost(agent, fromNode, from, toConnection.Action.Id);


        }

        private void ComputeWorkflowCost(Governor agent, AStarNode fromNode, string from, Connection toConnection, List<double> allCosts, Governor.GovernorState state) {
            // compute average distance
            var wf = ((ActionJoinWorkflow)toConnection.Action).TestWorkflow;


            //var totalCost = 0d;
            foreach (var connection in wf.Connections) {
                if (connection.Action != null && connection.CanPass(state, wf.Resources)) {
                    if (connection.Action is ActionMessage) {
                        var cost = this.ComputeCost(agent, fromNode, from, connection.Action.Id).Cost;
                        if (!float.IsPositiveInfinity(cost)) {
                            allCosts.Add(cost);
                        }
                    }

                    if (connection.Action is ActionJoinWorkflow) {
                        ComputeWorkflowCost(agent, fromNode, from, connection, allCosts, state);
                    }
                }
            }
        }

        private CostData ComputeCost(Governor governor, AStarNode currentNode, string fromObjectId, string toAction) {
            // if from action has no cost, we need to find a previous node that contains an environment element, 
            // so that we know the last position 
            // if no such node exists, we assume agent position (from is null)

            if (!string.IsNullOrEmpty(fromObjectId) && this.agentEnvironment.NoLocationInfo(fromObjectId) != null) {
                fromObjectId = null;

                var parent = currentNode.Parent;
                while (fromObjectId == null && parent != null) {
                    if (!string.IsNullOrEmpty(parent.CostData) &&
                        this.agentEnvironment.Actions.ContainsKey(parent.CostData)) {
                        fromObjectId = parent.CostData;
                    }
                    else {
                        parent = parent.Parent;
                    }
                }
            }

            var sensedObjects = this.sensor.FindActionObjects(toAction);

            // find all objects that provide the required action
            if (sensedObjects == null) {
                var data = this.agentEnvironment.NoLocationInfo(toAction);

                if (data == null) {
                    throw new ApplicationException("There is no visible object in the environment that does: " + toAction);
                }
                return new CostData(data.Duration, data.Id);

            }

            var objects = sensedObjects;
            var minDistance = double.MaxValue;
            string minObject = null;


            if (string.IsNullOrEmpty(fromObjectId)) {
                // we search the action based on agent's position 
                for (var index = 0; index < objects.Count; index++) {
                    if (index >= objects.Count) break;

                    var obj = objects[index];

                    // check the owner, we need to make sure that owned object are not considered
                    if (obj.Owner != null && !obj.Owner.Is(agent)) {
                        continue;
                    }

                    var distance = this.agentEnvironment.Distance(this.agentX, this.agentY, obj.transform.X, obj.transform.Y);
                    if (distance < minDistance) {
                        minDistance = distance;
                        minObject = obj.Name;
                    }
                }
            }
            else {
                // find the object that is the closest to the previous action
                for (var index = 0; index < objects.Count; index++) {
                    if (index >= objects.Count) break;

                    if (!this.agentEnvironment.Distances.ContainsKey(fromObjectId)) {
                        throw new InstitutionException("There is no environmental action: " + fromObjectId);
                    }

                    var obj = objects[index];
                    // distance is kept one way or another
                    double distance = this.agentEnvironment.Distances[fromObjectId].ContainsKey(obj.Name)
                        ? this.agentEnvironment.Distances[fromObjectId][obj.Name]
                        : this.agentEnvironment.Distances[obj.Name][fromObjectId];

                    if (distance < minDistance) {
                        minDistance = distance;
                        minObject = obj.Name;
                    }
                }
            }


            return new CostData((float)minDistance, minObject);
        }

        public struct Closest
        {
            public double distance;
            public string objectId;
        }

        public static Closest FindClosestAction(AgentEnvironment agentEnvironment, Governor agent, int agentX, int agentY, string action, string fromObjectId = null) {
            Closest closest;
            if (!agentEnvironment.Actions.ContainsKey(action)) {
                closest.objectId = null;
                closest.distance = float.MaxValue;
                return closest;
            }
            var objects = agentEnvironment.Actions[action];

            closest.distance = double.MaxValue;
            closest.objectId = null;

            if (string.IsNullOrEmpty(fromObjectId)) {
                // we search the action based on agent's position 
                for (var index = 0; index < objects.Count; index++) {
                    if (index >= objects.Count) break;

                    var obj = objects[index];

                    // check the owner, we need to make sure that owned object are not considered
                    if (obj.Owner != null && !obj.Owner.Is(agent)) {
                        continue;
                    }

                    var distance = agentEnvironment.Distance(agentX, agentY, obj.transform.X, obj.transform.Y);
                    if (distance < closest.distance) {
                        closest.distance = distance;
                        closest.objectId = obj.Name;
                    }
                }
            }
            else {
                // find the object that is the closest to the previous action
                for (var index = 0; index < objects.Count; index++) {
                    if (index >= objects.Count) break;

                    if (!agentEnvironment.Distances.ContainsKey(fromObjectId)) {
                        throw new InstitutionException("There is no environmental action: " + fromObjectId);
                    }

                    var obj = objects[index];
                    // distance is kept one way or another
                    double distance = agentEnvironment.Distances[fromObjectId].ContainsKey(obj.Name)
                        ? agentEnvironment.Distances[fromObjectId][obj.Name]
                        : agentEnvironment.Distances[obj.Name][fromObjectId];

                    if (distance < closest.distance) {
                        closest.distance = distance;
                        closest.objectId = obj.Name;
                    }
                }
            }

            return closest;
        }
    }
}
