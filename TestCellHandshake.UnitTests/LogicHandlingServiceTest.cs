using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TestCellHandshake.MqttService.Channels.LineController;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service;

namespace TestCellHandshake.MqttService.Tests.MqttClient.Service
{
    [TestClass]
    public class LogicHandlingServiceTests
    {
        private ILogicHandlingService? _logicHandlingService;
        private Mock<IPayloadParser>? _payloadParser;
        private Mock<IMainMqttCommandChannel>? _mainMqttCommandChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            _payloadParser = new Mock<IPayloadParser>();
            _mainMqttCommandChannel = new Mock<IMainMqttCommandChannel>();
            _logicHandlingService = new LogicHandlingService(new NullLogger<LogicHandlingService>(), _payloadParser.Object, _mainMqttCommandChannel.Object);

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