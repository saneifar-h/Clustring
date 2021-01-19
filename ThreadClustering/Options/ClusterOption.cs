using ThreadClustering.Interfaces;

namespace ThreadClustering.Options
{
    public class ClusterOption
    {
        public ClusterOption(string title, IItemProcessorFactory itemProcessorFactory)
        {
            Title = title;
            ItemProcessorFactory = itemProcessorFactory;
        }

        public string Title { get; }
        public IItemProcessorFactory ItemProcessorFactory { get; }
    }
}