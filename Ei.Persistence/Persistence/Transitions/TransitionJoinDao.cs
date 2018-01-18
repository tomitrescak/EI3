using Ei.Persistence.Templates;

namespace Ei.Persistence.Transitions
{
    public class TransitionJoinDao : TransitionDao
    {
        public override string GenerateCode() {
            //return result;
            return CodeGenerator.TransitionJoin(this);
        }
    }
}
