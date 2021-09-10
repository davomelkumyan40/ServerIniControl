using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Client.Model;
using System.Net;
using System.Text;
using System.Timers;

namespace Client
{
    public sealed class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        public ClientConfig Configuration { get; private set; }
        public string ResponseMessage { get; private set; }
        public string RequestMessage { get; set; }
        public bool Connected { get; private set; }


        public Client(IConfigurationBuilder builder)
        {
            if (builder != null)
            {
                var config = builder.Build();
                var section = config.GetSection(nameof(ClientConfig));
                Configuration = section.Get<ClientConfig>();
                Connected = false;
            }
        }

        public async Task ConnectionCheckAsync()
        {
            RequestMessage = "connectioncheck"; // TODO CONST
            await SendMessageAsync();
            await ReciveMessageAsync();
            if (ResponseMessage == "valid") //TODO CONST
                Connected = true;
            else
                Connected = false;
            ResponseMessage = string.Empty;
            RequestMessage = string.Empty;
        }


        public async Task ConnectAsync()
        {
            try
            {
                var ip = IPAddress.Parse(Configuration.IpAddress);
                var endPoint = new IPEndPoint(ip, Configuration.Port);
                client = new TcpClient();
                await client.ConnectAsync(ip, Configuration.Port);
                stream = client.GetStream();
                Connected = client.Connected;
            }
            catch
            {
                Disconnect();
                throw;
            }
        }


        public bool Disconnect()
        {
            if (Connected)
            {
                client.Close();
                stream.Close();
                Connected = client.Connected;
                return true;
            }

            return false;
        }


        public async Task<bool> SendMessageAsync()
        {
            if (string.IsNullOrEmpty(RequestMessage) || string.IsNullOrWhiteSpace(RequestMessage))
                return false;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(RequestMessage);
                await stream.WriteAsync(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReciveMessageAsync()
        {
            try
            {
                byte[] data = new byte[256];
                int length = await stream.ReadAsync(data, 0, data.Length);
                ResponseMessage = Encoding.ASCII.GetString(data, 0, length);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
