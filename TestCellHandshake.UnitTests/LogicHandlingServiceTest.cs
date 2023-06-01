using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MQTTnet;
using MQTTnet.Client;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service;

namespace TestCellHandshake.MqttService.Tests.MqttClient.Service
{
    [TestClass]
    public class LogicHandlingServiceTests
    {
        private ILogicHandlingService? _logicHandlingService;
        private Mock<IPayloadParser>? _payloadParser;

        [TestInitialize]
        public void TestInitialize()
        {
            _payloadParser = new Mock<IPayloadParser>();
            _logicHandlingService = new LogicHandlingService(new NullLogger<LogicHandlingService>(), _payloadParser.Object);

        }

        [TestMethod]
        public void HandleApplicationMessageReceived_Test()
        {
            //var eventArgs = new MqttApplicationMessageReceivedEventArgs("Test", new MqttApplicationMessageBuilder().WithPayload("TestPayload").WithTopic("TestTopic").Build(), null, null);
            //_logicHandlingService.HandleApplicationMessageReceived(eventArgs);
            //Assert.IsTrue(true);
        }
    }
}