namespace Ei.Ontology
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Diagnostics;
    using System.Threading;
    using Ei.Logs;
    using Ei.Runtime;
    using ActionBase = Ei.Ontology.Actions.ActionBase;
    using Timer = System.Timers.Timer;
    using System;
    using Ei.Runtime.Planning;

    public abstract class Institution : Entity, IStateProvider
    {
        public class InstitutionState
        {
            private readonly Institution ei;
            public InstitutionState(Institution ei) {
                this.ei = ei;

            }
            public long TimeMs { get { return this.ei.Time.ElapsedMilliseconds; } }
            public float TimeSeconds { get { return this.ei.Time.ElapsedMilliseconds / 1000f; } }
        }

        #region Fields
        private int idProvider;
        private readonly List<Role> roles;
        private readonly List<Organisation> organisations;
        private readonly List<Workflow> workflows;
        #endregion

        #region Properties

        public abstract Institution.InstitutionState Resources { get; }

        public ReadOnlyCollection<Role> Roles { get; }

        public ReadOnlyCollection<Organisation> Organisations { get; }

        public ReadOnlyCollection<Connection> GlobalConnections { get; }

        public ReadOnlyCollection<ActionBase> GlobalActions { get; }

        public Dictionary<string, string> GlobalFunctions { get; }

        public ReadOnlyCollection<Workflow> Workflows { get;}

        public string MainWorkflowId { get; protected set; }

        public Security AuthenticationPermissions { get; }

        public abstract Institution Instance { get; }

        // time and properties

        public Stopwatch Time { get; protected set; }

        public ManualResetEvent Pauser { get; protected set; }

        #endregion

        // constructor

        protected Institution(string id) : base(id) {
            Log.Info("Institution instantiated ...");

            this.AuthenticationPermissions = new Security(this);

            this.roles = new List<Role>();
            this.Roles = new ReadOnlyCollection<Role>(this.roles);

            this.organisations = new List<Organisation>();
            this.Organisations = new ReadOnlyCollection<Organisation>(this.organisations);

            this.workflows = new List<Workflow>();
            this.Workflows = new ReadOnlyCollection<Workflow>(this.workflows);
        }

        // constructor helpers

        protected void AddRoles(params Role[] role) {
            this.roles.AddRange(role);
        }

        protected void AddOrganisations(params Organisation[] organisation) {
            this.organisations.AddRange(organisation);
        }

        protected void AddWorkflows(params Workflow[] workflows) {
            this.workflows.AddRange(workflows);
        }

        // public methods

        public abstract void Start();

        public Workflow GetWorkflow(string id) {
            return this.Workflows.First(w => w.Id == id);
        }

        public Workflow.Instance CreateWorkflow(string id, Workflow.Instance parentWorkflow) {
            var workflow = this.Workflows.FirstOrDefault(w => w.Id == id);
            if (workflow == null) {
                throw new Exception("This workflow does not exist: " + id);
            }
            return workflow.StartWorkflow(parentWorkflow);
        }

        public Organisation OrganisationById(string organisationId) {
            return organisationId == "0" || organisationId == null ? null :
                this.Organisations.First(w => w.Id == organisationId);
        }

        public Role RoleById(string roleId) {
            return roleId == "0" || roleId == null ? null :
                this.Roles.FirstOrDefault(w => w.Id == roleId);
        }

        public Group GroupByName(params string[] role) {
            if (role.Length == 1) {

                if (this.Organisations.Count == 0) {
                    throw new InstitutionException("You need to define at least one organisation");
                }

                var org = this.Organisations[0];
                var rl = this.Roles.FirstOrDefault(w => w.Name == role[0].Trim());

                if (rl == null) {
                    throw new InstitutionException("Role does not exist: " + role[0]);
                }

                // role possibly does not exists
                return rl == null ? null : new Group(org, rl);
            }

            if (role.Length == 2) {
                var org = string.IsNullOrEmpty(role[0]) 
                    ? this.Organisations[0]
                    : this.Organisations.FirstOrDefault(w => w.Name == role[0].Trim());
                if (org == null) {
                    throw new InstitutionException("Organisation does not exist: " + role[0]);
                }
                var rl = this.Roles.FirstOrDefault(w => w.Name == role[1].Trim());
                if (rl == null) {
                    throw new InstitutionException("Role does not exist: " + role[1]);
                }
                return (rl == null || org == null) ? null : new Group(org, rl);
            }

            return null;
        }

        public Group GroupById(params string[] role) {
            if (role.Length == 1) {

                if (this.Organisations.Count == 0) {
                    throw new InstitutionException("You need to define at least one organisation");
                }

                var org = this.Organisations[0];
                var rl = this.Roles.FirstOrDefault(w => w.Id == role[0].Trim());

                if (rl == null) {
                    throw new InstitutionException("Role does not exist: " + role[0]);
                }

                // role possibly does not exists
                return rl == null ? null : new Group(org, rl);
            }

            if (role.Length == 2) {
                var org = string.IsNullOrEmpty(role[0]) 
                    ? this.Organisations[0]
                    : this.Organisations.FirstOrDefault(w => w.Id == role[0].Trim());
                if (org == null) {
                    throw new InstitutionException("Organisation does not exist: " + role[0]);
                }
                var rl = this.Roles.FirstOrDefault(w => w.Id == role[1].Trim());
                if (rl == null) {
                    throw new InstitutionException("Role does not exist: " + role[1]);
                }
                return (rl == null || org == null) ? null : new Group(org, rl);
            }

            return null;
        }

        public Group[] GroupsByName(params string[] roleNames) {
            var roleList = new Group[roleNames.Length / 2];
            for (int i = 0; i < roleNames.Length / 2; i++) {
                // in case there is only one organisation we allow users to specify only a role with no organisation
                var orgRole = this.GroupByName(roleNames[i*2], roleNames[i * 2 + 1]);
                if (orgRole == null) return null;
                roleList[i] = orgRole;
            }
            return roleList;
        }

        public Group[] GroupsByName(string[][] roleNames) {
            var roleList = new Group[roleNames.Length];
            for (int i = 0; i < roleNames.Length; i++) {
                // in case there is only one organisation we allow users to specify only a role with no organisation
                var orgRole = this.GroupByName(roleNames[i][0], roleNames[i][1]);
                if (orgRole == null) return null;
                roleList[i] = orgRole;
            }
            return roleList;
        }

        // internal methods

        internal int CreateId() {
            return this.idProvider++;
        }

        public void NotifyParameterChanged(string ownerString, object paramValue) {
            // we do not notify about institutional changes for now
        }
    }

    public abstract class Institution<T> : Institution where T : Institution.InstitutionState
    {
        // fields

        private Timer expressionTimer;
        private readonly List<Action<T>> expressions;
        // properties

        public Action<T> Expression { get;  }

        // constructor

        protected Institution(string id) : base(id) {
            this.expressions = new List<Action<T>>();

            this.Start();
        }

        // methods 

        public void AddExpressions(params Action<T>[] expressions) {
            this.expressions.AddRange(expressions);
        }

        public override void Start() {
            // init pauser that allows us to pause the institution
            this.Pauser = new ManualResetEvent(true);

            // start timer

            this.Time = new Stopwatch();
            this.Time.Start();

            // init expression evaluation
            if (this.Expression == null) return;
            
            this.expressionTimer = new Timer {
                Interval = 100,
                AutoReset = true
            };

            this.expressionTimer.Elapsed += (sender, args) => {
                this.Expression((T)this.Resources);
            };

            this.expressionTimer.Start();
        }
    }
}
