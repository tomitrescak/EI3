using System.Linq;
using Ei.Core.Runtime;
using Ei.Simulation.Behaviours;

namespace Ei.Projects.Physiology
{
    public class PhysiologCallbacks : GovernorCallbacks
    {
        public PhysiologCallbacks(SimulationAgent agent) : base(agent)
        {
        }

        public override void Split(Governor[] splits, bool shallowClone)
        {
            (this.owner as PhysiologyBasedAgent).MainAgent = splits.First(w => w.Name.EndsWith("Main"));
            (this.owner as PhysiologyBasedAgent).PhysiologyAgent = splits.First(w => w.Name.EndsWith("Physiology"));
        }
    }
}
