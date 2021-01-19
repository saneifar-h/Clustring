using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    public class AsyncClusterManager
    {
        private readonly ClusterProvider clusterProvider;
        private readonly ClusteringOption option;

        public AsyncClusterManager(ClusteringOption option, IClusterItemExecuteLogger logger)
        {
            this.option = option;
            clusterProvider = new ClusterProvider(option, WorkingMode.Async, logger);
        }

        public void Cluster(IHaveClusterKeyItem item)
        {
            clusterProvider.Enqueue(item);
        }

        public void Pause()
        {
            clusterProvider.Pause();
        }

        public void Resume()
        {
            clusterProvider.Resume();
        }
    }
}