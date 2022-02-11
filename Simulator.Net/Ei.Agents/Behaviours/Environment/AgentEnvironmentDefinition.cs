using Ei.Simulation.Behaviours.Environment;
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
        public NoLocationAction[] ActionsWithNoLocation { get; set; }
    }
}
