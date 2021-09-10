using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class ClientConfig
    {
        public string IpAddress { get; set; }
        public bool RetryConnection { get; set; }
        public int Port { get; set; }
        public int ConnectionTimeOut { get; set; }

    }
}
