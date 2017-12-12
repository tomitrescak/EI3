using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;
using Ei.Ontology.Actions;

namespace Ei.Runtime.Planning
{
    public class AStarNode
    {
        public AStarNode Parent { get; set; }

        public float F_Score { get; set; }

        public float G_Score { get; set; }

        public float Heuristic { get; set; }

        public Connection Arc { get; }

        public Governor.GovernorState Resources { get; set; }

        public Governor.GovernorState OriginalResources { get; set; }

        public int Visited { get; set; }

        public int Length
        {
            get
            {
                var length = 1;
                var parent = this.Parent;
                while (parent != null)
                {
                    parent = parent.Parent;
                    length++;
                }
                return length;
            }
        }

        public string CostData { get; set; }
        public AccessCondition AppliedEffect { get; set; }
        public List<AStarNode> Children { get; private set; }

        /// <summary>
        /// 0 - Not expanded
        /// 1 - Expanded
        /// 2 - Error
        /// 3 - Goal
        /// 4 - Non Existent subplan
        /// </summary>
        public byte Status { get; set; }
        public AStarNode(Connection arc)
        {
            this.Arc = arc;
            this.Children = new List<AStarNode>();
        }

        public override string ToString()
        {
            if (this.Arc.Action != null && this.Arc.Action is ActionExitWorkflow) {
                return "Exit workflow";
            }
            var name = this.Arc.ToChainString();
            var parent = this.Parent;
            while (parent != null)
            {
                name = parent.Arc.ToChainString() + " -> " + name;
                parent = parent.Parent;
            }
            //name = parent.Arc.From.Name + " -> ";

            return string.Format("{0} H:{1}, G:{2}, F:{3}", name, this.Heuristic, this.G_Score, this.F_Score);
        }

        public IEnumerable<Connection> ViableConnections(Governor agent, bool binary)
        {
            return this.Arc.To.ViableConnections(agent.Groups, this.Resources);
        }
    }
}
