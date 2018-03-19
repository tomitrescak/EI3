using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vittoria.Statistics
{
    public interface IStatistic
    {
        Axis[] Axes { get; }
        List<StatisticTrait> Traits { get; }
        void ProcessSamples();
        int ProcessTimeInMilliseconds { get; }
    }
}
