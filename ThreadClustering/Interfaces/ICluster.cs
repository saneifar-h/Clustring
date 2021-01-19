using ThreadClustering.Options;

namespace ThreadClustering.Interfaces
{
    internal interface ICluster
    {
        int Index { get; }
        string Title { get; }
        int Count { get; }
        int TotalItemsEnqueueCount { get; }
        WorkingMode WorkingMode { get; }
        int TotalItemsDequeueCount { get; }
        int Id { get; }
        void Enqueue(IHaveClusterKeyItem item);
        void Pause();
        void Resume();
        void Start();
    }
}