﻿using System.Collections.Generic;

namespace Ei.Runtime
{
    public interface IGovernorCallbacks
    {
        void EnteredInstitution(string id, string name);
        void ExitedInstitution(string id, string name);

        void EnteredWorkflow(string s, string id, string name);
        void ExitedWorkflow(string oldWorkflowId, string oldWorkflowName, string newWorkflowId, string newWorkflowName);

        void ChangedPosition(string name, string workflowId, int workflowInstanceId, string positionId);
        void WaitingForDecision();
        void NotifyWorkflowParameterChanged(string name, string workflowId, int workflowInstanceId, string parameterName, object value);
        void NotifyAgentParameterChanged(string name, string parameterName, object value);
        void Blocked();
        void NotifyActivity(string name, string workflowId, int workflowInstanceId, string agentName, string activityId, VariableState parameters);
        void NotifyActivityFailed(string name, string id, int instanceId, string agentName, string activityId, VariableState values);
        void Split(Governor[] clones, bool shallowClone);
        void Joined();
        
    }
}
