using Ei.Core.Runtime;
using System;
using System.Collections.Generic;
using Ei.Simulation.Behaviours.Environment.Actions;

namespace Ei.Simulation.Behaviours.Environment
{
    [Serializable]
    public class EnvironmentAction
    {
        public Interaction[] Plan;
        public string Id { get; set; }
        public VariableInstance[] Parameters { get; set; }
    }

    [Serializable]
    public class NoLocationAction : EnvironmentAction
    {
        public float Duration { get; set; }
    }
}
