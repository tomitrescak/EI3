using Ei.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Simulation.Behaviours.Environment.Objects
{
    public class DelayAction : ObjectAction
    {
        public int DelayMs { get; set; }

        public override async Task<bool> UseObject(SimulationAgent agent, EnvironmentAction action)
        {
            // TODO: This can be done per action but not needed ATM
            // See example for LimitUseAction

            Log.Debug(agent.Name, "Object", $"Using {this.gameObject.name} for {this.DelayMs}ms");
            await Task.Delay(this.DelayMs);
            return true;
        }
    }
}
