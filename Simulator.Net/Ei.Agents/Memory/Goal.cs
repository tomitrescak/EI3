using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Planning.Memory
{
    public class Goal: IGoal
    {
        private object value;

        public string Name { get; set; }
        public string RawValue { get; set; }
        public string Type { get; set; }
        public StateGoalStrategy Strategy { get; set; }

        public object Value {
            get {
                if (this.value == null) {
                    switch (this.Type) {
                        case "String":
                            this.value = this.RawValue;
                            break;
                        case "Int32":
                            this.value = int.Parse(this.RawValue);
                            break;
                        case "Double":
                            this.value = double.Parse(this.RawValue);
                            break;
                        default:
                            throw new NotImplementedException(string.Format("Type '{0}' is not implemented", this.Type));
                    }
                }
                return value;
            }
        }

        // ctors

        public Goal() { }

        public Goal(GoalState goal) {
            this.Name = goal.Name;
            this.Strategy = goal.Strategy;
            this.RawValue = goal.Value.ToString();
            this.Type = goal.Value.GetType().Name;
        }

        // public methods

        public bool Fulfils(IGoal goal) {
            if (this.Strategy != goal.Strategy) {
                return false;
            }
            if (this.Name != goal.Name) {
                return false;
            }
            if (this.Strategy == StateGoalStrategy.Equal) {
                return goal.Value.Equals(this.Value);
            } else if (this.Strategy == StateGoalStrategy.Min) {
                if (this.Type == "Double") {
                    return (double)this.Value >= (double)goal.Value;
                }
                return (int) this.Value >= (int) goal.Value;
            } else if (this.Strategy == StateGoalStrategy.Max) {
                if (this.Type == "Double") {
                    return (double)this.Value <= (double) goal.Value;
                }
                return (int)this.Value <= (int)goal.Value;
            }
            throw new NotImplementedException("Strategy not implemented");
        }
    }
}
