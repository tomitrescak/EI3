using System.Collections.Generic;

namespace Ei.Persistence
{
    public class ParametricEntityDao : EntityDao
    {
        public List<ParameterDao> Properties { get; set; }

        protected ParametricEntityDao()
        {
            this.Properties = new List<ParameterDao>();
        }
        
    }
}
