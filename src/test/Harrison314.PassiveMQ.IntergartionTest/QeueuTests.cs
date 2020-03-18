using Harrison314.PassiveMQ.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.IntergartionTest
{
    [TestClass]
    public class QeueuTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task CrateIfNotExistsAsync_Single()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task CrateIfNotExistsAsync_Multiple()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            try
            {
                await client.CrateIfNotExists();
                await client.CrateIfNotExists();
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task DeleteIfExistsAsync_Multiple()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.CrateIfNotExists();
            await client.DeleteIfExists();
            await client.DeleteIfExists();
        }

        [TestMethod]
        public async Task ExistsQueueAsync_Get()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            try
            {
                await client.CrateIfNotExists();
                bool exists1 = await client.ExistsQueue();
                Assert.IsTrue(exists1);
                await client.DeleteIfExists();
                bool exists2 = await client.ExistsQueue();
                Assert.IsFalse(exists2);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [DataTestMethod]
        [DataRow("myTestQueue", null, null)]
        [DataRow("myTestQueue", null, "http://localhost:145/adress?t=$(messageId)&k=4")]
        [DataRow("myTestQueue475", "/myDevice", null)]
        [DataRow("myTestQueue475", "/myDevice/any_result", null)]
        [DataRow("myTestQueue475", "/myDevice/any_result", "http://webhook.azure.com:145/adress/hooks")]
        [DataRow("myTestQueue475", "/myDevice/any_result/destinations/458", null)]
        public async Task GetDefinitionAsync_Get(string qeueuName, string topic, string notificationAdress)
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient(qeueuName);

            try
            {
                await client.CrateIfNotExists(topic, notificationAdress);

                Queu info = await client.GetDefinition();

                Assert.AreEqual(qeueuName, info.Name);
                Assert.AreEqual(topic, info.TopicPattern);
                Assert.AreEqual(notificationAdress, info.NotificationAdress);

                Assert.AreNotEqual(Guid.Empty, info.Id);
                Assert.AreNotEqual(default(DateTime), info.Created);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [DataTestMethod]
        [DataRow("queeueu1", null, null, null)]
        [DataRow("queeueu2", null, "http://localhost/update1", null)]
        [DataRow("queeueu3", null, "http://localhost/update1", "http://localhost/update45")]
        [DataRow("queeueu3", "http://localhost/update0", "http://localhost/update1", "http://localhost/update45")]
        [DataRow("queeueu3", "http://localhost/update0", null, "http://localhost/update45")]
        public async Task SetNotificationAdressAsync_Updated(string qeueuName, string naBase, string na1, string na2)
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient(qeueuName);

            try
            {
                await client.CrateIfNotExists(null, naBase);

                Queu info = await client.GetDefinition();
                Assert.AreEqual(naBase, info.NotificationAdress);

                await client.SetNotificationAdress(na1);
                Queu info1 = await client.GetDefinition();
                Assert.AreEqual(na1, info1.NotificationAdress);

                await client.SetNotificationAdress(na2);
                Queu info2 = await client.GetDefinition();
                Assert.AreEqual(na2, info2.NotificationAdress);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }
    }
}
