using Harrison314.PassiveMQ.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.IntergartionTest
{
    [TestClass]
    public class MessageOrderTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task GetMessageAsync_Batch()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient($"myTestQueue-{Guid.NewGuid()}");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();

                for (int i = 1; i < 10; i++)
                {
                    await client.AddMessage(new MqCreateMessage(i.ToString()));
                }

                await Task.Delay(50);

                List<int> results = new List<int>();
                for (int i = 1; i < 10; i++)
                {
                    MqMessage message = await client.GetMessage();

                    results.Add(int.Parse(message.AsString));
                }

                CollectionAssert.AllItemsAreUnique(results, "GetMessageAsync rerurns multiple same result.");
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task GetMessageAsync_BatchWithDelete()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient($"myTestQueue-{Guid.NewGuid()}");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();

                for (int i = 1; i < 10; i++)
                {
                    await client.AddMessage(new MqCreateMessage(i.ToString()));
                }

                await Task.Delay(50);

                List<int> results = new List<int>();
                for (int i = 1; i < 10; i++)
                {
                    MqMessage message = await client.GetMessage();

                    results.Add(int.Parse(message.AsString));
                    await Task.Delay(3);

                    await client.DeleteMessage(message.Id);
                }

                CollectionAssert.AllItemsAreUnique(results, "GetMessageAsync rerurns multiple same result.");
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }
    }
}
