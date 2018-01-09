using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;

namespace Ei.Runtime.Planning.Storage
{
    public class ListStorage : IStorage
    {
        private readonly List<AStarNode> opened; // maybe order by gscore?
        private readonly List<AStarNode> closed;

        public ListStorage()
        {
            this.opened = new List<AStarNode>(16);
            this.closed = new List<AStarNode>(16);
        }

        public AStarNode FindOpened(Connection arc)
        {
            return this.opened.FirstOrDefault(w => w.Arc == arc);
        }

        public AStarNode FindClosed(Connection arc)
        {
            return this.closed.FirstOrDefault(w => w.Arc == arc);
        }

        public bool HasOpened()
        {
            return this.opened.Count > 0;
        }

        public void RemoveOpened(AStarNode node)
        {
            this.opened.Remove(node);
        }

        public void RemoveClosed(AStarNode node)
        {
            this.closed.Remove(node);
        }

        public bool IsOpen(AStarNode node)
        {
            return this.opened.Contains(node);
        }

        public bool IsClosed(AStarNode node)
        {
            return this.closed.Contains(node);
        }

        public void AddToOpenList(AStarNode node)
        {
            this.opened.Add(node);
        }

        public void AddToClosedList(AStarNode node)
        {
            this.closed.Add(node);
        }

        public AStarNode RemoveCheapestOpenNode()
        {
            var lowestHeuristic = float.MaxValue;
            var lowestVal = float.MaxValue;
            var lowestIdx = -1;

            for (var i = 0; i < this.opened.Count; i++)
            {
                if (this.opened[i].F_Score < lowestVal ||
                    Math.Abs(this.opened[i].F_Score - lowestVal) < float.Epsilon && this.opened[i].G_Score <= lowestHeuristic)
                {
                    lowestVal = this.opened[i].F_Score;
                    lowestHeuristic = this.opened[i].G_Score;
                    lowestIdx = i;
                }
            }
            var val = this.opened[lowestIdx];
            this.opened.RemoveAt(lowestIdx);
            return val;
        }
    }
}
