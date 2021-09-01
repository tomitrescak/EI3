using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Ei.Persistence.Actions;
using Ei.Persistence.Templates;
using Newtonsoft.Json;

namespace Ei.Persistence
{
    public class InstitutionDao: ParametricEntityDao
    {
        public static InstitutionDao Instance;

        [JsonIgnore]
        public override string ClassName => "DefaultInstitution";
        
        [JsonIgnore]
        public OrganisationDao DefaultOrganisation { get; private set; }

        [JsonIgnore]
        public WorkflowDao CurrentWorkflow { get; set; }

        [JsonIgnore]
        public string CurrentWorkflowClass => this.CurrentWorkflow.ClassName;

        public List<ClassDao> Types { get; set; }
         
        public List<RoleDao> Roles { get; set;  }
        
        public List<OrganisationDao> Organisations { get; set; }

        public string Expressions { get; set; } 

        public List<WorkflowDao> Workflows { get; set; }

        public List<AuthorisationDao> Authorisation { get; set; }

        public GlobalsDao Globals { get; set; }

        public string InitialWorkflow { get; set; }

        public string CurrentAction { get; internal set; }

        // HELPERS

        public OrganisationDao OrganisationById(string id) {
            return this.Organisations.Find(o => o.Id == id);
        }

        public RoleDao RoleById(string id) {
            return this.Roles.Find(o => o.Id == id);
        }

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

            // temporary solution

            var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Generated.cs");
            Console.WriteLine($"\nSaving to: {path}");

            File.WriteAllText(path, @"
using Ei;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Ontology.Transitions;
using Ei.Runtime;
using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;

" + sb.ToString());
            
            return sb.ToString();
        }

        public override string GenerateCode() {
            return CodeGenerator.Institution(this);
        }
    }
}
