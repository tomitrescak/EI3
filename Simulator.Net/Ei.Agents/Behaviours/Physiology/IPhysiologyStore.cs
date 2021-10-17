namespace Ei.Simulation.Behaviours.Physiology
{
    public interface IPhysiologyStore
    {
        float Fatigue { get; }
        float Hunger { get; }
        float Thirst { get; }
        float ThirstDecay { get; }
        float HungerDecay { get; }
    }
}