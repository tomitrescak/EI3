using Ei.Core.Ontology;

namespace Ei.Core.Runtime.Planning.Storage
{
    public interface IStorage
    {
        AStarNode FindOpened(Connection arc);
        AStarNode FindClosed(Connection node);
        bool HasOpened();
        void RemoveOpened(AStarNode node);
        void RemoveClosed(AStarNode node);
        bool IsOpen(AStarNode node);
        bool IsClosed(AStarNode node);
        void AddToOpenList(AStarNode node);
        void AddToClosedList(AStarNode node);
        AStarNode RemoveCheapestOpenNode();
    }
}
