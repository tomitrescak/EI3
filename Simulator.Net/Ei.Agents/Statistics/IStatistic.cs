using System.Collections.Generic;

namespace Ei.Simulation.Statistics
{
    public interface IStatistic
    {
        Axis[] Axes { get; }
        List<StatisticTrait> Traits { get; }
        void ProcessSamples();
        int ProcessTimeInMilliseconds { get; }
    }
}
