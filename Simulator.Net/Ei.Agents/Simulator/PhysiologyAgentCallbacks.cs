using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Ei.Logs;
using Ei.Ontology.Actions;
using Ei.Runtime;
using Ei.Runtime.Planning;
using Ei.Runtime.Planning.Strategies;

namespace Ei.Simulator.Core
{
    public class PhysiologyAgentCallbacks : IGovernorCallbacks
    {
        public PhysiologyBasedAgent owner;

        public PhysiologyAgentCallbacks(PhysiologyBasedAgent agent)
        {
            this.owner = agent;
        }

        public virtual void EnteredInstitution(string id, string name)
        {
        }

        public virtual void ExitedInstitution(string id, string name)
        {
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
        }

        public virtual void NotifyWorkflowParameterChanged(string name, string workflowId, int workflowInstanceId, string parameterName, object value)
        {
           Log.Info(this.owner.Name + " " + name + " " + parameterName + ": " + value);
        }

        public virtual void NotifyAgentParameterChanged(string name, string parameterName, object value)
        {
            // Debug.WriteLine("{0} = {1}", parameterName, value);
            Log.Info(name, string.Format("{0} = {1}", parameterName, value));

            if (parameterName.StartsWith("a."))
            {
                parameterName = parameterName.Substring(2);
            }

            var param = this.owner.Properties.FirstOrDefault(w => w.Label == parameterName);
            if (param == null)
            {
                var property = new AgentProperty(parameterName, value.ToString());
                this.owner.View.AddProperty(property);
            }
            else
            {
                param.Value = value.ToString();
            }
        }

        public virtual void Blocked()
        {
        }

        public virtual void NotifyActivity(string name, string workflowId, int workflowInstanceId, string agentName, string activityId,
            ParameterState parameters)
        {
            // TODO: Separate callbacks for Potter
            if (activityId == "makePot")
            {
                var exchangeId = this.owner.Properties.First(w => w.Label == "ExchangeId").Value;

                var dic = new Dictionary<string, VariableInstance[]>();
                dic.Add("exchangeWorkflow", new[] { new VariableInstance("instanceId", exchangeId) });

                // place a new object to the environment
                Project.Current.Environment.AddObject("Pot", (int) this.owner.X, (int) this.owner.Y, dic);
            }
        }

        public void NotifyActivityFailed(string name, string id, int instanceId, string agentName, string activityId,
            ParameterState values)
        {
            
        }

        public virtual void Split(Governor[] splits, bool shallowClone)
        {
            this.owner.MainAgent = splits.First(w => w.Name.EndsWith("Main"));
            this.owner.PhysiologyAgent = splits.First(w => w.Name.EndsWith("Physiology"));
        }

        public virtual void Joined()
        {
        }
    }
}
