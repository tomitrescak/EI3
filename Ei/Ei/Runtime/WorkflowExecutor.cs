//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Ei.Ontology;
//using Ei.Ontology.Activities;
//using Ei.Ontology.Transitions;
//using Action = Ei.Ontology.Activities.Action;
//
//namespace Ei.Runtime
//{
//    public class WorkflowGovernor : Governor
//    {
//        private readonly Workflow workflow;
//        private readonly AgentState properties;
//
//        public WorkflowGovernor(Workflow workflow)
//        {
//            this.workflow = workflow;
//            this.properties = new AgentState(null);
//            this.properties["w"] = workflow;
//        }
//
//        public override Workflow Workflow { get { return this.workflow; } }
//
//        public override Governor Agent { get { throw new ApplicationException("Agent shall never be accessed in Workflow Governor!");} }
//
//        public override IAgentState Properties { get { return this.properties; } }
//
//        public override WorkflowPosition Position
//        {
//            get { return this.workflow.State; }
//            set { this.workflow.State = value; }
//        }
//        public override Group[] Groups { get { return null; } }
//
//        public override void NotifyWaiting()
//        {
//            if (this.workflow.Stateless)
//            {
//                throw new ApplicationException("Stateless workflow shall never wait!");
//            }
//
//            // notify all agents
//            foreach (var agent in workflow.Agents.ToArray())
//            {
//                agent.NotifyWaitForDecision();
//            }
//        }
//
//        public override void NotifyBlocked()
//        {
//            if (this.workflow.Stateless)
//            {
//                throw new ApplicationException("Stateless workflow shall never be blocked!");
//            }
//
//            // notify all agents
//            foreach (var agent in workflow.Agents.ToArray())
//            {
//                agent.NotifyAgentBlocked();
//            }
//        }
//    }
//}
