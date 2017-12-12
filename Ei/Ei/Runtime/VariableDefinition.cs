using Ei.Ontology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ei.Runtime
{
    public enum VariableAccess
    {
        Public,
        Protected,
        Private
    }

    public interface IVariableDefinition
    {
        string Name { get; }
        object DefaultValue { get; }
        object TypeDefault { get; }
        VariableAccess Access { get; }
        bool CanAccess(Group[] groups, ResourceState state);
        object Value(ResourceState state);
        void Update(ResourceState state, object value);
        void Parse(ResourceState state, string value);
        object ParseValue(string value);
    }

    public class VariableAttribute : System.Attribute
    {
        public object DefaultValue { get; set; }
        public VariableAccess Access { get; set; }
    }

    public struct VariableDefinition<T, V> : IVariableDefinition where V : ResourceState
    {
        public string Name { get; private set; }
        public VariableAccess Access { get; private set; }
        public T Default;
        public object TypeDefault { get { return default(T); } }

        public delegate T ParseDelegate(string value);

        private Func<V, T> selector;
        private Action<V, T> updater;
        private ParseDelegate parser;

        public VariableDefinition(ResourceState state, string name) : this(state, name, default(T), VariableAccess.Public) { }

        public VariableDefinition(ResourceState state, string name, T defaultValue, VariableAccess access = VariableAccess.Public) {
            this.Name = name;
            this.Access = access;
            this.Default = defaultValue;

            var parameter = typeof(V).GetProperty(name);
            this.selector = BuildTypedGetter(parameter);
            this.updater = BuildTypedSetter(parameter);

            // find parse method
            MethodInfo parseMethod = typeof(T).GetMethod("Parse",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(string) },
                null);
            if (parseMethod != null) {
                this.parser = (ParseDelegate)Delegate.CreateDelegate(typeof(ParseDelegate), parseMethod);
            }
            else if (typeof(T) == typeof(String)) {
                this.parser = Identity;
            }
            else {
                this.parser = null;
                // throw new Exception("DataType needs to define a Parse method");
            }

            // find custom attributes
            foreach (var ca in parameter.GetCustomAttributes(false)) {
                var variable = ca as VariableAttribute;
                if (variable != null) {
                    this.Access = variable.Access;
                    if (variable.DefaultValue != null) {
                        if (state == null) {
                            throw new Exception("You need to provide state to store the default value");
                        }
                        this.Default = (T)variable.DefaultValue;
                        this.Update(state, variable.DefaultValue);
                    }
                }
            }

        }

        public VariableDefinition(string name, VariableAccess access, T defaultValue, ParseDelegate parser, Func<V, T> selector, Action<V, T> updater = null) {
            this.Name = name;
            this.Access = access;
            this.Default = defaultValue;

            this.selector = selector;
            this.updater = updater;
            this.parser = parser;
        }


        public object DefaultValue {
            get { return this.Default; }
        }

        public T TypedDefaultValue {
            get { return this.Default; }
        }

        public object Value(ResourceState state) {
            return this.Value((V)state);
        }

        public object Value(V state) {
            return this.selector(state);
        }

        public bool CanAccess(Group[] groups, ResourceState state) {
            if (this.Access == VariableAccess.Protected) {
                throw new NotImplementedException();
            }
            return this.Access == VariableAccess.Public; // || this.Access.CanAccess(groups, state);
        }

        public void Update(ResourceState state, object value) {
            this.Update((V)state, (T)value);
        }

        public void Update(V state, object value) {
            this.updater(state, (T)value);
        }

        public void Parse(ResourceState state, string value) {
            this.Parse((V)state, value);
        }

        public void Parse(V state, string value) {
            this.updater(state, this.parser(value));
        }

        public T Parse(string value) {
            return this.parser(value);
        }

        public object ParseValue(string value) {
            return this.parser(value);
        }

        public static T Identity(object value) {
            return (T)value;
        }

        public static Action<V, T> BuildTypedSetter(PropertyInfo propertyInfo) {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetSetMethod();
            if (methodInfo == null) {
                return null;
            }
            var exTarget = Expression.Parameter(targetType, "t");
            var exValue = Expression.Parameter(typeof(T), "p");
            var exBody = Expression.Call(exTarget, methodInfo,
               Expression.Convert(exValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Action<V, T>>(exBody, exTarget, exValue);
            var action = lambda.Compile();
            return action;
        }

        public static Func<V, T> BuildTypedGetter(PropertyInfo propertyInfo) {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetGetMethod();
            var returnType = methodInfo.ReturnType;

            var exTarget = Expression.Parameter(targetType, "t");
            var exBody = Expression.Call(exTarget, methodInfo);
            var exBody2 = Expression.Convert(exBody, typeof(T));

            var lambda = Expression.Lambda<Func<V, T>>(exBody2, exTarget);

            var action = lambda.Compile();
            return action;
        }
    }
}
