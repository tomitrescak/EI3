using Ei.Agents.Sims;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Vittoria.Behaviours
{
    [DisplayName("Sim Object")]
    public class SimObjectDAO : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }

        [Description("Decides after how long time we will recreate this object")]
        public float SeedPeriodInMinutes { get; set; }
        public int MinSeedCount { get; set; }
        public int MaxSeedCount { get; set; }
        [Browsable(false)]
        public float NextSeed { get; set; }

        public List<SimActionDAO> Actions { get; set; }
        public string Description {
            get {
                this.Actions.ForEach(m => {
                    m.PropertyChanged -= PropertyChangedHandler;
                    m.PropertyChanged += PropertyChangedHandler;
                });
                return string.Join("; ", this.Actions.Select(m => m.Description));
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, e);
        }

        public SimObjectDAO() {
            this.Name = "Object";
            this.Count = 0;
            this.Actions = new List<SimActionDAO>();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString() {
            return this.Name + ": " + this.Description;
        }
    }
}
