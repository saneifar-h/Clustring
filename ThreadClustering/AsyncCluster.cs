using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ThreadClustering.Interfaces;
using ThreadClustering.Options;

namespace ThreadClustering
{
    internal class AsyncCluster : ICluster
    {
        private readonly BlockingCollection<IHaveClusterKeyItem> eventReadyQueue;

        private readonly IItemProcessor itemProcessor;
        private readonly IClusterItemExecuteLogger logger;
        private readonly AutoResetEvent pauseAutoResetEvent = new AutoResetEvent(false);

        private readonly object syncObject = new object();
        private bool paused;
        private Task task;

        public AsyncCluster(int index, string title, IItemProcessor itemProcessor, IClusterItemExecuteLogger logger,
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

        public WorkingMode WorkingMode => WorkingMode.Async;
        public int TotalItemsDequeueCount { get; private set; }
        public int Id => GetHashCode();

        public void Enqueue(IHaveClusterKeyItem item)
        {
            TotalItemsEnqueueCount++;
            eventReadyQueue.Add(item);
            logger.LogEnqueue(
                CreateInfo(),
                item);
        }

        public int TotalItemsEnqueueCount { get; private set; }

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
                if (paused)
                {
                    pauseAutoResetEvent.WaitOne();
                    paused = false;
                }

                try
                {
                    itemProcessor.Process(queItem);
                    logger.LogExecute(CreateInfo(), queItem,
                        new ExecutionInfo(true, null));
                }
                catch (Exception exception)
                {
                    logger.LogExecute(CreateInfo(), queItem,
                        new ExecutionInfo(false, exception));
                }
                finally
                {
                    TotalItemsDequeueCount++;
                }
            }
        }
    }
}