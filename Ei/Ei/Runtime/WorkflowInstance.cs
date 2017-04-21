//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//
//namespace Ei.Runtime
//{
//    using Ei.Ontology;
//    using Ei.Ontology.Arcs;
//
//    public class WorkflowInstance
//    {
//
//        // fields
//        private int id;
//
//        private Workflow wf;
//
//        private InstitutionInstance ei;
//
//        private List<Governor> agents;
//
//        private List<ParameterInstance> parameters; 
//
//        private WorkflowState currentState;
//
//        // constructors
//
//        public WorkflowInstance(InstitutionInstance ei, Workflow workflow)
//        {
//            this.wf = workflow;
//            this.ei = ei;
//            this.agents = new List<Governor>();
//            this.parameters = ParameterInstance.Init(workflow.Parameters);
//            this.id = this.ei.CreateId();
//        }
//
//        // properties
//
//        private ActionEnterExit InitialActivity => (ActionEnterExit)this.wf.Arcs.First(w => w is ActionEnterExit);
//
//        // public methods
//
//        public bool Join(Governor agent)
//        {
//            if (this.wf.Stateless)
//            {
//                // notify agent
//                agent.OnEnteredWorkflow(this);
//
//                // perform initial arc
//                this.InitialActivity.Perform(agent);
//
//                return true;
//            }
//
//            throw new NotImplementedException();
//        }
//
//        // private methods
//
//        
//
//        public WorkflowInfo GetInfo(Governor agent)
//        {
//            return new WorkflowInfo(
//                this.id,
//                this.wf.Name, 
//                ParameterInstance.FilterByAccess(agent, this.parameters));
//        }
//    }
//
//    
//}
