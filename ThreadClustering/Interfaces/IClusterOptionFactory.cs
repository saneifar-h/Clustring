using ThreadClustering.Options;

namespace ThreadClustering.Interfaces
{
    public interface IClusterOptionFactory
    {
        ClusterOption Create(int clusterIndex);
    }
}