using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ei.Agents.Sims
{
    public class SimAction
    {
        public string Name { get; set; }
        public int Uses { get; set; }
        public bool Depleted { get; set; }
        public ModifierAdvertisement[] Modifiers { get; set; }
        public string[] Plan { get; set; }
        public float DurationInMinutes { get; set; }

        public SimAction() {
            this.Plan = new string[0];
        }

        public SimAction(string name, int uses, ModifierAdvertisement[] modifiers, float durationInMinutes, string[] plan) {
            this.Name = name;
            this.Uses = uses;
            this.Modifiers = modifiers;
            this.Plan = plan;
            this.DurationInMinutes = durationInMinutes;
        }
    }
}
