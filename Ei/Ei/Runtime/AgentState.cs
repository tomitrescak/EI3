using Ei.Ontology;
using Ei.Runtime.Planning;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ei.Runtime
{
    public interface IVariableProvider
    {
        string Name { get; }
        object DefaultValue { get; }
        bool CanAccess(Group[] groups, VariableState state);
        object Value(VariableState state);
        
    }

    public struct VariableProperty
    {
        public string Name;
        public object Value;

        public VariableProperty(string name, object value) {
            this.Name = name;
            this.Value = value;
        }
    }

    public struct VariableDescriptor<T> : IVariableProvider
    {
        public string Name { get; private set; }
        public Access Access;
        public T Default;
        public Func<VariableState, T> Selector { get; }

        public VariableDescriptor(string name, Access access, T defaultValue, Func<VariableState, T> selector) {
            this.Name = name;
            this.Access = access;
            this.Default = defaultValue;
            this.Selector = selector;
        }

        public object DefaultValue {
            get { return this.Default; }
        }

        public object Value(VariableState state) {
           return this.Selector(state); 
        }

        public bool CanAccess(Group[] groups, VariableState state) {
            return this.Access == null || this.Access.CanAccess(groups, state);
        }
    }

    public abstract class VariableState
    {
        // fields

        // properties

        private List<IVariableProvider> Descriptors;
        private List<object> DefaultValues;

        // ctor

        public VariableState() { }

        public VariableState(List<IVariableProvider> descriptors) {
            this.Descriptors = descriptors;
            this.DefaultValues = descriptors.Select(d => d.DefaultValue).ToList();
        }

        public VariableState(VariableState state) {
            state.Descriptors = this.Descriptors;
            state.DefaultValues = this.DefaultValues;
        }

        // public methods

        internal object GetValue(string name) {
            return this.Descriptors.Find(d => d.Name == name).Value(this);
        }

        public VariableProperty[] FilterByAccess(Governor governor) {
            // TODO: Further optimisations possible, by caching results
            return this.Descriptors
                .Where(p => p.CanAccess(governor.Groups, governor.VariableState))
                .Select(p => new VariableProperty(p.Name, p.Value(this)))
                .ToArray();
        }

        public virtual VariableState Clone(VariableState state = null) {
            return state;
        }

        public GoalState[] ToGoalState(bool onlyDirty = false) {
            var list = new List<GoalState>();

            for (int i = 0; i < this.DefaultValues.Count; i++) {
                // we either return dirty ones or all of them
                if (!onlyDirty || !this.DefaultValues[i].Equals(this.Descriptors[i].Value(this))) {
                    list.Add(new GoalState(this.Descriptors[i].Name, this.Descriptors[i].Value(this), StateGoalStrategy.Equal, this.Descriptors[i]));
                }
            }
            return list.ToArray();
        }

        public void ResetDirty() {
            this.DefaultValues = this.Descriptors.Select(d => d.Value(this)).ToList();
        }

        
    }
}
