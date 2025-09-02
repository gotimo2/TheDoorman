using CoreRCON;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TheDoorman
{
    public class DiscordBot(ILogger<DiscordBot> logger, IOptions<DiscordConfig> config, RCONService rcon) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (config.Value is null)
            {
                logger.LogError("No config was found for Discord. verify this exists.");
            }

            var configuration = config.Value;

            return Task.CompletedTask;
        }
    }
}
