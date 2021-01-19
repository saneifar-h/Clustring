namespace ThreadClustering.Interfaces
{
    public interface IItemProcessor
    {
        void Process(IHaveClusterKeyItem item);
    }
}