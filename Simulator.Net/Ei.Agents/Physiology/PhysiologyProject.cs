using Ei.Simulation.Simulator;

namespace Ei.Simulation.Physiology
{
    public class PhysiologyProject : Project
    {
        public double HungerTreshold { get; set; }
        public double FatigueTreshold { get; set; }
        public double ThirstThreshold { get; set; }

        public double KillThirstThreshold { get; set; }
        public double KillHungerThreshold { get; set; }

        public double RestSpeed { get; set; }
        
        protected override SimulationAgent CreateAgent(string[][] groups, string[] goals) {
            return new PhysiologyBasedAgent(this, groups[0][1], goals);
        }
    }
}