using System;
using ThreadClustering.Interfaces;

namespace ThreadClustering.Test.FakeObjects
{
    public class HaveClusterKeyItem : IHaveClusterKeyItem
    {
        public HaveClusterKeyItem(int key, int id, DateTime creationDateTime)
        {
            Id = id;
            CreationDateTime = creationDateTime;
            Key = key;
        }

        public int Id { get; }
        public DateTime CreationDateTime { get; }

        public int Key { get; }

        object IHaveClusterKeyItem.Key => Key;
    }
}