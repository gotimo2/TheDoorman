using Discord.WebSocket;
using TheDoorman;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DiscordBot>();
builder.Services.AddLogging();

builder.Services.Configure<DiscordConfig>(builder.Configuration.GetSection("Discord"));
builder.Services.Configure<RconConfig>(builder.Configuration.GetSection("Minecraft"));

builder.Services.AddSingleton<RCONService>();

var config = new DiscordSocketConfig()
{

};


builder.Services
    .AddSingleton(config)
    .AddSingleton<DiscordSocketClient>();


var host = builder.Build();
host.Run();
