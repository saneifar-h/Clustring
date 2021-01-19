namespace ThreadClustering.Interfaces
{
    public interface IItemProcessorFactory
    {
        IItemProcessor Create(int clusterIndex);
    }
}