

namespace Ei.Ontology.Transitions
{
    using Ei.Runtime;
    using System.Collections.Generic;
    using System.Linq;

    public class TransitionSplit : Transition
    {
        private bool shallowClone;
        private string[][] names;

        
        /// <summary>
        /// Creates a new transition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="shallow"></param>
        /// <param name="names">[[toId,name]]</param>
        /// <param name="workflow"></param>        
        public TransitionSplit(string id, string name, string description, bool shallow, string[][] names, Workflow workflow)
            : base(id, name, description, workflow)
        {
            this.shallowClone = shallow;
            this.names = names;
        }


        protected override IActionInfo Perform(Governor agent)
        {
            var splits = this.Outs.Count;

            agent.Position.Exit(agent);

            // remove original agent from workflow
            agent.Workflow.RemoveAgent(agent);

            var clones = new Governor[splits];

            // add its clones
            for (var i = 0; i < splits; i++)
            {
                // find name
                var suffix = "_clone_" + (i+1);
                if (this.names != null)
                {
                    var group = this.names.FirstOrDefault(w => w[0] == this.Outs[i].To.Id);
                    if (group != null)
                    {
                        suffix = "_" + group[1];
                    }
                }

                var clone = agent.Clone(splits, suffix, this.shallowClone);
                agent.Workflow.AddAgent(clone);
                // remember this clone
                clones[i] = clone;
            }

            // notify
            agent.NotifySplit(clones, this.shallowClone);

            // start clones
            for (var i = 0; i < splits; i++)
            {
                // flow the clone into appropriate port
                this.Outs[i].Pass(clones[i]); //.To.EnterAgent(clone);
            }


            return ActionInfo.OkButDoNotContinue;
        }
    }
}
