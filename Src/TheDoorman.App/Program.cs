using Discord.WebSocket;
namespace TheDoorman;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<DiscordBot>();
        builder.Services.AddLogging();

        builder.Services.Configure<DiscordConfig>(builder.Configuration.GetSection("Discord"));
        builder.Services.Configure<RconConfig>(builder.Configuration.GetSection("Minecraft"));

        builder.Services.AddSingleton<RCONService>();

        var config = new DiscordSocketConfig();

        builder.Services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>();

        var host = builder.Build();
        await host.RunAsync();
    }
}