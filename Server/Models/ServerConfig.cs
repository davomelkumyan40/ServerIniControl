namespace Server.Models
{
    public class ServerConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string IniDirectory { get; set; }
        public string[] ServerSideCommands { get; set; }
    }
}
