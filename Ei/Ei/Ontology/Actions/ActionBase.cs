namespace Ei.Ontology.Actions
{
    using Ei.Runtime;

    public abstract class ActionBase : Entity
    {

        protected ActionBase() { }

        protected ActionBase(Institution ei, string id) : base(id)
        {
        }

        // abstract methods

        public virtual ResourceState ParseParameters(VariableInstance[] properties) {
            return null;
        }

        protected abstract IActionInfo PerformAction(Governor performer, Connection connection, ResourceState parameters);

        // public methods

        public IActionInfo Perform(Governor agent, Connection connection, ResourceState parameters)
        {
            // perform Action
            return this.PerformAction(agent, connection, parameters);
        }

        public virtual IActionInfo GetInfo()
        {
            return null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return this.Id;
            }
            return string.Format("{0} [{1}]", this.Name, this.Id);
        }
    }
}
