using System;

namespace ThreadClustering.Interfaces
{
    public interface IClusterItemExecuteLogger
    {
        void LogEnqueue(ClusterInfo info, IHaveClusterKeyItem item);
        void LogExecute(ClusterInfo info, IHaveClusterKeyItem item, ExecutionInfo executionInfo);
    }

    public class ExecutionInfo
    {
        public ExecutionInfo(bool executedSuccessfully, Exception exception)
        {
            ExecutedSuccessfully = executedSuccessfully;
            Exception = exception;
        }

        public bool ExecutedSuccessfully { get; }
        public Exception Exception { get; }
    }
}