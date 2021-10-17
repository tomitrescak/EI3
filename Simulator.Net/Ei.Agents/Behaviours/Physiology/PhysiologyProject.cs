using Ei.Simulation.Simulator;

namespace Ei.Simulation.Behaviours.Physiology
{
    public class PhysiologyProject : SimulationProject
    {
        public double HungerTreshold { get; set; }
        public double FatigueTreshold { get; set; }
        public double ThirstThreshold { get; set; }

        public double KillThirstThreshold { get; set; }
        public double KillHungerThreshold { get; set; }

        public double RestSpeed { get; set; }

        public float[] PhysiologyDiversity { get; set; }
    }
}