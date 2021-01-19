using System;
using System.Collections.Concurrent;
using System.Threading;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class FakeLogger : IClusterItemExecuteLogger
    {
        private readonly AutoResetEvent autoResetEvent;
        private readonly int itemNumber;
        private readonly object syncObj = new object();
        private int counter;
        public ConcurrentBag<EnqueueDequeResult> LstDequeue = new ConcurrentBag<EnqueueDequeResult>();
        public ConcurrentBag<EnqueueDequeResult> LstEnqueue = new ConcurrentBag<EnqueueDequeResult>();

        public FakeLogger(int itemNumber, AutoResetEvent autoResetEvent)
        {
            this.itemNumber = itemNumber;
            this.autoResetEvent = autoResetEvent;
        }

        public void LogEnqueue(ClusterInfo info, IHaveClusterKeyItem item)
        {
            lock (syncObj)
            {
                LstEnqueue.Add(new EnqueueDequeResult(info, item, null, DateTime.Now));
            }
        }

        public void LogExecute(ClusterInfo info, IHaveClusterKeyItem item, ExecutionInfo executionInfo)
        {
            lock (syncObj)
            {
                counter++;
                LstDequeue.Add(new EnqueueDequeResult(info, item, executionInfo, DateTime.Now));
                if (counter == itemNumber)
                    autoResetEvent.Set();
            }
        }
    }
}