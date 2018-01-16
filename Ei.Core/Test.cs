using Ei;
using Ei.Ontology;
using Ei.Runtime;
using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;

#region class DefaultInstitution
public class DefaultInstitution : Institution<DefaultInstitution.Store>
{

    #region class Store
    public class Store : Institution.InstitutionState
    {
        int Count { get; set; }

        public Store(Institution ei) : base(ei) {}
    }
    #endregion

    // constructor

    public DefaultInstitution() : base("default") {
        this.Name = "Default";
        this.Description = "Description";

        // init organisations
        this.AddOrganisations(
        );

        // init components
        this.AddRoles(
        );

        // init workflows
        this.AddWorkflows(
        );
        this.MainWorkflowId = this.Workflows[0].Id;

        // init security
        
        this.AuthenticationPermissions.Add(
            new AuthorisationInfo("User", "Password", "Organisation", 
                this.GroupByName("OId", "RId"),
                this.GroupByName("", "RId"), 
                this.GroupByName("OId", ""))
        );
    }

    // abstract implementation

    public override Institution Instance => new DefaultInstitution();

    public override Institution.InstitutionState Resources => new DefaultInstitution.Store(this);
}
#endregion