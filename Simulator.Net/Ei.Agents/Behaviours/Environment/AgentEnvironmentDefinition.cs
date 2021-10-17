using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Simulation.Behaviours
{
    public class AgentEnvironmentDefinition
    {
        public float MetersPerPixel { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public EnvironmentDataAction[] ActionsWithNoLocation { get; set; }
        public EnvironmentDataDefinition[] Elements { get; set; }
    }
}
