using Ei.Persistence.Templates;

namespace Ei.Persistence
{
    

    public class RoleDao : SearchableEntityDao
    {
        public override string ClassName => this.Name.ToId() + "Role";

        public string GenerateCode() {
             return CodeGenerator.Role(this);
        }
    }
}
