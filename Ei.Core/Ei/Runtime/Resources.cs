using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Core.Ontology;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;

namespace Ei.Core.Runtime
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
        public virtual void Parse(VariableInstance[] properties) { }

        public virtual string Validate() {
            return null;
        }

        public virtual void CopyTo(Workflow.Store workflowStore) { }
    }

    public abstract class ResourceState: BaseState
    {
        public abstract List<VariableInstance> FilterByAccess(Governor governor);
    }

    public abstract class SearchableState : ResourceState
    {
        public abstract object GetValue(string name);

        public abstract void SetValue(string name, object value);

        public abstract bool Contains(string variable);

        public abstract SearchableState Clone(SearchableState cloneto = null);

        public abstract void ResetDirty();

        public abstract GoalState[] ToGoalState();

        public override string ToString() {
            return string.Join("; ", this.FilterByAccess(null).Select(s => s.Name + ": " + s.Value));
        }
    }
    
    public abstract class RoleState : SearchableState
    {
        public abstract Governor.GovernorState Agent { get; }
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

        public override void ResetDirty() {
        }

        public override GoalState[] ToGoalState() {
            return null;
        }
    }
}
