using System;
using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    public class SyncClusterManager
    {
        private readonly ClusterProvider clusterProvider;
        private readonly ClusteringOption option;

        public SyncClusterManager(ClusteringOption option, IClusterItemExecuteLogger logger)
        {
            this.option = option;
            clusterProvider = new ClusterProvider(option, WorkingMode.Sync, logger);
        }

        public ExecutionInfo Cluster(IHaveClusterKeyItem item)
        {
            var warpItem = new SyncItemWrapper(item);
            clusterProvider.Enqueue(warpItem);
            return !warpItem.AutoResetEvent.WaitOne(TimeSpan.FromSeconds(option.SyncWaitTimeOutInSecond))
                ? new ExecutionInfo(false, new TimeoutException("پایان مهلت زمانی برای دریافت پاسخ  پردازش"))
                : warpItem.ExecutionInfo;
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