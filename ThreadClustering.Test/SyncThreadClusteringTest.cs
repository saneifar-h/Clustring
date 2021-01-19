using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadClustering.Helper;
using ThreadClustering.Options;
using ThreadClustering.Test.FakeObjects;

namespace ThreadClustering.Test
{
    [TestClass]
    public class SyncThreadClusteringTest
    {
        private FakeProcessorFactory fakeProcessorFactory;
        private ClusteringOption option;
        private SyncClusterManager syncClusterManager;

        private void Setup(FakeLogger logger, int concurrentClusterCount = 3, int mode = 0, int waitTimeout = 30)
        {
            if (!Directory.Exists("test\\AsyncClusterTest\\"))
                Directory.CreateDirectory("test\\AsyncClusterTest\\");
            for (var i = 0; i < 10; i++)
                if (File.Exists($"test\\AsyncClusterTest\\test{i}.dat"))
                    File.Delete($"test\\AsyncClusterTest\\test{i}.dat");
            fakeProcessorFactory = new FakeProcessorFactory(mode);
            var clusterOptionCreator = new DefaultClusterOptionCreator(fakeProcessorFactory);
            var clusterSelector = new DefaultClusterSelector();
            option = new ClusteringOption(concurrentClusterCount, clusterOptionCreator, clusterSelector, waitTimeout);
            syncClusterManager = new SyncClusterManager(option, logger);
        }

        private HaveClusterKeyItem CreateItem(int key, int id)
        {
            return new HaveClusterKeyItem(key, id, DateTime.Now);
        }

        [TestMethod]
        public void Test_Async_Cluster_Simple_Allocation_JustFor_SameKey()
        {
            var lstDuration = new List<double>();
            var itemNumber = 3;

            var autoResetEvent = new AutoResetEvent(false);
            var logger = new FakeLogger(itemNumber, autoResetEvent);
            Setup(logger);
            var item1 = CreateItem(1, 10);
            var item2 = CreateItem(1, 20);
            var item3 = CreateItem(1, 30);
            var st = new Stopwatch();
            st.Start();
            Task.Run(() => syncClusterManager.Cluster(item1));
            Task.Run(() => syncClusterManager.Cluster(item2));
            Task.Run(() => syncClusterManager.Cluster(item3));
            autoResetEvent.WaitOne();
            st.Stop();
            var duration = st.ElapsedMilliseconds;

            Console.WriteLine($"All Action Done Time = {duration}");
            var deqOrderedList = logger.LstDequeue.OrderBy(i => i.Item.Key).ToList();
            var enqOrderedList = logger.LstEnqueue.OrderBy(i => i.Item.Key).ToList();

            for (var i = 0; i < itemNumber; i++)
                lstDuration.Add((deqOrderedList[i].Time -
                                 enqOrderedList[i].Time).TotalMilliseconds);
            for (var i = 0; i < itemNumber; i++)
                Assert.AreEqual(deqOrderedList[i].ClusterInfo.Id, enqOrderedList[i].ClusterInfo.Id);
            Console.WriteLine($"Avg Process {lstDuration.Average()}");
        }

        [TestMethod]
        public void Test_Async_Cluster_Simple_Allocation_JustFor_DifferentKey()
        {
            var lstDuration = new List<double>();
            var itemNumber = 3;

            var autoResetEvent = new AutoResetEvent(false);
            var logger = new FakeLogger(3, autoResetEvent);
            Setup(logger);
            var item1 = CreateItem(1, 10);
            var item2 = CreateItem(2, 20);
            var item3 = CreateItem(3, 30);
            var st = new Stopwatch();
            st.Start();
            Task.Run(() => syncClusterManager.Cluster(item1));
            Task.Run(() => syncClusterManager.Cluster(item2));
            Task.Run(() => syncClusterManager.Cluster(item3));
            autoResetEvent.WaitOne();
            st.Stop();
            var duration = st.ElapsedMilliseconds;
            Console.WriteLine($"All Action Done Time = {duration}");
            var deqOrderedList = logger.LstDequeue.OrderBy(i => i.Item.Key).ToList();
            var enqOrderedList = logger.LstEnqueue.OrderBy(i => i.Item.Key).ToList();

            for (var i = 0; i < itemNumber; i++)
                lstDuration.Add((deqOrderedList[i].Time -
                                 enqOrderedList[i].Time).TotalMilliseconds);
            for (var i = 0; i < itemNumber; i++)
                Assert.AreEqual(deqOrderedList[i].ClusterInfo.Id, enqOrderedList[i].ClusterInfo.Id);
            Console.WriteLine($"Avg Process {lstDuration.Average()}");
        }

        [TestMethod]
        public void Test_Async_Cluster_Complex_Allocation_JustFor_DifferentKey()
        {
            var autoResetEvent = new AutoResetEvent(false);
            const int itemNumber = 100000;
            const int concurrentItems = 4;
            var logger = new FakeLogger(itemNumber, autoResetEvent);
            var lstDuration = new List<double>();
            Setup(logger, concurrentItems, 1);
            var st = new Stopwatch();
            st.Start();
            var tasks = new List<Task>();
            for (var i = 0; i < itemNumber; i++)
            {
                var haveClusterKeyItem = CreateItem(i, i);
                tasks.Add(new Task(() => { syncClusterManager.Cluster(haveClusterKeyItem); }));
            }

            foreach (var task in tasks) task.Start();
            autoResetEvent.WaitOne();
            st.Stop();
            var duration = st.ElapsedMilliseconds;
            Console.WriteLine($"All Action Done Time = {duration}");

            var deqOrderedList = logger.LstDequeue.OrderBy(i => i.Item.Key).ToList();
            var enqOrderedList = logger.LstEnqueue.OrderBy(i => i.Item.Key).ToList();

            for (var i = 0; i < itemNumber; i++)
                lstDuration.Add((deqOrderedList[i].Time -
                                 enqOrderedList[i].Time).TotalMilliseconds);
            for (var i = 0; i < itemNumber; i++)
                Assert.AreEqual(deqOrderedList[i].ClusterInfo.Id, enqOrderedList[i].ClusterInfo.Id);
            Assert.AreEqual(itemNumber, fakeProcessorFactory.LstResultList.Sum(i => i.Count));
            Console.WriteLine($"Avg Process {lstDuration.Average()}");
        }

        [TestMethod]
        public void Test_Async_Cluster_Complex_Allocation_JustFor_DifferentAndRepeatedKey()
        {
            var autoResetEvent = new AutoResetEvent(false);
            const int itemNumber = 100000;
            const int concurrentItems = 4;
            var logger = new FakeLogger(itemNumber, autoResetEvent);
            var lstDuration = new List<double>();
            Setup(logger, concurrentItems, 1);
            var st = new Stopwatch();
            st.Start();
            var tasks = new List<Task>();
            for (var i = 0; i < itemNumber; i++)
            {
                var haveClusterKeyItem = CreateItem(i % concurrentItems, i);
                tasks.Add(new Task(() => { syncClusterManager.Cluster(haveClusterKeyItem); }));
            }

            foreach (var task in tasks) task.Start();
            autoResetEvent.WaitOne();
            st.Stop();
            var duration = st.ElapsedMilliseconds;
            Console.WriteLine($"All Action Done Time = {duration}");

            var deqOrderedList = logger.LstDequeue.OrderBy(i => i.Item.Key).ToList();
            var enqOrderedList = logger.LstEnqueue.OrderBy(i => i.Item.Key).ToList();

            for (var i = 0; i < itemNumber; i++)
                lstDuration.Add((deqOrderedList[i].Time -
                                 enqOrderedList[i].Time).TotalMilliseconds);
            for (var i = 0; i < itemNumber; i++)
                Assert.AreEqual(deqOrderedList[i].ClusterInfo.Id, enqOrderedList[i].ClusterInfo.Id);
            Assert.AreEqual(itemNumber, fakeProcessorFactory.LstResultList.Sum(i => i.Count));
            Console.WriteLine($"Avg Process {lstDuration.Average()}");
        }

        [TestMethod]
        public void Test_Async_Cluster_Complex_Allocation_JustFor_Pause_And_Start()
        {
            var autoResetEvent = new AutoResetEvent(false);
            const int itemNumber = 100000;
            const int concurrentItems = 4;
            var logger = new FakeLogger(itemNumber, autoResetEvent);
            var lstDuration = new List<double>();
            Setup(logger, concurrentItems, 1);
            var st = new Stopwatch();
            st.Start();
            var tasks = new List<Task>();
            for (var i = 0; i < itemNumber; i++)
            {
                var haveClusterKeyItem = CreateItem(i % concurrentItems, i);
                tasks.Add(new Task(() => { syncClusterManager.Cluster(haveClusterKeyItem); }));
            }

            Task.Run(() =>
            {
                Task.Delay(2000).Wait();
                syncClusterManager.Resume();
            });
            syncClusterManager.Pause();
            foreach (var task in tasks) task.Start();
            autoResetEvent.WaitOne();
            st.Stop();
            var duration = st.ElapsedMilliseconds;
            Console.WriteLine($"All Action Done Time = {duration}");

            var deqOrderedList = logger.LstDequeue.OrderBy(i => i.Item.Key).ToList();
            var enqOrderedList = logger.LstEnqueue.OrderBy(i => i.Item.Key).ToList();

            for (var i = 0; i < itemNumber; i++)
                lstDuration.Add((deqOrderedList[i].Time -
                                 enqOrderedList[i].Time).TotalMilliseconds);
            for (var i = 0; i < itemNumber; i++)
                Assert.AreEqual(deqOrderedList[i].ClusterInfo.Id, enqOrderedList[i].ClusterInfo.Id);
            Assert.AreEqual(itemNumber, fakeProcessorFactory.LstResultList.Sum(i => i.Count));
            Console.WriteLine($"Avg Process {lstDuration.Average()}");
        }
    }
}