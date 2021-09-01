using System.Collections.Generic;
using System.Linq;

namespace Ei.Persistence
{
    public class ParametricEntityDao : EntityDao
    {
        public string Interface { get; set; }
        
        public List<ParameterDao> Properties { get; set; }

        public string[] Validations { get; set; }

        public string Parent { get; set; }

        protected ParametricEntityDao() {
            this.Properties = new List<ParameterDao>();
        }

        public string ImplementsInterface => string.IsNullOrEmpty(Interface) ? string.Empty : (", " + Interface);

        public List<ParameterDao> PublicProperties => this.Properties.Where(p => p.AccessType == VariableAccess.Public).ToList();
    }
}
