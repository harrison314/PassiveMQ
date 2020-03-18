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
    public class PublisherTests : IntegrationTestBase
    {
        [DataTestMethod]
        [DataRow("/aaa", "/aaa", true)]
        [DataRow("/aaa", "/aaa/bbb", true)]
        [DataRow("/aaa", "/aaa/bbb/ccc", true)]
        [DataRow("/aaa", "/bbb", false)]
        [DataRow("/aaa/bbb/ccc", "/aaa", false)]
        [DataRow("/abc/abc/14", "/defines/458", false)]
        [DataRow(null, "/aaa", false)]
        [DataRow(null, "/aaa/48/bak", false)]
        public async Task Publisher_PublishAsync_CheckMatch(string topicClient, string publishTopic, bool match)
        {
            string message = "my test message";
            string label = "myLabel";

            PassiveMqQueuClient clientConsumer = this.Account.CreateQueueClient("myTestQueue");
            PassiveMqPublishClient publisher = this.Account.CreatePublishClient();

            try
            {
                await clientConsumer.DeleteIfExists();
                await clientConsumer.CrateIfNotExists(topicClient);

                await publisher.Publish(publishTopic, new MqCreateMessage(message, label));

                MqMessage resultMessage = await clientConsumer.GetMessage();

                Assert.AreEqual(match, resultMessage != null, $"Inconsistent topic client '{topicClient}', topic publish '{publishTopic}', excepted match {match}.");

                if (match == true)
                {
                    Assert.AreEqual(message, resultMessage.AsString);
                    Assert.AreEqual(label, resultMessage.Label);
                }
            }
            finally
            {
                await clientConsumer.DeleteIfExists();
            }
        }

        [TestMethod]
        public async Task GetMessageAsync_Batch()
        {
            string message = "my test message";
            string label = "myLabel";

            PassiveMqQueuClient clientConsumer1 = this.Account.CreateQueueClient("myTestQueue1");
            PassiveMqQueuClient clientConsumer2 = this.Account.CreateQueueClient("myTestQueue2");
            PassiveMqQueuClient clientConsumer3 = this.Account.CreateQueueClient("myTestQueue3");
            PassiveMqQueuClient clientConsumer4 = this.Account.CreateQueueClient("myTestQueue4");
            PassiveMqQueuClient clientConsumerNull = this.Account.CreateQueueClient("myTestQueueNull");

            PassiveMqPublishClient publisher = this.Account.CreatePublishClient();

            try
            {
                await clientConsumer1.DeleteIfExists();
                await clientConsumer1.CrateIfNotExists("/myTestConsumer/aa");

                await clientConsumer2.DeleteIfExists();
                await clientConsumer2.CrateIfNotExists("/myTestConsumer/aa/bb");

                await clientConsumer3.DeleteIfExists();
                await clientConsumer3.CrateIfNotExists("/myTestConsumer/aa/bb/cc");

                await clientConsumer4.DeleteIfExists();
                await clientConsumer4.CrateIfNotExists("/myTestConsumer/bb/4");


                await publisher.Publish("/myTestConsumer/aa/bb/cc/75", new MqCreateMessage(message, label));

                MqMessage message1 = await clientConsumer1.GetMessage();
                MqMessage message2 = await clientConsumer2.GetMessage();
                MqMessage message3 = await clientConsumer3.GetMessage();
                MqMessage message4 = await clientConsumer4.GetMessage();

                Assert.IsNotNull(message1);
                Assert.IsNotNull(message2);
                Assert.IsNotNull(message3);
                Assert.IsNull(message4);
            }
            finally
            {
                await clientConsumer1.DeleteIfExists();
                await clientConsumer2.DeleteIfExists();
                await clientConsumer3.DeleteIfExists();
                await clientConsumer4.DeleteIfExists();
            }
        }
    }
}
