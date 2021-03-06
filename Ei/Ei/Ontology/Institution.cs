﻿namespace Ei.Ontology
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

    public abstract class Institution : Entity, IStateProvider
    {
        public class InstitutionState : VariableState
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

        #endregion

        #region Properties

        public ReadOnlyCollection<Role> Roles { get; protected set; }

        public ReadOnlyCollection<Organisation> Organisations { get; protected set; }

        public ReadOnlyCollection<Connection> GlobalConnections { get; protected set; }

        public ReadOnlyCollection<ActionBase> GlobalActions { get; protected set; }

        public Dictionary<string, string> GlobalFunctions { get; protected set; }

        public ReadOnlyCollection<Workflow> Workflows { get; protected set; }

        public string MainWorkflowId { get; protected set; }

        public Security AuthenticationPermissions { get; protected set; }

        public abstract Institution Instance { get; }

        // time and properties

        public Stopwatch Time { get; protected set; }

        public ManualResetEvent Pauser { get; protected set; }

        #endregion

        // constructor

        protected Institution(string id) : base(id) {
            Log.Info("Institution instantiated ...");

            this.AuthenticationPermissions = new Security();
        }

        // public methods

        public abstract void Start();

        public Workflow GetWorkflow(string id) {
            return this.Workflows.First(w => w.Id == id);
        }

        public Workflow.Instance CreateWorkflow(string id, Workflow.Instance parentWorkflow) {
            var workflow = this.Workflows.First(w => w.Id == id);
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

        public Group GroupByName(string[] role) {
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
                var org = this.Organisations.FirstOrDefault(w => w.Name == role[0].Trim());
                if (org == null) {
                    throw new InstitutionException("Organisation does not exist: " + role[0]);
                }
                var rl = this.Roles.FirstOrDefault(w => w.Name == role[1].Trim());
                if (rl == null) {
                    throw new InstitutionException("Role does not exist: " + role[0]);
                }
                return (rl == null || org == null) ? null : new Group(org, rl);
            }

            return null;
        }

        public Group[] RolesByName(string[][] roleNames) {
            var roleList = new Group[roleNames.Length];
            for (int i = 0; i < roleNames.Length; i++) {
                // in case there is only one organisation we allow users to specify only a role with no organisation
                var orgRole = this.GroupByName(roleNames[i]);
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

        // properties

        public ReadOnlyCollection<Action<T>> Expressions { get; protected set; }

        public abstract T VariableState { get; }

        // constructor

        protected Institution(string id) : base(id) {
            this.Start();
        }

        // methods 

        public override void Start() {
            // init pauser that allows us to pause the institution
            this.Pauser = new ManualResetEvent(true);

            // start timer

            this.Time = new Stopwatch();
            this.Time.Start();

            // init expression evaluation
            if (this.Expressions.Count > 0) {
                this.expressionTimer = new Timer {
                    Interval = 100,
                    AutoReset = true
                };

                this.expressionTimer.Elapsed += (sender, args) => {
                    foreach (var expression in this.Expressions) {
                        expression(this.VariableState);
                    }
                };

                this.expressionTimer.Start();
            }
        }
    } 
}
