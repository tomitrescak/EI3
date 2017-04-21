//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Ei.Logs;
//using Ei.Ontology;
//using Ei.Ontology.Activities;
//using Action = Ei.Ontology.Activities.Action;
//
//namespace Ei.Runtime
//{
//    public abstract class Governor
//    {        
//        // abstract methods
//        
//        public abstract IAgentState Properties { get; }
//
//        public abstract Workflow Workflow { get; }
//        public abstract Governor Agent { get; }
//
//        public abstract WorkflowPosition Position { get; set; }
//        public abstract Group[] Groups { get; }
//
//        public abstract void NotifyWaiting();
//        public abstract void NotifyBlocked();
//
//        // public methods
//
//
//        public void LogAction(InstitutionCodes code, params string[] parameters)
//        {
//            Log.Info(code, parameters);
//        }
//
//
//        public void ApplyPostconditions(AccessCondition[] postconditions)
//        {
//            if (postconditions != null && postconditions.Length > 0)
//            {
//                foreach (var postcondition in postconditions)
//                {
//                    if (postcondition.AppliesTo(this.Groups))
//                    {
//                        postcondition.ApplyPostconditions(this.Properties);
//                    }
//                }
//            }
//        }
//
//        public void Continue()
//        {
//            // we may have exited the institution
//            if (this.Position == null)
//            {
//                return;
//            } 
//
//            // filter output arcs
//            var viableArcs = this.Position.ViableTransitions(this);
//
//            if (viableArcs.Length == 0)
//            {
//                return;
//            }
//
//            // we flow only through transitions,if there are more than on possibility, throw error
//            if (viableArcs.Length > 0)
//            {
//                throw new ApplicationException("Ambiguous connection! More than one possible transition from: " + this.Position.Id);
//            }
//
//            var arc = viableArcs[0];
//            if (arc.Pass(this).IsNotOk)
//            {
//                throw new ApplicationException("Transitioning arc failed! " + arc);
//            }
//
//            // enter transition or state
//            arc.To.Enter(this);
//        }
//    }
//}
