using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ei.Logs;
using Ei.Runtime;

namespace Ei.Ontology.Actions
{
    

    public class ActionJoinWorkflow : ActionBase
    {
        // parameters

        public class Parameters : ActionParameters
        {
            public int InstanceId { get; set; }

            public override void Apply(VariableState institutionState, VariableState workflowState, VariableState agentState) { 
            }

            public override void Parse(ActionParameter1[] parameters) {
            }
        }

        // fields

        private Institution ei;
        private int instanceId;
        private Workflow testWorkflow;

        // properties

        public string WorkflowId { get; }
        public List<Workflow> Workflows { get; }

        public Workflow TestWorkflow
        {
            get
            {
                if (this.testWorkflow == null)
                {
                    this.testWorkflow = this.ei.CreateWorkflow(this.WorkflowId, null, 0);
                }
                return this.testWorkflow;
            }
        }

        // ctor

        public ActionJoinWorkflow(string id, Institution institution, string workflowId)
            : base(institution, id)
        {
            this.ei = institution;

            // get the workflow
            
            this.WorkflowId = workflowId;
            this.Workflows = new List<Workflow>();

            // add default parameter
            this.instanceId = -1;
        }

//        private ActionJoinWorkflow(Workflow workflow)
//        {
//            this.WorkflowId = workflow.Id;
//        }

        public Workflow Create(Governor performer, ActionParameters parameters = null)
        {
 //           Console.WriteLine("[THREAD] Creating workflow ...: " + Thread.CurrentThread.ManagedThreadId);

            var newWorkflow = this.ei.CreateWorkflow(this.WorkflowId, performer.Workflow, this.instanceId++);
            
            // initialise parameters
            if (parameters != null)
            {
                parameters.Apply(this.ei.VariableState, newWorkflow.VariableState, performer.VariableState);
            }

            // set owner to the agent that created this workflow
            newWorkflow.VariableState.Owner = performer.VariableState;

            this.Workflows.Add(newWorkflow);

            Log.Info(newWorkflow.Name, InstitutionCodes.WorkflowStarted, 
                newWorkflow.Name, 
                newWorkflow.Id, 
                newWorkflow.ToString());

            return newWorkflow;
        }

        protected override IActionInfo PerformAction(Governor agent, Connection connection, ActionParameters parameters)
        {
            var workflow = ei.GetWorkflow(this.WorkflowId);
            var joinParameters = parameters as ActionJoinWorkflow.Parameters;

            // lazily load

            if (this.Workflows.Count == 0)
            {              
                if (workflow.Static)
                {
                    this.Create(agent);
                }
            }

            // handle static workflow

            if (this.Workflows.Count == 1 && this.Workflows[0].Static)
            {
                agent.EnterWorkflow(connection, this.Workflows[0]);

                return ActionInfo.OkButDoNotContinue;
            }

            // handle statefull workflow

            // 1. join existing workflow

            if (joinParameters.InstanceId >= 0) {
                var workflowInstanceId = joinParameters.InstanceId;
                var workflowInstance = this.Workflows.Find(w => w.InstanceId == workflowInstanceId);

                if (workflowInstance == null) {
                    return new ActionInfo(InstitutionCodes.WorkflowInstanceNotRunning);
                }
                agent.EnterWorkflow(connection, workflowInstance);
            }

            // 2. create new workflow instance

            else if (workflow.CreatePermissions.CanAccess(agent.Groups, agent.VariableState))
            {
                var newWorkflow = this.Create(agent);
                agent.EnterWorkflow(connection, newWorkflow);
            }

            // 3. agent cannot enter the requested workflow

            else
            {
                return ActionInfo.Failed;
            }
            
            return ActionInfo.OkButDoNotContinue;
        }

        public WorkflowInfo[] GetWorkflows(Governor governor)
        {
            return this.Workflows.Select(w => w.GetInfo(governor)).ToArray();
        }
    }
}
