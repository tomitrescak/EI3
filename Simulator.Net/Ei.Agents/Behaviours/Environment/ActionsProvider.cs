using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Core.Runtime;
using UnityEngine;
using Newtonsoft.Json;

namespace Ei.Simulation.Behaviours.Environment;

public class ActionsProvider : MonoBehaviour
{
    [NonSerialized]
    public Governor Owner;

    public EnvironmentAction[] Actions;
    
    [JsonIgnore]
    public string Name { get => this.gameObject.name; }
    
    public void Start()
    {
        // register with the environment
        FindObjectOfType<AgentEnvironment>().AddObject(this);

        //this.withBeforeAll = this.plan.Where(p => p.BeforeAll != null).ToArray();
        //this.withBeforeEach = this.plan.Where(p => p.BeforeEach != null).ToArray();
        //this.withAfterAll= this.plan.Where(p => p.AfterAll != null).ToArray();
        //this.withAfterEach = this.plan.Where(p => p.AfterEach != null).ToArray();
    }
    
    public virtual async Task<bool> Use(SimulationAgent agent, string actionId)
        {
            var action = this.Actions.First(w => w.Id == actionId);
            if (action == null)
            {
                throw new Exception("This objects does not provide action: " + actionId);
            }


            //// before all
            //if (this.withBeforeAll.Length > 0)
            //{
            //    foreach (var item in this.withBeforeAll)
            //    {
            //        var result = await item.BeforeAll(action);
            //        if (result == false)
            //        {
            //            Log.Error(this.Name, "Error processing BeforeAll in action: " + item.gameObject.name);
            //            return false;
            //        }
            //    }
            //}

            foreach (var item in action.Plan)
            {
                //// before each
                //if (this.withBeforeEach.Length > 0)
                //{
                //    foreach (var be in this.withBeforeEach)
                //    {
                //        var result = await be.BeforeEach(action);
                //        if (result == false)
                //        {
                //            Log.Error(this.Name, "Error processing BeforeEach in action: " + item.gameObject.name);
                //            return false;
                //        }
                //    }
                //}

                // execute
                await item.UseObject(this.gameObject, agent, action);


                //// after each
                //if (this.withAfterEach.Length > 0)
                //{
                //    foreach (var ae in this.withAfterEach)
                //    {
                //        var result = await ae.AfterEach(action);
                //        if (result == false)
                //        {
                //            Log.Error(this.Name, "Error processing AfterEach in action: " + item.gameObject.name);
                //            return false;
                //        }
                //    }
                //}
            }

            // after all 
            //if (this.withAfterAll.Length > 0)
            //{
            //    foreach (var item in this.plan)
            //    {
            //        var result = await item.AfterAll(action);
            //        if (result == false)
            //        {
            //            Log.Error(this.Name, "Error processing AfterAll in action: " + item.gameObject.name);
            //            return false;
            //        }
            //    }
            //}

            return true;
        }
}