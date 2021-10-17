using System.Linq;
using Ei.Core.Runtime;

namespace Ei.Simulation.Behaviours.Physiology
{
    public class PhysiologCallbacks : GovernorCallbacks
    {
        public PhysiologCallbacks(SimulationAgent agent) : base(agent)
        {
        }

        public virtual void Split(Governor[] splits, bool shallowClone)
        {
            (this.owner as PhysiologyBasedAgent).MainAgent = splits.First(w => w.Name.EndsWith("Main"));
            (this.owner as PhysiologyBasedAgent).PhysiologyAgent = splits.First(w => w.Name.EndsWith("Physiology"));
        }
    }
}
