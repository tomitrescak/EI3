namespace Ei.Persistence
{
    public interface IDaoFactory
    {
        InstitutionDao Load(string source, IWorkflowImportLoader loader);

        bool Save(InstitutionDao dao, string target);
    }
}
