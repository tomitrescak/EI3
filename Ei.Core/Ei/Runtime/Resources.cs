using Ei.Ontology;
using Ei.Runtime.Planning;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Ei.Runtime
{
    //public interface IResourceStateCreator
    //{
    //    ResourceState Instance(ResourceState cloneFrom);
    //}

    //public class ResourceStateCreator<T>: IResourceStateCreator where T : ResourceState
    //{
    //    private static Func<ResourceState, T> _new;
    //    public static Func<ResourceState, T> New {
    //        get {
    //            if (_new == null) {
    //                var constructor = typeof(T).GetConstructor(new[] { typeof(ResourceState) });
    //                var parameter = Expression.Parameter(typeof(ResourceState), "variableState");
    //                var constructorExpression = Expression.New(constructor, parameter);
    //                _new = Expression.Lambda<Func<ResourceState, T>>(constructorExpression, parameter).Compile();
    //            }
    //            return _new;
    //        }
    //    }

    //    public ResourceState Instance(ResourceState cloneFrom) {
    //        return New(cloneFrom);
    //    }
    //}

    public interface IResourceStateCreator
    {
        ResourceState Instance();
    }

    public class ResourceStateCreator<T> : IResourceStateCreator where T : ResourceState
    {
        private static Func<T> _new;
        public static Func<T> New {
            get {
                if (_new == null) {
                    var constructor = typeof(T).GetConstructor(new Type[0]);
                    // var parameter = Expression.Parameter(typeof(ResourceState), "variableState");
                    var constructorExpression = Expression.New(constructor);
                    _new = Expression.Lambda<Func<T>>(constructorExpression).Compile();
                }
                return _new;
            }
        }

        public ResourceState Instance() {
            return New();
        }
    }

    public struct ValidationError
    {
        public string VariableName { get; set; }
        public object CurrentValue { get; set; }
        public string ErrorMessage { get; set; }
    }

    public abstract class ResourceState
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
        static Dictionary<Type, IResourceStateCreator> stateCreators = new Dictionary<Type, IResourceStateCreator>();

        public ResourceState() {
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

        //public ResourceState(IVariableDefinition[] descriptors) {
        //    this.Descriptors = descriptors;
        //}

        //public ResourceState(ResourceState state) {
        //    this.Descriptors = state.Descriptors;
        //    this.Merge(state, true);
        //}

        //public ResourceState(VariableInstance[] variables): this() {
        //    this.Parse(variables);
        //}


        // public methods

        public ResourceState Parse(VariableInstance[] properties) {
            if (properties != null) {
                foreach (var property in properties) {
                    this.GetVariableDefiniton(property.Name).Parse(this, property.Value);
                }
            }
            return this;
        }

        public ResourceState Merge(ResourceState state, bool clone = false) {
            foreach (var variable in state.Descriptors) {
                // only merge values from "state" that are different from the default value in "state"
                if (clone
                    || variable.DefaultValue != null && !variable.DefaultValue.Equals(variable.Value(state))) {
                    variable.Update(this, variable.Value(state));
                }
            }
            return this;
        }

        public virtual string Validate() {
            return null;
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
                .Where(p => p.CanAccess(governor.Groups, governor.Resources.FindProvider(p.Name)))
                .Select(p => new VariableInstance(p.Name, p.Value(this).ToString()))
                .ToArray();
        }

        public virtual ResourceState Clone(ResourceState state = null) {
            // IMPORTANT: for bigger performance, inherited methods can override clone behaviours
            // and set values manually

            state = state == null ? this : state;
            // we cache instance creators for increased performance
            if (!stateCreators.ContainsKey(state.GetType())) {
                var genericType = typeof(ResourceStateCreator<>);
                var specificType = genericType.MakeGenericType(state.GetType());
                stateCreators[state.GetType()] = (IResourceStateCreator) Activator.CreateInstance(specificType); 
            }
            return stateCreators[this.GetType()].Instance().Merge(state, true);
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

        public override string ToString() {
            return string.Join(",", this.Descriptors.Select(d => d.Name + ": " + d.Value(this)).ToArray());
        }
    }
}
