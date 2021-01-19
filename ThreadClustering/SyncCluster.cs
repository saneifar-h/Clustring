using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    internal class SyncCluster : ICluster
    {
        private readonly BlockingCollection<IHaveClusterKeyItem> eventReadyQueue;

        private readonly IItemProcessor itemProcessor;
        private readonly IClusterItemExecuteLogger logger;
        private readonly AutoResetEvent pauseAutoResetEvent = new AutoResetEvent(false);
        private readonly object syncObject = new object();
        private bool paused;
        private Task task;

        public SyncCluster(int index, string title, IItemProcessor itemProcessor, IClusterItemExecuteLogger logger,
            int maxItemInQueue)
        {
            eventReadyQueue =
                new BlockingCollection<IHaveClusterKeyItem>(maxItemInQueue);
            this.itemProcessor = itemProcessor;
            this.logger = logger;
            Index = index;
            Title = title;
            Start();
        }

        public int Index { get; }
        public string Title { get; }
        public int Count => eventReadyQueue.Count;
        public int TotalItemsEnqueueCount { get; private set; }
        public WorkingMode WorkingMode => WorkingMode.Sync;
        public int TotalItemsDequeueCount { get; private set; }
        public int Id => GetHashCode();

        public void Enqueue(IHaveClusterKeyItem item)
        {
            var syncItem = (SyncItemWrapper) item;
            eventReadyQueue.Add(item);
            TotalItemsEnqueueCount++;
            logger.LogEnqueue(CreateInfo(), syncItem.Item);
        }

        public void Pause()
        {
            lock (syncObject)
            {
                paused = true;
            }
        }

        public void Resume()
        {
            lock (syncObject)
            {
                if (paused) pauseAutoResetEvent.Set();
            }
        }

        public void Start()
        {
            if (task == null)
                task = Task.Factory.StartNew(Consume, TaskCreationOptions.LongRunning);
        }

        private ClusterInfo CreateInfo()
        {
            return new ClusterInfo(Index, Title, Count, TotalItemsEnqueueCount, TotalItemsDequeueCount, WorkingMode,
                Id);
        }

        private void Consume()
        {
            while (true)
            {
                var queItem = eventReadyQueue.Take();
                var syncItem = (SyncItemWrapper) queItem;
                if (paused)
                {
                    pauseAutoResetEvent.WaitOne();
                    paused = false;
                }

                try
                {
                    itemProcessor.Process(syncItem.Item);
                    syncItem.SetExecutionInfo(new ExecutionInfo(true, null));
                    logger.LogExecute(CreateInfo(), syncItem.Item,
                        new ExecutionInfo(true, null));
                }
                catch (Exception exception)
                {
                    syncItem.SetExecutionInfo(new ExecutionInfo(true, exception));
                    logger.LogExecute(CreateInfo(), syncItem.Item,
                        new ExecutionInfo(false, exception));
                }
                finally
                {
                    TotalItemsDequeueCount++;
                    syncItem.AutoResetEvent.Set();
                }
            }
        }
    }
}