
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence{
    public enum VariableAccess
    {
        Public,
        Private
    }

    /**
     * 
     */
    public class ParameterDao {

        public bool IsNullable {
            get {
                return this.Type != "int" && this.Type != "float";
            }
        }

        internal string Id => this.Name.ToId();

        /**
         * Name of the parameter
         */
        public string Name { get; set; }

        /**
         * Type of the parameter (for example int, string, CustomType). Type cannot contain spaces.
         */
        public string Type { get; set; }

        public VariableAccess AccessType { get; set; }

        /**
         * Default value for the parameter.
         */
        public string DefaultValue { get; set; }

        ///**
        // * Minimal value for the parameter.
        // */
        //public string MinValue { get; set; }

        ///**
        // * Maximal value for the parameter.
        // */
        //public string MaxValue { get; set; }

        ///**
        // * Access details for the parameter specifying organisations and roles which can access the parameter
        // */
        //public AccessConditionDao[] AllowAccess { get; set; }

        //public AccessConditionDao[] DenyAccess { get; set; }

        ///**
        // * Decides, whether value of this parameter is optional.
        // */
        //public bool Optional { get; set; }

        //public string Consolidation { get; set; }
    }
}