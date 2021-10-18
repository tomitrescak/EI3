using Ei.Core.Ontology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    public class SimulationTimer: MonoBehaviour
    {
        public float DayLengthInSeconds { get; set; }

        [JsonIgnore]
        public bool Paused { get; set; }

        Institution ei;

        [JsonIgnore]
        public Stopwatch Time { get; protected set; }

        [JsonIgnore]
        public string RealTime
        {
            get
            {
                var realSeconds = this.Time.ElapsedMilliseconds / 1000f;

                TimeSpan t1 = TimeSpan.FromSeconds(realSeconds);

                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                    t1.Hours,
                    t1.Minutes,
                    t1.Seconds);

            }
        }

        [JsonIgnore]
        public double SimulatedTime
        {
            get
            {
                var realSeconds = this.Time.ElapsedMilliseconds / 1000f;
                return realSeconds * (86400 / DayLengthInSeconds);
            }
        }

        [JsonIgnore]
        public string SimulatedTimeString => FormatTime(this.SimulatedTime);

        [JsonIgnore]
        public double SimulatedSecond => 86400 / this.DayLengthInSeconds;

        public void Start()
        {
            var project = FindObjectOfType<SimulationProject>();
            this.Time = new Stopwatch();
            this.Time.Start();  

            this.ei = project.Ei;
        }

        public double CalculateDuration(float waitTimeInSeconds)
        {
            return waitTimeInSeconds / SimulatedSecond;
        }

        public static string FormatTime(double simulatedSeconds)
        {
            TimeSpan t2 = TimeSpan.FromSeconds(simulatedSeconds);

            return string.Format("{0}d {1:D2}:{2:D2}",
                t2.Days,
                t2.Hours,
                t2.Minutes,
                t2.Seconds);
        }
    }
}
