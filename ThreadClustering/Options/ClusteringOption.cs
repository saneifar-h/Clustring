using ThreadClustering.Interfaces;

namespace ThreadClustering.Options
{
    public class ClusteringOption
    {
        public ClusteringOption(long maxConcurrentCluster, IClusterOptionFactory clusterCreationOptionFactory,
            IClusterIndexSelector clusterIndexSelector, int syncWaitTimeOutInSecond = 30,
            int maxConcurrentItemInQueue = 1000000)
        {
            MaxConcurrentCluster = maxConcurrentCluster;
            ClusterCreationOptionFactory = clusterCreationOptionFactory;
            ClusterIndexSelector = clusterIndexSelector;
            MaxItemInQueue = maxConcurrentItemInQueue;
            SyncWaitTimeOutInSecond = syncWaitTimeOutInSecond;
        }

        public int MaxItemInQueue { get; }
        public IClusterOptionFactory ClusterCreationOptionFactory { get; }
        public IClusterIndexSelector ClusterIndexSelector { get; }
        public long MaxConcurrentCluster { get; }
        public int SyncWaitTimeOutInSecond { get; }
    }
}