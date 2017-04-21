using Ei.Runtime;

namespace Ei.Ontology
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
