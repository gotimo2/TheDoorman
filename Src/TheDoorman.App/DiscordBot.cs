using CoreRCON;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace TheDoorman
{
    public class DiscordBot(ILogger<DiscordBot> logger, IOptions<DiscordConfig> config, DiscordSocketClient client, RCONService rcon) : BackgroundService
    {
        private readonly ILogger<DiscordBot> _logger = logger;
        private readonly DiscordConfig _config = config.Value ?? throw new ArgumentException("No Discord configuration provided.");
        private readonly DiscordSocketClient _client = client;
        private readonly RCONService _rcon = rcon;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_config.Token)) {
                _logger.LogError("Token is empty!");
            }

            HttpClient httpclient = new HttpClient();

            var result = await httpclient.GetAsync("https://discord.com/api/v10/gateway");
            
            if (result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully pinged discord gateway.");
            }
            
            _logger.LogInformation("Connecting to discord using token {Token}...", string.Join(string.Empty, _config.Token.Take(0..3).Concat(Enumerable.Repeat('*', _config.Token.Length - 3))));

            _client.Log += (log) => {
                _logger.LogInformation("Discord.NET: {Source} : {Severity} : {Message} {Exception} ", log.Source, log.Severity.ToString(), log.Message, log.Exception.Message);
                return Task.CompletedTask;    
            }; 

            await _client.LoginAsync(TokenType.Bot, _config.Token);

            await _client.StartAsync();

            while (_client.ConnectionState != ConnectionState.Connected)
            { 
                await Task.Delay(100, stoppingToken);
            }

            _logger.LogInformation("Connected to discord under {Account}", _client.CurrentUser.Username);

            await CreateCommands();

            _client.SlashCommandExecuted += HandleCommand;

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task CreateCommands()
        {
            var guild = _client.GetGuild(_config.GuildId);

            _logger.LogInformation("Registering commands in Guild {GuildName}", guild?.Name ?? throw new ArgumentException($"No guild found by ID {_config.GuildId}"));

            var builder = new SlashCommandBuilder()
                .WithName(CommandNames.Checkin)
                .WithDescription("Add the specified name to the server whitelist.")
                .WithContextTypes(InteractionContextType.Guild)
                .AddOption("username", ApplicationCommandOptionType.String, "The username to add", isRequired: true, minLength: 4, maxLength: 15);

            await _client.Rest.CreateGuildCommand(builder.Build(), guild.Id);

            _logger.LogInformation("Registered commands.");
        }

        private async Task HandleCommand(SocketSlashCommand command)
        {
            _logger.LogDebug("Handling command {Name}", command.CommandName);

#if DEBUG
            await command.DeferAsync();
#endif

            Task<string> result = command.CommandName switch
            {
                CommandNames.Checkin => _rcon.AddToWhitelist(command.Data.Options.FirstOrDefault().Value as string ?? string.Empty),
                _ => Task.FromResult("Command not found.")
            };

            var message = await result;

            await command.RespondAsync(text: message, ephemeral: false);
        }
    }
}
