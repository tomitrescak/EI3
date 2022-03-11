using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ei.Core.Runtime;

namespace Ei.Core.Ontology
{
    public abstract class RelationalEntity : Entity
    {

        // constructors

        protected RelationalEntity(string id): base(id) { }

        protected RelationalEntity(string id, string name, string description) : base(id, name, description) { }

        // properties

        #region Parents
        private RelationalEntity[] parents;
        private RelationalEntity[] AllParents { get; set; }
        public RelationalEntity[] Parents
        {
            get { return this.parents; }
            set
            {
                this.parents = value;
                this.AllParents = this.Search(node => node.Node.Parents);
            }
        }
        #endregion

        #region AllExcludes

        private RelationalEntity[] exclude;
        private RelationalEntity[] AllExcludes { get; set; }
        public RelationalEntity[] Exclude
        {
            get { return this.exclude; }
            set
            {
                this.exclude = value;

                // build all excludes
                this.AllExcludes = this.Search(node => node.Node.Exclude);
            }
        }

        #endregion

        public bool Is(RelationalEntity role)
        {
            if (role == null || this == role)
            {
                return true;
            }

            return this.AllParents == null ? this == role : this.AllParents.Any(w => w == role);
        }

        public bool Excludes(RelationalEntity role)
        {
            return this.AllExcludes.Any(w => w == role);
        }

        // helpers 

        private class SearchNode
        {
            internal RelationalEntity Node { get; }
            private SearchNode Child { get; }

            internal SearchNode(RelationalEntity node, SearchNode child)
            {
                this.Node = node;
                this.Child = child;

                // check for cycles

                var descendant = this.Child;
                while (descendant != null)
                {
                    if (descendant.Node == node)
                    {
                        throw new InstitutionException("Role relations cannot contain loops");
                    }
                    descendant = descendant.Child;
                }
            }
        }

        private RelationalEntity[] Search(Func<SearchNode, RelationalEntity[]> selector)
        {
            // use BFS to find all parents (we do not use DFP because we want to have parents 
            // ordered bottom to top for parameter inheritance)
            // detect cycles during the process

            var result = new List<RelationalEntity>();
            var stack = new Queue<SearchNode>();
            stack.Enqueue(new SearchNode(this, null));

            while (stack.Count > 0)
            {
                var current = stack.Dequeue();

                // add all parent
                var nodes = selector(current);

                if (nodes != null)
                {
                    foreach (var parent in nodes)
                    {
                        stack.Enqueue(new SearchNode(parent, current));

                        // add new parent
                        if (!result.Contains(parent))
                        {
                            result.Add(parent);
                        }
                    }
                }
            }
            return result.ToArray();
        }
    }
}
