using System;
using System.Collections.Generic;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class ListFakeProcessor : IItemProcessor
    {
        private readonly int clusterIndex;
        private readonly List<string> lstResult;

        public ListFakeProcessor(int clusterIndex, List<string> lstResult)
        {
            this.clusterIndex = clusterIndex;
            this.lstResult = lstResult;
        }

        public void Process(IHaveClusterKeyItem item)
        {
            lstResult.Add($"item Processed at {DateTime.Now.TimeOfDay} Key:{item.Key}");
        }
    }
}