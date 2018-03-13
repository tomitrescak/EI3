using System;
using Ei.Persistence.Templates;

namespace Ei.Persistence.Transitions
{
    public class TransitionSplitDao : TransitionDao
    {
        public bool Shallow { get; set; }
        public string[][] Names { get; set; }

        public override string GenerateCode() {
            if (this.Names.Length == 0) {
                throw new Exception($"Split transition '{this.Id}' needs to define all split names!");
            }
            //return result;
            return CodeGenerator.TransitionSplit(this);
        }
    }
}
