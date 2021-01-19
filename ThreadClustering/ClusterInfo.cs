using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    public class ClusterInfo
    {
        internal ClusterInfo(int index, string title, int itemInQueueCount, int totalItemsEnqueueCount,
            int totalItemsDequeueCount, WorkingMode workingMode, int id)
        {
            Index = index;
            Title = title;
            ItemInQueueCount = itemInQueueCount;
            TotalItemsEnqueueCount = totalItemsEnqueueCount;
            TotalItemsDequeueCount = totalItemsDequeueCount;
            WorkingMode = workingMode;
            Id = id;
        }

        internal ClusterInfo(ICluster cluster) : this(cluster.Index,
            cluster.Title, cluster.Count, cluster.TotalItemsEnqueueCount, cluster.TotalItemsDequeueCount,
            cluster.WorkingMode, cluster.Id)
        {
        }

        public int Index { get; }
        public string Title { get; }
        public int ItemInQueueCount { get; }
        public int TotalItemsEnqueueCount { get; }
        public WorkingMode WorkingMode { get; }
        public int TotalItemsDequeueCount { get; }
        public int Id { get; }
    }
}