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
        public float Duration { get; set; }
        public VariableInstance[] Parameters { get; set; }
    }
}
