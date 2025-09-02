using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDoorman
{
    public class RconConfig
    {
        public string Host { get; set; } = string.Empty;

        public int RCONPort { get; set; } = 25575; 

        public string RCONPassword { get; set; } = string.Empty;
    }
}
