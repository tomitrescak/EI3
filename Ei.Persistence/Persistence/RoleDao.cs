using Ei.Persistence.Templates;

namespace Ei.Persistence
{
    

    public class RoleDao : SearchableEntityDao
    {
        public override string ClassName => this.Name.ToId() + "Role";

        public override string ParentClassName => 
            this.Parent != null && InstitutionDao.Instance != null 
                ? InstitutionDao.Instance.Roles.Find(o => o.Id == this.Parent).ClassName
                : "Role";
        
        public string GenerateCode() {
             return CodeGenerator.Role(this);
        }
    }
}
