using System;
using System.Collections.Generic;
using System.Text;
using Ei.Persistence.Actions;
using Ei.Persistence.Templates;
using Newtonsoft.Json;

namespace Ei.Persistence
{
    public class InstitutionDao: ParametricEntityDao
    {
        public static InstitutionDao Instance;

        public override string ClassName => base.ClassName + "Institution";

        public List<ClassDao> Types { get; set; }
         
        public List<RoleDao> Roles { get; set;  }
        
        public List<OrganisationDao> Organisations { get; set; }

        public List<string> Expressions { get; set; } 

        public List<WorkflowDao> Workflows { get; set; }

        public List<AuthorisationDao> Authorisation { get; set; }

        public GlobalsDao Globals { get; set; }

        public string InitialWorkflow { get; set; }

        // HELPERS

        [JsonIgnore]
        public OrganisationDao DefaultOrganisation { get; private set; }

        public InstitutionDao()
        {
            Instance = this;
      
            this.Types = new List<ClassDao>();
            this.Roles = new List<RoleDao>();
            this.Organisations = new List<OrganisationDao>();
            this.Workflows = new List<WorkflowDao>();
        }

        public string GenerateAll() {
            var sb = new StringBuilder();

            sb.AppendLine(CodeGenerator.Institution(this));

            this.Roles.ForEach(o => sb.AppendLine(o.GenerateCode()));
            this.Organisations.ForEach(o => sb.AppendLine(o.GenerateCode()));
            this.Workflows.ForEach(o => sb.AppendLine(o.GenerateCode()));

            return sb.ToString();
        }

        public override string GenerateCode() {
            return CodeGenerator.Institution(this);
        }
    }
}
