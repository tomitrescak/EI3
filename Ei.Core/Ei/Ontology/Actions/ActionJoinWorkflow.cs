using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ei.Core;
using Ei.Core.Ontology;
using Ei.Core.Runtime;
using Ei.Logs;

namespace Ei.Core.Ontology.Actions
{


    public class ActionJoinWorkflow : ActionBase
    {
        // parameters

        public class Parameters : ParameterState
        {
            // fields
            public static Parameters Instance = new Parameters();

            public int InstanceId { get; set; }
            public int RunningInstances { get; set; }

            public Parameters() {
                this.InstanceId = -1;
            }

            public override void Parse(VariableInstance[] properties) {
                base.Parse(properties);

                foreach (var property in properties) {
                    switch (property.Name) {
                        case "InstanceId":
                            this.InstanceId = int.Parse(property.Value);
                            break;
                    }
                }
            }
        }

        // fields

        private Institution ei;
        private Workflow.Instance testWorkflow;
        private Workflow workflow;

        // properties

        public string WorkflowId { get; }
        public List<int> Workflows { get; }


        public Workflow.Instance TestWorkflow {
            get {
                if (this.testWorkflow == null) {
                    this.testWorkflow = this.ei.CreateWorkflow(this.WorkflowId, null);
                }
                return this.testWorkflow;
            }
        }

        // ctor

        public ActionJoinWorkflow(string id, Institution institution, string workflowId, Func<ParameterState> parameters)
            : base(institution, id) {
            this.ei = institution;
            this.CreateParameters = parameters ?? (() => new Parameters()); 

            // get the workflow

            this.WorkflowId = workflowId;
            this.Workflows = new List<int>();
        }

        public Workflow.Instance Create(Governor performer, ParameterState parameters = null) {
            //           Console.WriteLine("[THREAD] Creating workflow ...: " + Thread.CurrentThread.ManagedThreadId);
            this.workflow = this.ei.GetWorkflow(this.WorkflowId);
            var newWorkflow = this.ei.CreateWorkflow(this.WorkflowId, performer.Workflow);

            // initialise parameters
            if (parameters != null) {
                parameters.CopyTo(newWorkflow.Resources);
            }

            // set owner to the agent that created this workflow
            newWorkflow.Resources.Owner = performer.Resources;

            this.Workflows.Add(newWorkflow.InstanceId);

            if (Log.IsInfo) Logger.Info(null, "Workflow", InstitutionCodes.WorkflowStarted,
                newWorkflow.Name,
                newWorkflow.Id,
                newWorkflow.ToString());

            performer.Resources.CreatedInstanceId = newWorkflow.InstanceId;

            return newWorkflow;
        }

        protected override IActionInfo PerformAction(Governor agent, Connection connection, ParameterState parameters) {
            this.workflow = ei.GetWorkflow(this.WorkflowId);
            var joinParameters = parameters as ActionJoinWorkflow.Parameters;
            if (joinParameters == null) {
                // no parameters are specified, try to create the workflow
                joinParameters = new Parameters {
                    InstanceId = -1
                };
            }
            joinParameters.RunningInstances = this.Workflows.Count;

            Workflow.Instance workflowInstance = null;

            // 1. create a new workflow or join static workflow
            if (joinParameters.InstanceId == -1) {
                // static workflow are lazily created 
                // TODO: Check logic of static workflows wheter lazy loading is ok
                if (workflow.Static && this.Workflows.Count == 0) {
                    workflowInstance = this.Create(agent, parameters);
                } else if (workflow.Static && this.Workflows.Count > 0) {
                    workflowInstance = this.workflow.GetInstance(this.Workflows[0]);
                }
                else if (this.workflow.CreatePermissions == null 
                    || this.workflow.CreatePermissions.CanAccess(agent.Resources, null, joinParameters)) { 
                    workflowInstance = this.Create(agent, parameters);
                } else {
                    return ActionInfo.AccessDenied;
                }
            }

            // 2. join existing workflow

            else {
                var workflowInstanceId = joinParameters.InstanceId;
                workflowInstance = this.workflow.GetInstance(workflowInstanceId);

                if (workflowInstance == null) {
                    return new ActionInfo(InstitutionCodes.WorkflowInstanceNotRunning);
                }
            }

            if (workflowInstance != null) {

                // check access conditions

                if (workflowInstance.Workflow.JoinPermissions != null &&
                    !workflowInstance.Workflow.JoinPermissions.CanAccess(agent.Resources, workflowInstance.Resources)) {
                    return ActionInfo.AccessDenied;
                }
                agent.EnterWorkflow(connection, workflowInstance);
                return ActionInfo.OkButDoNotContinue;
            }

            //// 2. create new workflow instance

            //else if (workflow.CreatePermissions.CanAccess(agent.Resources))
            //{
            //    var newWorkflow = this.Create(agent);
            //    agent.EnterWorkflow(connection, newWorkflow);
            //}

            //// 3. agent cannot enter the requested workflow

            //else
            //{
            //    return ActionInfo.Failed;
            //}

            //return ActionInfo.OkButDoNotContinue;

            if (this.Workflows.Count == 0) {
                return new ActionInfo(InstitutionCodes.WorkflowInstanceNotRunning);
            }

            return ActionInfo.Failed;
        }

        public WorkflowInfo[] GetWorkflows(Governor governor) {
            return this.Workflows.Select(w => this.workflow.GetInstance(w).GetInfo(governor)).ToArray();
        }

    }
}
