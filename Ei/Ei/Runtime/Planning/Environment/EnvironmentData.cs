using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology.Actions;

namespace Ei.Runtime.Planning.Environment
{
    [Serializable]
    public struct EnvironmentData
    {
        private Dictionary<string, int> uses;
         
        public string Id;
        public EnvironmentDataDefinition Definition;
        public int X;
        public int Y;

        [NonSerialized]
        public Governor Owner;

        [NonSerialized]
        public Dictionary<string, VariableProperty[]> Parameters;

        public EnvironmentData(EnvironmentDataDefinition definition, int x, int y, Dictionary<string, VariableProperty[]> parameters = null, Governor owner = null)
        {
            this.uses = new Dictionary<string, int>();
            this.Id = definition.NewId();
            this.Definition = definition;
            this.X = x;
            this.Y = y;
            this.Owner = owner;
            this.Parameters = parameters;
        }

        public EnvironmentData(string id, string actionName, int x, int y, int destroyAfter = 0, Dictionary<string, VariableProperty[]> parameters = null, Governor owner = null)
        {
            this.uses = new Dictionary<string, int>();
            this.Id = id;
            this.Definition = new EnvironmentDataDefinition
            {
                Actions = new [] { new EnvironmentDataAction { Id = actionName, DestroyAfter = destroyAfter } }
            };
            this.X = x;
            this.Y = y;
            this.Owner = owner;
            this.Parameters = parameters;
        }

        //        public EnvironmentData(string id, string[] actionIds, int x, int y)
        //        {
        //            this.Id = id;
        //            //this.ActionId = actionId;
        //            this.X = x;
        //            this.Y = y;
        //        }

        public int Use(string id)
        {
            //try
            //{
                var action = this.Definition.Actions.First(w => w.Id == id);

                if (action.DestroyAfter == 0)
                {
                    return 1; // there will always be one more use
                }

                if (!this.uses.ContainsKey(id))
                {
                    this.uses.Add(id, 0);
                }

                this.uses[id] ++;

                // return how many uses are remainig

                return action.DestroyAfter - this.uses[id];
            //}
            //catch (Exception ex) { }
            //finally { }
           // return 1;
        }
    }
}
