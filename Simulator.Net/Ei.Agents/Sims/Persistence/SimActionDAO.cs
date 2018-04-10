using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vittoria.Behaviours;

namespace Ei.Simulation.Sims.Persistence
{

    [DisplayName("Action")]
    public class SimActionDAO : INotifyPropertyChanged
    {
        private string name;
        private int uses;
        public float DurationInMinutes { get; set; }
        private List<string> plan;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name {
            get { return this.name; }
            set { this.name = value; this.OnPropertyChanged("Description"); }
        }
        public int Uses {
            get { return this.uses; }
            set { this.uses = value; this.OnPropertyChanged("Description"); }
        }

        public string Description {
            get {
                this.Modifiers.ForEach(m => {
                    m.PropertyChanged -= PropertyChangedHandler;
                    m.PropertyChanged += PropertyChangedHandler;
                });
                return this.Name + " [" + this.uses + "] " + string.Join(", ", this.Modifiers.Select(m => m.Type + " " + m.Delta));
            }
        }

        public List<PerformanceModifierDAO> Modifiers { get; set; }

        public List<string> Plan {
            get { return plan; }
            set {
                this.plan = value;
                this.OnPropertyChanged("Plan");
            }
        }

        // ctor

        public SimActionDAO() {
            this.Modifiers = new List<PerformanceModifierDAO>();
            this.Plan = new List<string>();
        }

        private void OnPropertyChanged(string name) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, e);
        }

        public override string ToString() {
            return this.Name + ": " + this.Description;
        }
    }
}
