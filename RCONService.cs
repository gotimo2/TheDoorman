using CoreRCON;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheDoorman
{
    public partial class RCONService
    {
        private readonly ILogger<RCONService> _logger;
        private readonly RconConfig _config;
        private readonly RCON _client;

        public RCONService(ILogger<RCONService> logger, IOptions<RconConfig> config)
        {
            _logger = logger;
            _config = config.Value ?? throw new ArgumentException("No config found for RCON client.");

            _client = new RCON(new IPEndPoint(IPAddress.Parse(_config.HostIpAddress), _config.RCONPort), _config.RCONPassword);
        }

        public async Task<string> AddToWhitelist(string username)
        {
            _logger.LogInformation("Adding user {Username} to whitelist.", username);

            var isValid = ValidUsernameRegex().IsMatch(username) && username.Length > 3 && username.Length < 16;

            if (!isValid)
            {
                return "Invalid username!";
            }

            await _client.ConnectAsync();

            var result = await _client.SendCommandAsync(string.Format("whitelist add {0}", username));

            _logger.LogDebug("Server response: {result}", result);

            return $"Added user {username} to whitelist.";
        }

        [GeneratedRegex(@"^[a-zA-Z0-9_]{3,16}$", RegexOptions.Compiled)]
        private static partial Regex ValidUsernameRegex();
    }
}
