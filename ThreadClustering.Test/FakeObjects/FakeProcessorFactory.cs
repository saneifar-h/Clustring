using System.Collections.Generic;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class FakeProcessorFactory : IItemProcessorFactory
    {
        private readonly int mode;
        public List<List<string>> LstResultList = new List<List<string>>();

        public FakeProcessorFactory(int mode)
        {
            this.mode = mode;
        }

        public IItemProcessor Create(int clusterIndex)
        {
            if (mode == 0)
                return new FileFakeProcessor(clusterIndex);
            var lst = new List<string>();
            LstResultList.Insert(clusterIndex, lst);
            return new ListFakeProcessor(clusterIndex, lst);
        }
    }
}