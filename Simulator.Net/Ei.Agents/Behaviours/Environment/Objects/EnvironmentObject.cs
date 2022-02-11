using Ei.Core.Runtime;
using Ei.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Objects
{

    public class EnvironmentObject : MonoBehaviour
    {
        private EnvironmentAction[] actions;
        private ObjectAction[] plan;

        //private ObjectAction[] withBeforeAll;
        //private ObjectAction[] withAfterAll;
        //private ObjectAction[] withBeforeEach;
        //private ObjectAction[] withAfterEach;
       

        // public string Id;

        [NonSerialized]
        public Governor Owner;

        public string Name { get => this.gameObject.name; } 

        public string Icon;

        public virtual EnvironmentAction[] Actions { 
            get { return actions; } 
            set { this.actions = value; } 
        }


        public void Start()
        {
            // register with the environment
            FindObjectOfType<AgentEnvironment>().AddObject(this);

            this.plan = GetComponents<ObjectAction>().OrderBy(a => a.Index).ToArray();
            
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

            foreach (var item in this.plan)
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
                await item.UseObject(agent, action);


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

}
