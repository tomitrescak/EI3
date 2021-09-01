using Ei.Persistence.Templates;
using HandlebarsDotNet;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ei.Persistence
{
    public class SearchableEntityDao : ParametricEntityDao
    {
        
    }

    public class OrganisationDao : SearchableEntityDao
    {
        public override string ClassName => this.Name.ToId() + "Organisation";

        public override string ParentClassName => 
            this.Parent != null && InstitutionDao.Instance != null 
            ? InstitutionDao.Instance.Organisations.Find(o => o.Id == this.Parent).ClassName
            : "Organisation";

        public override string GenerateCode() {
            return CodeGenerator.Organisation(this);
        }
    }
}
