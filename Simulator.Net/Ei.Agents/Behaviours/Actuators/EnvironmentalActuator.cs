using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Costs;
using Ei.Logs;
using Ei.Simulation.Behaviours.Environment.Objects;
using Ei.Simulation.Behaviours.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Simulation.Behaviours.Actuators
{
    public class EnvironmentalActuator: Actuator
    {
        AgentEnvironment environment;
        NavigationBase navigation;
        Sensor sensor;

        public void Start()
        {
            this.sensor = GetComponent<Sensor>();
            if (this.sensor == null)
            {
                throw new Exception("You must define agent sensor to use with EnvironmentalActuator");
            }

            this.navigation = GetComponent<NavigationBase>();
            if (this.navigation == null)
            {
                throw new Exception("You must define navigation controller to use with EnvironmentalActuator");
            }

            this.environment = FindObjectOfType<AgentEnvironment>();
            if (this.environment == null)
            {
                throw new Exception("You must define agent environment to use with EnvironmentalActuator");
            }
        }

        public override ICostManager CreateCostManager(PlanManager.PlannerSession session)
        {
            return new TravelCostManager(session.agent.Governor, this.environment, this.sensor, (int)this.transform.X, (int)this.transform.Y);
        }

        public override async Task<bool> ExecutePlanItem(SimulationAgent agent, AStarNode planItem)
        {
            if (this.gameObject.GameEngine.IsRunning == false)
            {
                Log.Warning(agent.Name + " E.Actuator", "Not executing the plan item as engine is not running");
                return false;
            }

            // if is it has no location
            // we only wait for the defined time and execute the insitutional action

            var actionId = planItem.Arc.Action.Id;
            var item = this.environment.NoLocationInfo(actionId);
            if (item != null)
            {
                Log.Debug(agent.Name + " Actuator", "Waiting: " + item.Duration);
                // only wait for specific interval
                await Task.Delay((int) item.Duration);
                return base.PerformAction(agent, planItem);
            }

            var itemId = planItem.CostData;

            // the plan item contains no information on cost data
            // get random object that provides this action
            if (itemId == null)
            {
                var actionObjects = this.sensor.FindActionObjects(actionId);
                if (actionObjects == null || actionObjects.Count == 0) { 
                    throw new Exception($"Environment does not contain object providing action '{actionId}'");
                }
                itemId = actionObjects[UnityEngine.Random.Range(0, actionObjects.Count)].Name;
            }


            // find this object in the environment
            // move to its location
            // interact with it and wait for interaction to finish
            if (this.environment.TryGetValue(itemId, out EnvironmentObject obj))
            {
                // travel to the new destination
                var moved = await this.navigation.MoveToDestination(obj.transform.X, obj.transform.Y);
                if (!moved)
                {
                    Log.Warning(agent.Name + " Actuator", "Failed moving to the destination");
                    return false;
                }
                
                var action = obj.Actions.FirstOrDefault(x => x.Id == actionId);

                // check if the action provides parameters
                if (action != null)
                {
                    Log.Debug(agent.Name + " Actuator", $"Started interaction with '{obj.Name}' lasting {action.Duration}ms");
                    await obj.Use(actionId);
                    Log.Debug(agent.Name + " Actuator", $"Finished interaction with '{obj.Name}' after {action.Duration}ms");
                    
                    return base.PerformAction(agent, planItem, action.Parameters);
                } else
                {
                    throw new Exception("This object does not provide action: " + actionId);
                }
            }
            else
            {
                throw new PlanException("Missing resouce: " + planItem.CostData);
            }
            return false;
        }
    }
}
