using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Runtime.Planning.Environment
{
    public class AgentEnvironmentDefinition
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public EnvironmentDataAction[] ActionsWithNoLocation { get; set; }
        public EnvironmentDataDefinition[] Elements { get; set; }
    }
}
