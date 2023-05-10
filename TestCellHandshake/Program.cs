using TestCellHandshake.MqttService;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Channels.MqttLc;
using TestCellHandshake.MqttService.MqttService.Configuration;
using TestCellHandshake.MqttService.MqttService.Service;
using TestCellHandshake.MqttService.MqttService.Workers;
using TestCellHandshake.OpcuaService.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMqttService, MqttService>();
builder.Services.AddSingleton<IMainCommandChannel, MainCommandChannel>();
builder.Services.AddSingleton<IMainMqttCommandChannel, MainMqttCommandChannel>();
builder.Services.Configure<MqttConfig>(builder.Configuration.GetSection(MqttConfig.MqttSection));
builder.Services.Configure<OpcuaConfig>(builder.Configuration.GetSection(OpcuaConfig.OpcuaSection));
builder.Services.AddHostedService<MqttLineControllerWorker>();
builder.Services.AddHostedService<MainWorker>();

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
