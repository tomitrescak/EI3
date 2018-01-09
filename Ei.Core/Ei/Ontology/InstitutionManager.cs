using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Agents;
using Ei.Logs;

namespace Ei.Ontology
{
    using Ei.Runtime;

    public class InstitutionManager
    {
        #region Fields
        public Institution Ei { get; set; }

        public Workflow.Instance MainWorkflow { get; private set; }

        private List<Governor> agents;
        #endregion


        public InstitutionManager() {
            this.agents = new List<Governor>();
        }

        // static

        public static InstitutionManager Launch(Institution ei)
        {
            var manager = new InstitutionManager();
            manager.Start(ei.Instance);

            return manager;
        }

        // properties

        public int ConnectedAgents { get { return this.agents.Count; } }

        //public List<Governor> Agents => this.agents; 

        public bool Start(Institution ei)
        {
            this.Ei = ei;
            this.agents.Clear();

            this.MainWorkflow = this.Ei.CreateWorkflow(this.Ei.MainWorkflowId, null);
            this.agents = new List<Governor>();

            if (Log.IsInfo) Log.Info("Manager", InstitutionCodes.InstitutionStarted, this.Ei.Name);

            return true;
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        public InstitutionCodes Connect(IGovernorCallbacks callback, string organisation, string username, string password, string[][] roles, out Governor governor)
        {
            governor = null;

            if (this.MainWorkflow == null)
            {
                return InstitutionCodes.NotRunning;
            }

            var user = this.Ei.AuthenticationPermissions.Authenticate(organisation, username, password);
            if (user.IsEmpty)
            {
                return InstitutionCodes.AuthentificationFailed;
            }

            var roleList = user.Groups; // in case user does not require to play only a specific role, by default is assigned all roles

            if (roles != null)
            {
                // build role list
                roleList = this.Ei.RolesByName(roles);
                if (roleList == null)
                {
                    return InstitutionCodes.IncorrectRoleOrOrganisation;
                }

                if (!Security.Authorise(user, roleList))
                {
                    return InstitutionCodes.AuthorisationFailed;
                }
            }
            else
            {
                // in case user used authorisation without roles, at least one specific role needs to be preset
                if (roleList.All(w => w.Role == null))
                {
                    return InstitutionCodes.MissingRoleAssignment;
                }
            }
            // create governor
            governor = this.RegisterGovernor(callback, username, roleList);

            // start governor
            governor.Start();

            return InstitutionCodes.Ok;
        }

        public void RemoveGovernor(Governor governor)
        {
            this.agents.Remove(governor);
        }

        

        public virtual Governor CreateGovernor()
        {
            return new Governor();
        }

        private Governor RegisterGovernor(IGovernorCallbacks callback, string user, Group[] roles)
        {
            var governor = this.CreateGovernor();
            governor.Init(callback, user, roles, this);
            this.agents.Add(governor);

            return governor;
        }
    }
}
