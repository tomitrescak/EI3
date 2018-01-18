using Ei.Persistence.Templates;

namespace Ei.Persistence.Transitions
{
    public class TransitionSplitDao : TransitionDao
    {
        public bool Shallow { get; set; }
        public string[][] Names { get; set; }

        public override string GenerateCode() {
            //return result;
            return CodeGenerator.TransitionSplit(this);
        }
    }
}
