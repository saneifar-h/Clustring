using System;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class EnqueueDequeResult
    {
        public EnqueueDequeResult(ClusterInfo clusterInfo, IHaveClusterKeyItem item, ExecutionInfo executionInfo,
            DateTime time)
        {
            ClusterInfo = clusterInfo;
            Item = item;
            ExecutionInfo = executionInfo;
            Time = time;
        }

        public ClusterInfo ClusterInfo { get; }

        public IHaveClusterKeyItem Item { get; }

        public ExecutionInfo ExecutionInfo { get; }

        public DateTime Time { get; }
    }
}