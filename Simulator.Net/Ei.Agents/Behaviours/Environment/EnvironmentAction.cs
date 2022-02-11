using Ei.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Simulation.Behaviours.Environment
{
    [Serializable]
    public class EnvironmentAction
    {
        public string Id { get; set; }
        public VariableInstance[] Parameters { get; set; }
    }

    [Serializable]
    public class NoLocationAction : EnvironmentAction
    {
        public float Duration { get; set; }
    }
}
