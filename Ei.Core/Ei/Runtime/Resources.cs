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

        public abstract ResourceState NewInstance();
    }

    public abstract class SearchableState : ResourceState
    {
        public abstract object GetValue(string name);

        public abstract void SetValue(string name, object value);

        public abstract bool Contains(string variable);

        public abstract SearchableState Clone(SearchableState cloneto = null);

        public abstract void ResetDirty();

        public abstract GoalState[] ToGoalState();
    }

    public class EmptyResources : SearchableState
    {
        private List<object> defaultValues = new List<object>();

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = new EmptyResources();
            return clone;
        }

        public override bool Contains(string variable) {
            return false;
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = new List<VariableInstance>();
            return list;
        }

        public override object GetValue(string name) {
            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            throw new Exception("Key does not exists: " + name);
        }

        public override ResourceState Merge(BaseState state) {
            return this;
        }

        public override ResourceState NewInstance() {
            return new EmptyResources();
        }

        public override void ResetDirty() {
        }

        public override GoalState[] ToGoalState() {
            return null;
        }
    }
}
