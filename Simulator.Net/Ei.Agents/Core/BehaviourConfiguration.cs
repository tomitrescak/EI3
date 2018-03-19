using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Agents.Core
{
    public static class BehaviourConfiguration
    {
        public static float DayLengthInSeconds = 240;

        // we need the reservoirs empty 4 times a day, that is 4 times it needs to go from 100 to -100
        public static float SimulatedDecay { get { return 800 / DayLengthInSeconds; } }

        public static float RealSecondsToSimulated { get { return 86400 / DayLengthInSeconds; } }

        public static float SimulatedSecondsToReal { get { return DayLengthInSeconds / 86400; } }

        public static float SimulatedMinutesToReal { get { return (DayLengthInSeconds / 86400) * 60; } }
    }
}
