using System.Collections.Generic;
using System.ComponentModel;
using Ei.Simulation.Sims;

namespace Vittoria.Behaviours
{
    [DisplayName("Performance Modifer")]
    public class PerformanceModifierDAO : INotifyPropertyChanged
    {
        private ModifierType type;
        private float delta;

        public event PropertyChangedEventHandler PropertyChanged;

        public ModifierType Type {
            get { return this.type; }
            set { this.type = value;
                this.OnPropertyChanged("Type");
                this.OnPropertyChanged("Description");
            }
        }
        public float Delta {
            get { return this.delta; }
            set { this.delta = value;
                this.OnPropertyChanged("Delta");
                this.OnPropertyChanged("Description");
            }
        }
        public List<PersonalityModifier> PersonalityModifiers { get; set; }

        public PerformanceModifierDAO() {
            this.PersonalityModifiers = new List<PersonalityModifier>();
        }

        private void OnPropertyChanged(string name) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString() {
            return this.Type + ": " + this.Delta;
        }
    }
}
