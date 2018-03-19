using Ei.Agents.Core.Behaviours;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ei.Agents.Sims
{
    [Serializable]
    public class ModifierAdvertisement : MonoBehaviour
    {
        public ModifierType Type { get; set; }
        public float Delta { get; set; }
        public List<PersonalityModifier> PersonalityModifiers { get; set; }

        public ModifierAdvertisement() {
            this.PersonalityModifiers = new List<PersonalityModifier>(2);
        }

        public ModifierAdvertisement(float delta, ModifierType type, List<PersonalityModifier> modifiers) {
            this.Delta = delta;
            this.Type = type;
            this.PersonalityModifiers = modifiers;
        }
    }
}
