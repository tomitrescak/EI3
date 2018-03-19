using Ei.Agents.Core;
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
    public class SimsStatistic
    {
        public float happiness { get; set; }

        public float dead { get; set; }
        public float hunger { get; set; }
        public float thirst { get; set; }
        public float social { get; set; }
        public float hygiene { get; set; }
        public float fun { get; set; }
        public float room { get; set; }
        public float energy { get; set; }
        public float comfort { get; set; }
    }

    public class SimsStatistics: IStatistic
    {
        public Axis[] Axes { get; private set; }
        public List<StatisticTrait> Traits { get; private set; }

        public int ProcessTimeInMilliseconds {
            get {
                return 2000;
            }
        }

        public SimsStatistics() {
            this.Axes = new[] {
            new LinearAxis {
                Key = "Normalised",
                Maximum = 1,
                Minimum = 0,
                Position = AxisPosition.Left
            },
             new LinearAxis {
                Key = "Date",
                // LabelFormatter = d => Project.FormatTime(d),
                Position = AxisPosition.Bottom
            }
            };

            // add traits
            this.Traits = new List<StatisticTrait>();
            this.Traits.Add(
                this.CreateTrait("Average Happiness", data => data.happiness / 100, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Dot));
            this.Traits.Add(
                this.CreateTrait("Dead", data => data.dead, 6, StatisticRelaxationStrategy.Average, 4, LineStyle.LongDash));

            this.Traits.Add(this.CreateTrait("Hunger", data => data.hunger, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Thirst", data => data.thirst, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Energy", data => data.energy, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Comfort", data => data.comfort, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Fun", data => data.fun, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Social", data => data.social, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Hygiene", data => data.hygiene, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
            this.Traits.Add(this.CreateTrait("Room", data => data.room, 6, StatisticRelaxationStrategy.Average, 2, LineStyle.Solid));
        }

        public void ProcessSamples() {
            // get samples
            var statistic = new SimsStatistic();
            var allSims = GameObject.FindObjectsOfType<Sim>().ToArray();
            if (allSims == null || allSims.Length == 0) {
                return;
            }
            var sims = allSims.Where(s => !s.IsDead).ToArray();

            statistic.happiness = sims.Average(s => s.Happiness);
            statistic.dead = allSims.Count(s => s.IsDead) / (float)allSims.Length;
            statistic.hunger = (100 - sims.Average(s => (s.Hunger + 100) / 2)) /100f;
            statistic.thirst = (100 - sims.Average(s => (s.Thirst + 100) / 2)) / 100f;
            statistic.social = (100 - sims.Average(s => (s.Social + 100) / 2)) / 100f;
            statistic.fun = (100 - sims.Average(s => (s.Fun + 100) / 2)) / 100f;
            statistic.room = (100 - sims.Average(s => (s.Room + 100) / 2)) / 100f;
            statistic.comfort = (100 - sims.Average(s => (s.Comfort + 100) / 2)) / 100f;
            statistic.hygiene = (100 - sims.Average(s => (s.Hygiene + 100) / 2)) / 100f;
            statistic.energy = (100 - sims.Average(s => (s.Energy + 100) / 2)) / 100f;

            // process with all traits
            foreach (var trait in this.Traits) {
                ((StatisticTrait<SimsStatistic>)trait).Process(Time.time * 60 * BehaviourConfiguration.SimulatedMinutesToReal, statistic);
            }
        }

        public bool LoadDataset(BinaryReader br) {
            return false;
        }

        private StatisticTrait<SimsStatistic> CreateTrait(string name,
           Func<SimsStatistic, double> func,
           int relaxationPeriod = 6,
           StatisticRelaxationStrategy relaxationStreategy = StatisticRelaxationStrategy.Average,
           double thickness = 3,
           LineStyle lineStyle = LineStyle.Solid) {

            return new StatisticTrait<SimsStatistic>(name, func, relaxationPeriod, relaxationStreategy, thickness, lineStyle);
        }
    }
}
