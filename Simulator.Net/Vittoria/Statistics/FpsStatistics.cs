using Ei.Agents.Planning;
using Ei.Agents.Sims;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vittoria.Core;

namespace Vittoria.Statistics
{
    public class FpsStatistic
    {
        public int Fps { get; set; }
        public int AgentCount { get; set; }
    }

    public class FpsStatistics: IStatistic
    {
        private StatisticTrait<FpsStatistic> fpsTrait;

        public Axis[] Axes { get; private set; }
        public List<StatisticTrait> Traits { get; private set; }

        public int ProcessTimeInMilliseconds {
            get {
                return 500;
            }
        }

        public FpsStatistics() {
            this.Axes = new[] {
            new LinearAxis {
                Key = "Fps",
                Position = AxisPosition.Left,
                Maximum = 70
            },
            //new LinearAxis {
            //    Key = "AgentCount",

            //    Position = AxisPosition.Left
            //},
             new LinearAxis {
                Key = "Count",
                // LabelFormatter = d => Project.FormatTime(d),
                Position = AxisPosition.Bottom
            }
            };

            // add traits
            this.Traits = new List<StatisticTrait>();

            this.fpsTrait = new StatisticTrait<FpsStatistic>("Fps", data => data.Fps);
            this.fpsTrait.Series.YAxisKey = "Fps";
            this.fpsTrait.Series.XAxisKey = "Count";
            
            this.Traits.Add(this.fpsTrait);
        }

        public void ProcessSamples() {
            // get samples
            //var statistic = new FpsStatistic { Fps = Time.Fps, AgentCount = GameObject.FindObjectsOfType<Sim>().Count() + GameObject.FindObjectsOfType<EiAgent>().Count() };

            // process with all traits
            //this.fpsTrait.Process(statistic.AgentCount, statistic);
        }

        public bool LoadDataset(BinaryReader br) {
            return false;
        }
    }
}
