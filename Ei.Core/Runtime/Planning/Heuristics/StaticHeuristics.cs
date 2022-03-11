namespace Ei.Core.Runtime.Planning.Heuristics
{

    internal class StaticHeuristics : IHeuristics
    {
        private string goalAction;

        internal StaticHeuristics(string action)
        {
            this.goalAction = action;
        }

        public float Calculate(AStarNode from)
        {
            return from.Heuristic;
        }

        public bool CheckGoal(AStarNode currentNode)
        {
            return currentNode.Arc.Action != null && currentNode.Arc.Action.Id == goalAction;
        }
    }
}
