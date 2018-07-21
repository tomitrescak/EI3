namespace Ei.Persistence
{
    using Ei.Ontology;

    public interface IDataProvider
    {
        Institution Load(string name, string connectionString = null);

        bool Save();
    }
}
