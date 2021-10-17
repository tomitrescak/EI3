using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Sims.Behaviours;
using UnityEngine;

namespace Ei.Simulation.Statistics
{
    public class FpsStatistic
    {
        public int Fps { get; set; }
        public int AgentCount { get; set; }
    }


    public class FpsStatistics : MonoBehaviour, IStatistic
    {
        private readonly StatisticTrait<FpsStatistic> fpsTrait;

        public Axis[] Axes { get; private set; }
        public List<StatisticTrait> Traits { get; private set; }

        public int ProcessTimeInMilliseconds => 500;

        public FpsStatistics() {
            this.Axes = new[] {
                new Axis {
                    Key = "Fps",
                    Position = AxisPosition.Left,
                    Maximum = 70
                },
                //new LinearAxis {
                //    Key = "AgentCount",

                //    Position = AxisPosition.Left
                //},
                new Axis {
                    Key = "Count",
                    // LabelFormatter = d => Project.FormatTime(d),
                    Position = AxisPosition.Bottom
                }
            };

            // add traits
            this.Traits = new List<StatisticTrait>();

            this.fpsTrait = new StatisticTrait<FpsStatistic>("Fps", data => data.Fps) {
                YAxisKey = "Fps",
                XAxisKey = "Count"
            };

            this.Traits.Add(this.fpsTrait);
        }

        public void ProcessSamples() {
            // get samples
            var statistic = new FpsStatistic {
                Fps = Time.Fps,
                AgentCount = FindObjectsOfType<Sim>().Count() +
                             FindObjectsOfType<SimulationAgent>().Count()
            };

            // process with all traits
            this.fpsTrait.Process(statistic.AgentCount, statistic);
        }

        public bool LoadDataset(BinaryReader br) {
            return false;
        }
    }
}