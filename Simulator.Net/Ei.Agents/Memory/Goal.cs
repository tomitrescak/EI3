using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Planning.Memory
{
    public class Goal : IGoal
    {
        private object value;


        public string Name { get; set; }
        public string RawValue { get; set; }
        public string Type { get; set; }
        public StateGoalStrategy Strategy { get; set; }

        private object updateValue;

        public string UpdateType { get; set; }
        public string RawUpdateValue { get; set; }
        public GoalUpdateStrategy UpdateStrategy { get; set; }

        public object Value {
            get {
                if (this.value == null) {
                    this.value = this.Parse(this.RawValue, this.Type);
                }
                return value;
            }
        }

        public object UpdateValue {
            get {
                if (this.updateValue == null) {
                    this.updateValue = this.Parse(this.RawUpdateValue, this.UpdateType);
                }
                return this.updateValue;
            }
        }

        // ctors

        public Goal() { }

        public Goal(GoalState goal) {
            this.Name = goal.Name;
            this.Strategy = goal.Strategy;

            this.RawValue = goal.Value.ToString();
            this.Type = goal.Value.GetType().Name;

            if (goal.UpdateStrategy != GoalUpdateStrategy.None) {
                this.UpdateStrategy = goal.UpdateStrategy;
                this.RawUpdateValue = goal.UpdateValue.ToString();
                this.UpdateType = goal.UpdateValue.GetType().Name;
            }
        }

        private object Parse(string value, string type) {
            switch (type) {
                case "String":
                    return value;
                case "Int32":
                    return int.Parse(value);
                case "Double":
                    return double.Parse(value);
                default:
                    throw new NotImplementedException(string.Format("Type '{0}' is not implemented", type));
            }
        }

        // public methods

        public bool Fulfils(IGoal goal) {
            if (this.Name != goal.Name) {
                return false;
            }

            // check update startegies
            if (goal.UpdateStrategy != GoalUpdateStrategy.None 
                && goal.UpdateStrategy == this.UpdateStrategy
                && goal.Strategy == this.Strategy
                && goal.UpdateValue.Equals(this.updateValue)) {
                return true;
            }

            // check normal strategies
            if (this.Strategy != goal.Strategy) {
                return false;
            }
            
            if (this.Strategy == StateGoalStrategy.Equal) {
                return goal.Value.Equals(this.Value);
            }
            else if (this.Strategy == StateGoalStrategy.Min) {
                if (this.Type == "Double") {
                    return (double)this.Value >= (double)goal.Value;
                }
                return (int)this.Value >= (int)goal.Value;
            }
            else if (this.Strategy == StateGoalStrategy.Max) {
                if (this.Type == "Double") {
                    return (double)this.Value <= (double)goal.Value;
                }
                return (int)this.Value <= (int)goal.Value;
            }
            throw new NotImplementedException("Strategy not implemented");
        }
    }
}
