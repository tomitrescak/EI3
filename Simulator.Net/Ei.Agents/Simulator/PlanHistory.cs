using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Ei.Runtime.Planning;

namespace Ei.Simulator.Core
{
    public class PlanHistory: INotifyPropertyChanged
    {
        private DateTime finish;
        private string result;
        public DateTime Start { get; set; }

        public DateTime Finish
        {
            get { return this.finish; }
            set
            {
                this.finish = value;
                this.OnPropertyChanged("Finish");
            }
        }

        public string Goals { get; set; }

        public string Result
        {
            get { return this.result; }
            set
            {
                this.result = value;
                this.OnPropertyChanged("Result");
            }
        }

        public List<AStarNode> GeneratedPlan { get; set; } 

        public int Items { get; set; }

        public string Message { get; set; }
        public string GoalType { get; set; }
        public bool Failed { get; set; }
        public List<AStarNode> InitialNode { get; set; }
        public string StartString { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
