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
    public class MessagesTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task GetCountAsync_Zero()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();

                int messagesCount = await client.GetCount();
                Assert.AreEqual(0, messagesCount);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task GetCountAsync_More()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();

                await client.AddMessage(new MqCreateMessage("any message"));
                Assert.AreEqual(1, await client.GetCount());
                await client.AddMessage(new MqCreateMessage("any message"));
                Assert.AreEqual(2, await client.GetCount());
                await client.AddMessage(new MqCreateMessage("any message"));
                Assert.AreEqual(3, await client.GetCount());
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [DataTestMethod]
        [DataRow("message1", null)]
        [DataRow("Any message for send lorem ipsun .... 0123456789 %_ábč235žýýáíé", null)]
        [DataRow("Message 2 ", "test label")]
        [DataRow("message1", "Harrison314.SqlMq.IntegrationTests.MessagesTests, Harrison314.SqlMq.IntegrationTests")]
        [DataRow("Message 458", "{6C066595-A120-4C4D-91DF-29567544ACE6}")]
        public async Task AddMessageAsync_Success(string messageText, string label)
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();

                MqCreateMessage sendMessage = new MqCreateMessage(messageText, label);
                MqMessage createdMessage = await client.AddMessage(sendMessage);

                Assert.IsNotNull(createdMessage);
                Assert.AreEqual(messageText, createdMessage.AsString);
                Assert.AreEqual(label, createdMessage.Label);

                Assert.AreNotEqual(Guid.Empty, createdMessage.Id);
                Assert.AreNotEqual(default(DateTime), createdMessage.InsertionTime);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task PeekMessageAsync_Empty()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                MqMessage nullMessage = await client.PeekMessage();
                Assert.IsNull(nullMessage);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [DataTestMethod]
        [DataRow("message1", null)]
        [DataRow("message2", "any label")]
        public async Task PeekMessageAsync_Result(string message, string label)
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage(message, label));
                MqMessage realMessage = await client.PeekMessage();

                Assert.AreEqual(message, realMessage.AsString);
                Assert.AreEqual(label, realMessage.Label);

                Assert.AreNotEqual(Guid.Empty, realMessage.Id);
                Assert.AreNotEqual(default(DateTime), realMessage.InsertionTime);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task PeekMessageAsync_VisibilityTime()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage("some messagefor MOM"));
                MqMessage realMessage = await client.GetMessage(TimeSpan.FromSeconds(5));
                Assert.IsNotNull(realMessage);
                Assert.AreEqual(1, realMessage.RetryCount);

                MqMessage realNextMessage = await client.PeekMessage();
                Assert.IsNull(realNextMessage);

                await Task.Delay(10000);

                MqMessage realRentrantMessage = await client.PeekMessage();
                Assert.IsNotNull(realRentrantMessage);
                Assert.AreEqual(1, realRentrantMessage.RetryCount);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [DataTestMethod]
        [DataRow("message1", null)]
        [DataRow("message2", "any label")]
        public async Task GetMessageAsync_CheckValues(string message, string label)
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage(message, label));
                MqMessage realMessage = await client.GetMessage();

                Assert.AreEqual(message, realMessage.AsString);
                Assert.AreEqual(label, realMessage.Label);

                Assert.AreNotEqual(Guid.Empty, realMessage.Id);
                Assert.AreNotEqual(default(DateTime), realMessage.InsertionTime);
                Assert.AreNotEqual(null, realMessage.NextVisibleTime);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task GetMessageAsync_VisibilityTime()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage("some messagefor MOM"));
                MqMessage realMessage = await client.GetMessage(TimeSpan.FromSeconds(5));
                Assert.IsNotNull(realMessage);
                Assert.AreEqual(1, realMessage.RetryCount);

                MqMessage realNextMessage = await client.GetMessage();
                Assert.IsNull(realNextMessage);

                await Task.Delay(10000);

                MqMessage realRentrantMessage = await client.GetMessage();
                Assert.IsNotNull(realRentrantMessage);
                Assert.AreEqual(2, realRentrantMessage.RetryCount);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task DeleteMessageAsync_FromCreated()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                MqMessage message = await client.AddMessage(new MqCreateMessage("some messagefor MOM"));

                await client.DeleteMessage(message);
                MqMessage newMessage = await client.PeekMessage();
                Assert.IsNull(newMessage);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task DeleteMessageAsync_FromPeek()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage("some messagefor MOM"));

                MqMessage message = await client.PeekMessage();
                await client.DeleteMessage(message);
                MqMessage newMessage = await client.PeekMessage();
                Assert.IsNull(newMessage);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task DeleteMessageAsync_FromGetMessage()
        {
            PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");
            await client.DeleteIfExists();

            try
            {
                await client.CrateIfNotExists();
                await client.AddMessage(new MqCreateMessage("some messagefor MOM"));

                MqMessage message = await client.GetMessage();
                await client.DeleteMessage(message);
                MqMessage newMessage = await client.PeekMessage();
                Assert.IsNull(newMessage);
            }
            finally
            {
                await client.DeleteIfExists();
            }
        }
    }
}
