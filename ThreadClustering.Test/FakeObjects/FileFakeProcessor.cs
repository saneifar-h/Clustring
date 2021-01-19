using System;
using System.IO;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class FileFakeProcessor : IItemProcessor
    {
        private readonly int clusterIndex;

        public FileFakeProcessor(int clusterIndex)
        {
            this.clusterIndex = clusterIndex;
        }

        public void Process(IHaveClusterKeyItem item)
        {
            File.AppendAllText($"test\\AsyncClusterTest\\test{clusterIndex}.dat",
                $"item Processed at {DateTime.Now.TimeOfDay} Key:{item.Key}" +
                Environment.NewLine);
        }
    }
}