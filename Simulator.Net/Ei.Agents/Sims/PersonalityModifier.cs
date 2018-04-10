using System;

namespace Ei.Simulation.Sims
{
    [Serializable]
    public class PersonalityModifier
    {
        public PersonalityType type { get; set; }
        public float delta { get; set; }
    }
}
