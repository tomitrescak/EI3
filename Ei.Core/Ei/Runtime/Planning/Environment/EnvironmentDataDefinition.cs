using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Ontology.Actions;

namespace Ei.Core.Runtime.Planning.Environment
{
    [Serializable]
    public class EnvironmentDataDefinition
    {
        private int maxId;
        private int count;

        public string Id { get; set; }
        public string Image { get; set; }
        public int Seed { get; set; }
        public int[] Range { get; set; }
        public float Probability { get; set; }
        public int Max { get; set; }
        public EnvironmentDataAction[] Actions { get; set; }

        public string NewId()
        {
            this.count ++;
            return this.Id + "_" + this.maxId++;
        }

        public bool CanCreate()
        {
            return this.Probability > 0 && this.count < this.Max;
        }

        public void Destroy()
        {
            this.count--;
        }
    }

    [Serializable]
    public class EnvironmentDataAction
    {
        public string Id { get; set; }
        public int DestroyAfter { get; set; }
        public float Duration { get; set; }
    }

    
}