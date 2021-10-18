using System.Collections.Generic;
using System.Linq;
using Ei.Core.Runtime;
using Ei.Logs;
using Ei.Simulation.Simulator;

namespace Ei.Simulation.Behaviours
{
    public class GovernorCallbacks : IGovernorCallbacks
    {
        public SimulationAgent owner;

        public GovernorCallbacks(SimulationAgent agent)
        {
            this.owner = agent;
        }

        public virtual void EnteredInstitution(string id, string name)
        {
            Log.Info(this.owner.Name, "Entered Institution: " + id);
        }

        public virtual void ExitedInstitution(string id, string name)
        {
            Log.Info(this.owner.Name, "Exited institution: " + id);
        }

        public virtual void EnteredWorkflow(string s, string id, string name)
        {
            Log.Info(this.owner.Name, "Entered workflow: " + id);
        }

        public virtual void ExitedWorkflow(string oldWorkflowId, string oldWorkflowName, string newWorkflowId, string newWorkflowName)
        {
            Log.Info(this.owner.Name, "Exited workflow: " + oldWorkflowId);
        }

        public virtual void ChangedPosition(string name, string workflowId, int workflowInstanceId, string positionId)
        {
            Log.Info(this.owner.Name, "Changed position: " + positionId);
        }

        public virtual void WaitingForDecision()
        {
            Log.Info(this.owner.Name, "Waiting for decision");
        }

        public virtual void NotifyWorkflowParameterChanged(string name, string workflowId, int workflowInstanceId, string parameterName, object value)
        {
           Log.Info(this.owner.Name + " " + name + " " + parameterName + ": " + value);
        }

        public virtual void NotifyAgentParameterChanged(string name, string parameterName, object value)
        {
            // Debug.WriteLine("{0} = {1}", parameterName, value);
            Log.Info(name, string.Format("{0} = {1}", parameterName, value));

            //if (parameterName.StartsWith("a."))
            //{
            //    parameterName = parameterName.Substring(2);
            //}

            //var param = this.owner.Properties.FirstOrDefault(w => w.Label == parameterName);
            //if (param == null)
            //{
            //    var property = new AgentProperty(parameterName, value.ToString());
            //    this.owner.AddProperty(property);
            //}
            //else
            //{
            //    param.Value = value.ToString();
            //    this.owner.PropertyValueChanged();
                    
            //}
        }

        public virtual void Blocked()
        {
            Log.Info(this.owner.Name, "Blocked");
        }

        public virtual void NotifyActivity(string name, string workflowId, int workflowInstanceId, string agentName, string activityId,
            ParameterState parameters)
        {
            Log.Info(this.owner.Name, $"Activity Started {name} {agentName}:{workflowId}:{workflowInstanceId} Activity {activityId} {parameters}");
            // TODO: Separate callbacks for Potter
            //            if (activityId == "makePot")
            //            {
            //                var exchangeId = this.owner.Properties.First(w => w.Label == "ExchangeId").Value;
            //
            //                var dic = new Dictionary<string, VariableInstance[]>();
            //                dic.Add("exchangeWorkflow", new[] { new VariableInstance("instanceId", exchangeId) });
            //
            //                // place a new object to the environment
            //                Project.Current.Environment.AddObject("Pot", (int) this.owner.X, (int) this.owner.Y, dic);
            //            }
        }

        public void NotifyActivityFailed(string name, string workflowId, int instanceId, string agentName, string activityId,
            ParameterState parameters)
        {
            Log.Info(this.owner.Name, $"Activity Failes {name} {agentName}:{workflowId}:{instanceId} Activity {activityId} {parameters}");
        }

        public virtual void Split(Governor[] splits, bool shallowClone)
        {
            Log.Info(this.owner.Name, $"Split to {splits.Length} instances, shallow: {shallowClone}");
        }

        public virtual void Joined()
        {
            Log.Info(this.owner.Name, "Joined");
        }
    }
}
