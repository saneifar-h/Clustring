using System;
using System.Threading;
using ThreadClustering.Interfaces;

namespace ThreadClustering
{
    internal class SyncItemWrapper : IHaveClusterKeyItem
    {
        public SyncItemWrapper(IHaveClusterKeyItem item)
        {
            Item = item;
            AutoResetEvent = new AutoResetEvent(false);
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public IHaveClusterKeyItem Item { get; }
        public AutoResetEvent AutoResetEvent { get; }
        public ExecutionInfo ExecutionInfo { get; private set; }
        public object Key => Item.Key;

        public void SetExecutionInfo(ExecutionInfo executionInfo)
        {
            ExecutionInfo = executionInfo;
        }
    }
}