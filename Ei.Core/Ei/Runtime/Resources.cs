using Ei.Ontology;
using Ei.Runtime.Planning;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Ei.Runtime
{ 
    public struct ValidationError
    {
        public string VariableName { get; set; }
        public object CurrentValue { get; set; }
        public string ErrorMessage { get; set; }
    }

    public abstract class BaseState { }

    public abstract class ParameterState: BaseState
    {
        public abstract ParameterState Parse(VariableInstance[] properties);

        public virtual string Validate() {
            return null;
        }
    }

    public abstract class ResourceState: BaseState
    {
        public abstract List<VariableInstance> FilterByAccess(Governor governor);

        public abstract ResourceState Merge(BaseState state);

        protected abstract List<object> DefaultValues { get; }

        public abstract ResourceState NewInstance();
    }

    public abstract class SearchableState : ResourceState
    {
        public abstract object GetValue(string name);

        public abstract bool Contains(string variable);

        public abstract SearchableState Clone();

        public abstract void ResetDirty();

        public abstract GoalState[] ToGoalState(bool onlyDirty = false);
    }
}
