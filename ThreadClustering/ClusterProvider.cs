using System;
using System.Collections.Generic;
using System.Linq;
using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    internal class ClusterProvider
    {
        private readonly List<ICluster> clusters;
        private readonly ClusteringOption option;


        public ClusterProvider(ClusteringOption option, WorkingMode workingMode, IClusterItemExecuteLogger logger)
        {
            this.option = option;

            clusters = new List<ICluster>();
            switch (workingMode)
            {
                case WorkingMode.Async:
                    for (var i = 0; i < option.MaxConcurrentCluster; i++)
                    {
                        var createOption = option.ClusterCreationOptionFactory.Create(i);
                        var cluster = new AsyncCluster(i, createOption.Title,
                            createOption.ItemProcessorFactory.Create(i),
                            logger, option.MaxItemInQueue);
                        clusters.Add(cluster);
                    }

                    break;
                case WorkingMode.Sync:
                    for (var i = 0; i < option.MaxConcurrentCluster; i++)
                    {
                        var createOption = option.ClusterCreationOptionFactory.Create(i);
                        var cluster = new SyncCluster(i, createOption.Title,
                            createOption.ItemProcessorFactory.Create(i),
                            logger, option.MaxItemInQueue);
                        clusters.Add(cluster);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Enqueue(IHaveClusterKeyItem item)
        {
            var selectedIndex =
                option.ClusterIndexSelector.SelectIndex(item, clusters.Select(i => new ClusterInfo(i)).ToList());
            clusters[selectedIndex].Enqueue(item);
        }

        public void Pause()
        {
            foreach (var cluster in clusters) cluster.Pause();
        }

        public void Resume()
        {
            foreach (var cluster in clusters) cluster.Resume();
        }
    }
}