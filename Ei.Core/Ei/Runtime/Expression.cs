//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using Ei.Logs;
//using Ei.Ontology;

//namespace Ei.Runtime
//{

//    public abstract class EiExpression
//    {
//        private const string GetReg = @"([a-zA-Z]\w*\.\w+(\.\w*)*)";
//        private const string GetReplacement = "get('$1')";
//        private const string FuncReg = @"\$(\w+)(\([^\)]*\))?"; // matches all function as $fnc(minim)

//        //this.Conditions[index].Contains("this.") ||
//        //this.Conditions[index].Contains("w.last.") ||
//        //this.Conditions[index].Contains("w.owner.") ||
//        //this.Conditions[index].Contains("i.")
//        public abstract bool IsRuntimeExpression { get; }
//        //  a.
//        public abstract bool HasAgentParameters { get; }
//        // this.
//        public abstract bool HasActivityParameters { get; }

//        public abstract object Evaluate(VariableState state, bool planningMode = false);

//        public abstract bool Check(VariableState state, bool planningMode = false);
//    }
//}
