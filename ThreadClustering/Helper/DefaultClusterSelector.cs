using System.Collections.Generic;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Helper
{
    public class DefaultClusterSelector : IClusterIndexSelector
    {
        private readonly Dictionary<object, ClusterInfo> clusterAssignmentDic;
        private readonly object syncObject = new object();

        public DefaultClusterSelector()
        {
            clusterAssignmentDic =
                new Dictionary<object, ClusterInfo>();
        }

        public int SelectIndex(IHaveClusterKeyItem item, IReadOnlyList<ClusterInfo> clusters)
        {
            lock (syncObject)
            {
                if (clusterAssignmentDic.ContainsKey(item.Key))
                    return clusterAssignmentDic[item.Key].Index;
                var index = 0;
                var minCount = clusters[0].ItemInQueueCount + clusters[0].TotalItemsDequeueCount;
                for (var i = 1; i < clusters.Count; i++)
                    if (clusters[i].ItemInQueueCount + clusters[i].TotalItemsDequeueCount < minCount)
                        index = i;
                var cluster = clusters[index];
                clusterAssignmentDic.Add(item.Key, cluster);
                return clusterAssignmentDic[item.Key].Index;
            }
        }
    }
}