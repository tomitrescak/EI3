﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ei.Core.Runtime;

namespace Ei.Core.Ontology.Actions
{
    public class ActionMessage : ActionBase
    {

        // constructor

        public ActionMessage(string id, Institution ei, Func<ParameterState> createParameters, Group[] notifyGroups = null, List<string> notifyAgents = null)
            : base(ei, id) {

            this.CreateParameters = createParameters;

            if (notifyGroups != null) {
                this.NotifyRoles = new ReadOnlyCollection<Group>(notifyGroups);
            }
            if (notifyAgents != null) {
                this.NotifyAgents = new ReadOnlyCollection<string>(notifyAgents);
            }
        }

        #region Properties
        public ReadOnlyCollection<Group> NotifyRoles { get; }

        public ReadOnlyCollection<string> NotifyAgents { get; }
        #endregion

        protected override IActionInfo PerformAction(Governor agent, Connection connection, ParameterState parameters) {
            var result = parameters.Validate();
            if (result != null) {
                return new ActionInfo(InstitutionCodes.InvalidParameters, result);
            }

            agent.LogAction(
                InstitutionCodes.ActionPerformed,
                agent.Name,
                this.Id,
                parameters.ToString()
            );

            // notify roles
            if (this.NotifyRoles != null && this.NotifyRoles.Count > 0) {
                agent.Workflow.NotifyRoles(this.NotifyRoles, agent.Name, this.Id, parameters);
            }
            // notify agents
            if (this.NotifyAgents != null && this.NotifyAgents.Count > 0) {
                agent.Workflow.NotifyAgents(this.NotifyAgents, this.Name, parameters);
            }

            return ActionInfo.Ok;
        }
    }
}
