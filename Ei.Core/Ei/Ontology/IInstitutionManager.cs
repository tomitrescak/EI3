using Ei.Core.Runtime;

namespace Ei.Core.Ontology
{

    public interface IInstitutionManager
    {
        Institution Ei { get; set; }
        bool Start(Institution ei);
        InstitutionCodes Connect(
            IGovernorCallbacks callback, 
            string organisation, 
            string username, 
            string password,
            string[][] roles,
            out Governor gov);
    }
}
