using System;
using System.Collections.Generic;
using Ei.Persistence.Actions;

namespace Ei.Persistence
{
    public class InstitutionDao: ParametricEntityDao
    {
        public List<ClassDao> Types { get; set; }
         
        public List<RoleDao> Roles { get; set;  }

        public List<RelationDao> RoleRelations { get; set; }
        
        public List<OrganisationDao> Organisations { get; set; }

        public List<RelationDao> OrganisationRelations { get; set; }

        public List<string> Expressions { get; set; } 

//        public List<ActionDao> Actions { get; set; }

        public List<WorkflowDao> Workflows { get; set; }

        public List<AuthorisationDao> Authorisation { get; set; }

        public GlobalsDao Globals { get; set; }

        //        public List<ActivityDao> Activities { get; }
        //        
        //        public List<ConnectionDao> Connections { get; }

        public string InitialWorkflow { get; set; }

        public InstitutionDao()
        {
            this.Types = new List<ClassDao>();
            this.Roles = new List<RoleDao>();
            this.RoleRelations = new List<RelationDao>();
            this.Organisations = new List<OrganisationDao>();
            this.OrganisationRelations = new List<RelationDao>();
//            this.Actions = new List<ActionDao>();
            this.Workflows = new List<WorkflowDao>();
        }
    }
}
