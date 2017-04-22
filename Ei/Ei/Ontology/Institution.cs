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


    public abstract class Institution : Entity, IStateProvider
    {
        public class InstitutionState: VariableState
        {
            private Institution ei;
            public InstitutionState(Institution ei) {
                this.ei = ei;

            }
            public long TimeMs { get { return this.ei.Time.ElapsedMilliseconds; } } 
            public float TimeSeconds { get { return this.ei.Time.ElapsedMilliseconds / 1000f; } }
        }


        #region Fields
        private int idProvider;
        private List<ActionBase> globalActions;
        private List<Connection> globalConnections;
        private List<Role> roles;
        private List<Organisation> organisations;
        private List<EiExpression> expressions;
        private Timer expressionTimer;

        #endregion

        #region Properties

        public ReadOnlyCollection<Role> Roles { get; protected set; }

        public ReadOnlyCollection<Organisation> Organisations { get; protected set; }

        public ReadOnlyCollection<Connection> GlobalConnections { get; protected set; }

        public ReadOnlyCollection<ActionBase> GlobalActions { get; protected set; }

        public Dictionary<string, string> GlobalFunctions { get; protected set; }

        public ReadOnlyCollection<Workflow> Workflows { get; protected set; }

        public string MainWorkflowId { get; protected set; }

        public Security CreatePermissions { get; protected set; }

        public abstract Institution Instance { get; }

        // time and properties

        public Stopwatch Time { get; private set; }
        
        public ManualResetEvent Pauser { get; private set; }

        public InstitutionState VariableState { get; protected set; }

        #endregion

        // constructor

        protected Institution(string id, string name, string description) : base(id, name, description) {
        }

        protected Institution(string id): base(id)
        {
            if (Log.IsInfo) Log.Info("Institution instantiated ...");

            this.CreatePermissions = new Security();
            this.VariableState = new InstitutionState(this);
        }

        // public methods

        public void Start() {
            // init pauser that allows us to pause the institution
            this.Pauser = new ManualResetEvent(true);

            // start timer

            this.Time = new Stopwatch();
            this.Time.Start();

            // init expression evaluation
            if (this.expressions.Count > 0) {
                this.expressionTimer = new Timer {
                    Interval = 100,
                    AutoReset = true
                };

                this.expressionTimer.Elapsed += (sender, args) => {
                    foreach (var expression in this.expressions) {
                        expression.Evaluate(this.VariableState, false);
                    }
                };
                
                this.expressionTimer.Start();
            }
        }

        public Workflow GetWorkflow(string id) {
            return this.Workflows.First(w => w.Id == id);
        }

        public Workflow CreateWorkflow(string id, Workflow parentWorkflow, int instanceId = 0)
        {
            var workflow = this.Workflows.First(w => w.Id == id);
            return workflow.CreateInstance(this, parentWorkflow, instanceId);
        }

        public Organisation OrganisationById(string organisationId)
        {
            return organisationId == "0" || organisationId == null ? null : 
                this.Organisations.First(w => w.Id == organisationId);
        }

        public Role RoleById(string roleId)
        {
            return roleId == "0" || roleId == null ? null : 
                this.Roles.FirstOrDefault(w => w.Id == roleId);
        }

        public Group GroupByName(string[] role)
        {
            if (role.Length == 1)
            {
                if (this.Organisations.Count > 1)
                {
                    throw new InstitutionException("Ambiguous organisation. More than one possibility for given role"); 
                }

                var org = this.Organisations[0];
                var rl = this.Roles.FirstOrDefault(w => w.Name == role[0].Trim());

                // role possibly does not exists
                return rl == null ? null : new Group(org, rl);
            }

            if (role.Length == 2)
            {
                var org = this.Organisations.FirstOrDefault(w => w.Name == role[0].Trim());
                var rl = this.Roles.FirstOrDefault(w => w.Name == role[1].Trim());
                return (rl == null || org == null) ? null : new Group(org, rl);
            }

            return null;
        }

        public Group[] RolesByName(string[][] roleNames)
        {
            var roleList = new Group[roleNames.Length];
            for (int i=0; i<roleNames.Length; i++)
            {
                // in case there is only one organisation we allow users to specify only a role with no organisation
                var orgRole = this.GroupByName(roleNames[i]);
                if (orgRole == null) return null;
                roleList[i] = orgRole;
            }
            return roleList;
        } 

        // internal methods

        internal int CreateId()
        {
            return this.idProvider++;
        }

        public void NotifyParameterChanged(string ownerString, object paramValue)
        {
            // we do not notify about institutional changes for now
        }
    }
}
