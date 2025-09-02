using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDoorman
{
    public class DiscordConfig
    {
        public string Token { get; set; } = string.Empty;

        public ulong GuildId { get; set; } = 0;
    }
}
