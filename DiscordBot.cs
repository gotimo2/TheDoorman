using CoreRCON;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            await _client.LoginAsync(TokenType.Bot, _config.Token);

            await _client.StartAsync();

            while (_client.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(400);
            }

            await CreateCommand();

            _client.SlashCommandExecuted += HandleCommand;

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task CreateCommand()
        {
            var guild = _client.GetGuild(_config.GuildId);
            //1309276024890069064
            var builder = new SlashCommandBuilder()
                .WithName(CommandNames.Checkin)
                .WithDescription("Add the specified name to the server whitelist.")
                .WithContextTypes(InteractionContextType.Guild)
                .AddOption("username", ApplicationCommandOptionType.String, "The username to add", isRequired: true, minLength: 4, maxLength: 15);
            await _client.Rest.CreateGuildCommand(builder.Build(), guild.Id);
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

            await command.RespondAsync(text: message, ephemeral: true);
        }
    }
}
