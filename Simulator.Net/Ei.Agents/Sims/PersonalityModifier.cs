﻿using System;

namespace Ei.Agents.Sims
{
    [Serializable]
    public class PersonalityModifier
    {
        public PersonalityType type { get; set; }
        public float delta { get; set; }
    }
}
