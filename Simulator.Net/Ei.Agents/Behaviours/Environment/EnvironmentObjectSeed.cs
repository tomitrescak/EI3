using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Ontology.Actions;

namespace Ei.Simulation.Behaviours.Environment
{
    [Serializable]
    public class EnvironmentObjectSeed
    {
        private int maxId;
        private int count;

        public string Id { get; set; }
        public string Image { get; set; }
        public int Seed { get; set; }
        public int[] Range { get; set; }
        public float Probability { get; set; }
        public int Max { get; set; }
        public EnvironmentAction[] Actions { get; set; }

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



    
}