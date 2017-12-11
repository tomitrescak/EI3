using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;

namespace Ei.Runtime.Planning
{
    public enum StateGoalStrategy
    {
        Min,
        Max,
        Equal
    }


    public class GoalState
    {
        // fields

        private IVariableDefinition variableProvider;

        public string Name;
        public object Value;
        public StateGoalStrategy Strategy;

        // constructors

        public GoalState(string name, object value, StateGoalStrategy strategy, IVariableDefinition variableProvider)
        {
            this.Name = name;
            this.Value = value;
            this.Strategy = strategy;
            this.variableProvider = variableProvider;
        }


        // methods

        public object CurrentValue(Governor.ResourceState state) {
            return this.variableProvider.Value(state.FindProvider(this.Name));
        }

        public float Difference(Governor.ResourceState state) {
            return this.ParameterDifference(this.variableProvider.Value(state.FindProvider(this.Name)));
        }

        private float ParameterDifference(object param)
        {
            if (param == null) return 0;

            if (param is bool || param is string)
            {
                if (this.Strategy == StateGoalStrategy.Equal)
                {
                    return this.Value.Equals(param) ? 0 : 1;
                }
                throw new NotImplementedException("This type is not implemented");
            }

            float goalValue = 0f;
            float currentValue = 0f;

            if (param is int)
            {
                goalValue = (int)this.Value;
                currentValue = (int)param;
            }
            else if (param is float || param is double)
            {
                if (this.Value is float)
                {
                    goalValue = (float) this.Value;
                }
                
                goalValue = this.Value as float? ?? (float) (double)this.Value;
                currentValue = param as float? ?? (float) (double) param;
            }
            else
            {
                throw new NotImplementedException("This type is not implemented");
            }

            if (this.Strategy == StateGoalStrategy.Equal)
            {
                return Math.Abs(goalValue - currentValue);
            }
            if (this.Strategy == StateGoalStrategy.Min)
            {
                return goalValue - currentValue;
            }
            if (this.Strategy == StateGoalStrategy.Max)
            {
                return currentValue - goalValue;
            }
            throw new NotImplementedException("This strategy is not implemented");
        }

        public bool IsValid(Governor.ResourceState state)
        {
            var difference = this.Difference(state);

            return this.Strategy == StateGoalStrategy.Equal && difference < 0.00001 ||
                   this.Strategy == StateGoalStrategy.Min && difference <= 0f ||
                   this.Strategy == StateGoalStrategy.Max && difference <= 0f;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, Strategy == StateGoalStrategy.Equal ? "==" : Strategy.ToString(), Value);
        }

        /// <summary>
        /// Calculates how the goal is changed in between two steps. Returns:
        /// <list type="bullet">
        /// <item>
        /// <description>-1: if state change is going away from the goal</description>
        /// </item>
        /// <item>
        /// <description>0: if there is no change in state</description>
        /// </item>
        /// <item>
        /// <description>1: if reach the final state</description>
        /// </item>
        /// <item>
        /// <description>n: if we need to repeat this action n times to reach the goal</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="changedState"></param>
        /// <returns>
        /// </returns>
        public float GetDeltaRatio(Governor.ResourceState startState, Governor.ResourceState changedState)
        {
            var before = this.Difference(startState);
            var after = this.Difference(changedState);

            if (before <= after)
            {
                return -1;
            }

            if (Math.Abs(after) < 0.00001)
            {
                return 1;
            }

            // check how many times we need to reach the goal

            if (this.Value is float)
            {
                return (before - (float) this.Value) / (before - after);
            }
            if (this.Value is double)
            {
                return (float)(before - (double) this.Value) / (before - after);
            }
            if (this.Value is int)
            {
                return  (before - (int) this.Value) / (before - after);
            }

            throw new NotImplementedException("This is not expected, difference has to be calculated correctly");


            // return (float) after;

            //throw new NotImplementedException();
        }
    }
}
