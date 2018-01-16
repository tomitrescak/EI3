using Ei.Persistence.Templates;
using System;

namespace Ei.Persistence
{
    public class ClassDao : ParametricEntityDao
    {
       public string GenerateCode() {
            return CodeGenerator.Class(this);
        }
    }
}
