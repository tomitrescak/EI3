namespace Ei.Runtime
{

    public class RuntimeManager : IRuntimeManager
    {
        public virtual Governor CreateGovernor()
        {
            return new Governor();
        }
    }
}
