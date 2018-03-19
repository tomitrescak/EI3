﻿namespace Ei.Ontology.Actions
{
    using Ei.Runtime;
    using System;

    public abstract class ActionBase : Entity
    {

        protected ActionBase() { }

        protected ActionBase(Institution ei, string id) : base(id)
        {
        }

        // abstract methods

        public ParameterState ParseParameters(VariableInstance[] properties) {
            if (this.CreateParameters != null) {
                var parameters = this.CreateParameters();
                parameters.Parse(properties);
                return parameters;
            }
            return null;
        }

        public virtual Func<ParameterState> CreateParameters { get; set; }

        protected abstract IActionInfo PerformAction(Governor performer, Connection connection, ParameterState parameters);

        // public methods

        public IActionInfo Perform(Governor agent, Connection connection, ParameterState parameters)
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
