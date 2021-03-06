﻿using Ei.Ontology;
using Ei.Runtime.Planning;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Ei.Runtime
{
    public interface IVariableStateCreator
    {
        VariableState Instance(VariableState cloneFrom);
    }

    public class VariableStateCreator<T>: IVariableStateCreator where T : VariableState
    {
        private static Func<VariableState, T> _new;
        public static Func<VariableState, T> New {
            get {
                if (_new == null) {
                    var constructor = typeof(T).GetConstructor(new[] { typeof(VariableState) });
                    var parameter = Expression.Parameter(typeof(VariableState), "variableState");
                    var constructorExpression = Expression.New(constructor, parameter);
                    _new = Expression.Lambda<Func<VariableState, T>>(constructorExpression, parameter).Compile();
                }
                return _new;
            }
        }

        public VariableState Instance(VariableState cloneFrom) {
            return New(cloneFrom);
        }
    }

    public abstract class VariableState
    {
        // fields
        private List<object> defaultValues;

        // properties

        public IVariableDefinition[] Descriptors { get; private set; }
        private List<object> DefaultValues {
            get {
                if (this.defaultValues == null) {
                    this.defaultValues = this.Descriptors.Select(d => d.DefaultValue).ToList();
                }
                return this.defaultValues;
            }
        }

        // ctor
        static Dictionary<Type, IVariableDefinition[]> typeDescriptors = new Dictionary<Type, IVariableDefinition[]>();
        static Dictionary<Type, IVariableStateCreator> stateCreators = new Dictionary<Type, IVariableStateCreator>();

        public VariableState() {
            // get type descriptors from cache
            if (!typeDescriptors.ContainsKey(this.GetType())) {
                var properties = this.GetType().GetProperties().Where(p => p.Name != "Descriptors").ToArray();
                var variableDefinitions = new IVariableDefinition[properties.Length];

                for (var i=0; i<properties.Length; i++) {
                    // create generic type dynamically
                    var genericListType = typeof(VariableDefinition<,>);
                    var specificListType = genericListType.MakeGenericType(properties[i].PropertyType, this.GetType());

                    variableDefinitions[i] = (IVariableDefinition) Activator.CreateInstance(specificListType, new object[] { this, properties[i].Name });
                }

                typeDescriptors[this.GetType()] = variableDefinitions;
                
            }
            this.Descriptors = typeDescriptors[this.GetType()];
        }

        public VariableState(IVariableDefinition[] descriptors) {
            this.Descriptors = descriptors;
        }

        public VariableState(VariableState state) {
            this.Descriptors = state.Descriptors;
            this.Merge(state, true);
        }

        public VariableState(VariableInstance[] variables): this() {
            this.Parse(variables);
        }


        // public methods

        public VariableState Parse(VariableInstance[] properties) {
            foreach (var property in properties) {
                this.GetVariableDefiniton(property.Name).Parse(this, property.Value);
            }
            return this;
        }

        public void Merge(VariableState state, bool clone = false ) {
            foreach (var variable in state.Descriptors) {
                // only merge non default values
                if (clone 
                    || variable.DefaultValue != null && !variable.DefaultValue.Equals(variable.Value(state))) {
                    variable.Update(this, variable.Value(state));
                }
            }
        }

        public object GetValue(string name) {
            return this.Descriptors.First(d => d.Name == name).Value(this);
        }

        public IVariableDefinition GetVariableDefiniton(string name) {
            return this.Descriptors.First(d => d.Name == name);
        }

        public IVariableDefinition FindByName(string name) {
            return this.Descriptors.FirstOrDefault(d => d.Name == name);
        }

        public VariableInstance[] FilterByAccess(Governor governor) {
            // TODO: Further optimisations possible, by caching results
            return this.Descriptors
                .Where(p => p.CanAccess(governor.Groups, governor.VariableState))
                .Select(p => new VariableInstance(p.Name, p.Value(this).ToString()))
                .ToArray();
        }

        public virtual VariableState Clone(VariableState state = null) {
            // IMPORTANT: for bigger performance, inherited methods can override clone behaviours
            // and set values manually

            state = state == null ? this : state;
            // we cache instance creators for increased performance
            if (!stateCreators.ContainsKey(state.GetType())) {
                var genericType = typeof(VariableStateCreator<>);
                var specificType = genericType.MakeGenericType(state.GetType());
                stateCreators[state.GetType()] = (IVariableStateCreator) Activator.CreateInstance(specificType); 
            }
            return stateCreators[state.GetType()].Instance(this);
        }

        public GoalState[] ToGoalState(bool onlyDirty = false) {
            var list = new List<GoalState>();

            for (int i = 0; i < this.DefaultValues.Count; i++) {
                // we either return dirty ones or all of them
                if (!onlyDirty
                    || this.DefaultValues[i] == null && this.Descriptors[i].Value(this) != null
                    || this.DefaultValues[i] != null && !this.DefaultValues[i].Equals(this.Descriptors[i].Value(this))) {
                    list.Add(new GoalState(this.Descriptors[i].Name, this.Descriptors[i].Value(this), StateGoalStrategy.Equal, this.Descriptors[i]));
                }
            }
            return list.ToArray();
        }

        public void ResetDirty() {
            this.defaultValues = this.Descriptors.Select(d => d.Value(this)).ToList();
        }
    }
}
