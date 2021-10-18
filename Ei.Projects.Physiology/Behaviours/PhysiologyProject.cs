using Ei.Simulation.Behaviours;
using Ei.Simulation.Simulator;

namespace Ei.Projects.Physiology.Behaviours
{
    public class PhysiologyProject : SimulationProject
    {
        public float[] SpeedDiversity { get; set; }
        public float[] PhysiologyDiversity { get; set; }

        public double HungerTreshold { get; set; }
        public double FatigueTreshold { get; set; }
        public double ThirstThreshold { get; set; }

        public double KillThirstThreshold { get; set; }
        public double KillHungerThreshold { get; set; }

        public double RestSpeed { get; set; }

    }
}