using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Logs;
using Ei.Core.Ontology;

namespace Ei.Core.Runtime.Planning.Heuristics
{
    internal class ResourceBasedHeuristics : IHeuristics
    {
        private GoalState[] goals;

        internal ResourceBasedHeuristics(GoalState[] goals)
        {
            this.goals = goals;
        }

        public float Calculate(AStarNode from)
        {
            // TODO: Make sure that this is using a correct stategy
            double heuristic = 0;
            foreach (var goal in goals)
            {
                var difference = goal.Difference(from.Resources);

                if (difference > 0)
                {
                    if (Log.IsDebug) {
                        if (Log.IsDebug) Log.Debug("Planner", string.Format("Heuristic for '{0}': {1} [Now: {2} -> Goal: {3}]", goal.Name, difference * 10, goal.CurrentValue(from.Resources), goal.Value));
                    }
                }

                heuristic += difference*10;

//                if (param.Definition.Type == "bool" || param.Definition.Type == "string")
//                {
//                    if (goal.Strategy == StateGoalStrategy.Equal)
//                    {
//                        heuristic += goal.Value == param.Value ? 0 : 1;
//                    }
//                    else {
//                        throw new NotImplementedException("This type is not implemented");
//                    }
//                }
//                   
//                else if (param.Definition.Type == "int")
//                {
//                    var gv = (int) goal.Value;
//                    var pv = (int) param.Value;
//
//                    if (goal.Strategy == StateGoalStrategy.Equal)
//                    {
//                        heuristic += goal.Value == param.Value ? 0 : Math.Abs(gv - pv);
//                    }
//                    else if (goal.Strategy == StateGoalStrategy.Min)
//                    {
//                        heuristic += gv - pv < 0 ? 0 : gv - pv;
//                    }
//                    else if (goal.Strategy == StateGoalStrategy.Max)
//                    {
//                        heuristic += pv - gv < 0 ? 0 : pv - gv;
//                    }
//                    heuristic *= 10;
//                }
//                else
//                {
//                    throw new NotImplementedException("This type is not implemented");
//                }
                
            }

            return (float) heuristic;
        }

        public bool CheckGoal(AStarNode currentNode)
        {
            return this.goals.All(goal => goal.IsValid(currentNode.Resources));
        }
    }
}
