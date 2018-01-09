//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Ei.Ontology;
//using Ei.Ontology.Activities;
//using Action = Ei.Ontology.Activities.Action;
//
//namespace Ei.Runtime
//{
//    public class AgentGovernor : Governor
//    {
//        private Governor agent;
//
//        public AgentGovernor(Governor governor)
//        {
//            this.agent = governor;
//        }
//
//        public override IAgentState Properties
//        {
//            get { return this.agent.Properties; }
//        }
//
//        public override Workflow Workflow
//        {
//            get { return this.agent.Workflow; }
//        }
//
//        public override Governor Agent { get { return this.agent; } }
//
//        public override WorkflowPosition Position
//        {
//            get { return this.agent.Position; }
//            set { this.agent.Position = value; }
//        }
//
//        public override Group[] Groups
//        {
//            get { return this.agent.Roles; }
//        }
//
//        public override void NotifyWaiting()
//        {
//            this.agent.NotifyWaitForDecision();
//        }
//
//        public override void NotifyBlocked()
//        {
//            this.agent.NotifyAgentBlocked();
//        }
//    }
//}
