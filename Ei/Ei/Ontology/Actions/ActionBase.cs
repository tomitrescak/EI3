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

        protected abstract IActionInfo PerformAction(Governor performer, Connection connection, ActionParameters parameters);

        protected virtual void Performed(Governor performer) { }

        // public methods

        public IActionInfo Perform(Governor agent, Connection connection, ActionParameters parameters)
        {
            // perform Action
            var result = this.PerformAction(agent, connection, parameters);

            if (!result.IsAcceptable)
            {
                return result;
            }

            // post process Action, e.g. notify users
            this.Performed(agent);

            return result;
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
