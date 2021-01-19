using System.Collections.Generic;

namespace ThreadClustering.Interfaces
{
    public interface IClusterIndexSelector
    {
        int SelectIndex(IHaveClusterKeyItem item, IReadOnlyList<ClusterInfo> cluster);
    }
}