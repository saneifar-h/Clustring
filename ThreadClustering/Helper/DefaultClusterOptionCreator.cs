using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering.Helper
{
    public class DefaultClusterOptionCreator : IClusterOptionFactory
    {
        private readonly IItemProcessorFactory processorFactory;

        public DefaultClusterOptionCreator(IItemProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public ClusterOption Create(int clusterIndex)
        {
            return new ClusterOption($"Cluster{clusterIndex}", processorFactory);
        }
    }
}