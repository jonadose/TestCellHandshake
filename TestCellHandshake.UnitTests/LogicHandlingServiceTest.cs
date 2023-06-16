using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TestCellHandshake.ApplicationLogic.Channels.RequestChannel;
using TestCellHandshake.ApplicationLogic.Channels.ResponseChannel;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service;

namespace TestCellHandshake.UnitTests
{
    [TestClass]
    public class LogicHandlingServiceTests
    {
        private ILogicHandlingService? _logicHandlingService;
        private Mock<IPayloadParser>? _payloadParser;
        private Mock<IHandshakeRequestChannel>? _handshakeRequestChannel;
        private Mock<IDeviceDestinationService>? _deviceDestinationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _payloadParser = new Mock<IPayloadParser>();
            _handshakeRequestChannel = new Mock<IHandshakeRequestChannel>();
            _deviceDestinationService = new Mock<IDeviceDestinationService>();
            _logicHandlingService = new LogicHandlingService(
                new NullLogger<LogicHandlingService>(),
                _payloadParser.Object,
                _handshakeRequestChannel.Object,
                _deviceDestinationService.Object);

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