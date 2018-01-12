using System.Collections.Generic;
using System.Linq;

namespace Ei.Persistence
{
    public class ParametricEntityDao : EntityDao
    {
        public List<ParameterDao> Properties { get; set; }

        public string Parent { get; set; }

        protected ParametricEntityDao() {
            this.Properties = new List<ParameterDao>();
        }

        public List<ParameterDao> PublicProperties => this.Properties.Where(p => p.AccessType == VariableAccess.Public).ToList();
    }
}
