using TestCellHandshake.ApplicationLogic;
using TestCellHandshake.MqttService;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Channels.LineController;
using TestCellHandshake.MqttService.Channels.TestCell;
using TestCellHandshake.MqttService.MqttClient;
using TestCellHandshake.MqttService.MqttClient.ClientService;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service;
using TestCellHandshake.MqttService.MqttService.Configuration;
using TestCellHandshake.MqttService.MqttService.Service;
using TestCellHandshake.MqttService.MqttService.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mqtt Services
builder.Services.AddSingleton<IMqttService, MqttService>();
builder.Services.AddSingleton<IMainCommandChannel, MainCommandChannel>();
builder.Services.Configure<MqttConfig>(builder.Configuration.GetSection(MqttConfig.MqttSection));

// Services to mock Test Cell Handshake Logic 
builder.Services.AddTransient<IMqttClientService, MqttClientService>();
builder.Services.AddHostedService<MainWorker>();

// Application Services
builder.Services.AddTransient<IPayloadParser, PayloadParser>();
builder.Services.AddTransient<IDeviceDestinationService, DeviceDestinationService>();

// Infrastructure Services
builder.Services.AddSingleton<IMainMqttCommandChannel, MainMqttCommandChannel>();
builder.Services.AddHostedService<TestCellHandshakeProcessor>();

// Line Controller Logic Services
builder.Services.AddHostedService<NodeVikingRobotCellProcessor>();
builder.Services.AddTransient<ILogicHandlingService, LogicHandlingService>();

// Mqtt Test Cell Services
builder.Services.AddSingleton<ITestCellChannel, TestCellChannel>();
builder.Services.AddHostedService<TestCellWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
