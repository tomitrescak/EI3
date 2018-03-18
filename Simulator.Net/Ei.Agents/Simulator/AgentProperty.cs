using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ei.Simulator.Core
{
    public class AgentProperty : INotifyPropertyChanged
    {
        private string value;

        public AgentProperty(string parameterName, string value)
        {
            this.Label = parameterName;
            this.value = value;
        }

        public string Label { get; set; }

        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
