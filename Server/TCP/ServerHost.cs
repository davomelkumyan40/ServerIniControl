using Microsoft.Extensions.Configuration;
using Server.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IniManager;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Server.TCP
{
    public class ServerHost
    {
        //client side action commands
        private static string[] clientActionCommands = new string[] { "--get", "--set" };
        private TcpListener listener;
        public IniConfigProvider IniProvider { get; private set; }

        public event EventHandler<ConnectionEventArgs> ClientConnected;
        public event EventHandler<ConnectionEventArgs> ClientDisconnected;
        public ServerConfig Configuration { get; private set; }
        public bool Listening { get; private set; }
        public List<TcpClient> Clients { get; private set; }

        public ServerHost(IConfigurationBuilder builder)
        {
            if (builder != null)
            {
                var config = builder.Build().GetSection(nameof(ServerConfig));
                Configuration = config.Get<ServerConfig>();
                Listening = false;
                Clients = new List<TcpClient>();
            }
            InitServices();
        }

        public void InitServices()
        {
            IniProvider = new IniConfigProvider();
            IniProvider.Load(Configuration.IniDirectory);
        }

        public void Start()
        {
            IPAddress localAddress = IPAddress.Parse(Configuration.IpAddress);
            IPEndPoint endPoint = new IPEndPoint(localAddress, Configuration.Port);
            listener = new TcpListener(endPoint);
            listener.Start();
            Listening = true;
        }


        public void Listen()
        {
            ThreadPool.QueueUserWorkItem(async (state) =>
            {
                try
                {

                    while (Listening)
                    {
                        TcpClient client = await listener.AcceptTcpClientAsync();
                        Clients.Add(client);
                        ThreadPool.QueueUserWorkItem(HandleClient, client);
                    }
                }
                catch
                {
                    Stop();
                }
            });

        }

        public void StopListening()
        {
            Listening = false;
        }

        public void Stop()
        {
            Clients?.ForEach(c => c.Close());
            listener?.Stop();
        }

        //Bad practice to return void while using async await
        public async void HandleClient(object state)
        {
            TcpClient client = state as TcpClient;
            //subscribe to event for more control
            ClientConnected(this, new ConnectionEventArgs(Clients.IndexOf(client)));

            try
            {
                while (client.Connected)
                {
                    string data = await ReciveMessageAsync(client);
                    if (ConnectionCheck(data))
                    {
                        await SendMessageAsync(client, $"valid");
                        continue;
                    }
                    ServerInstructions instruction = ServerInstructions.ParseClient(data);
                    if (instruction.Identifier == "client")
                    {

                        if (instruction.Arguments.All(a => !string.IsNullOrEmpty(a)) && instruction.Arguments.Any())
                            switch (instruction.Command)
                            {
                                case "--get":
                                    string value = string.Empty;
                                    var outputG = IniProvider.TryGetValue(instruction.Arguments[0], ref value);
                                    if (outputG == 0)
                                        await SendMessageAsync(client, $"get: {value}");
                                    else if (outputG == 255)
                                        await SendMessageAsync(client, $"Something gone wrong Error code [{outputG}]");
                                    else
                                        await SendMessageAsync(client, $"no such a [key]: [{instruction.Arguments[0]}] check [key]");
                                    break;
                                case "--set":
                                    var outputS = await IniProvider.SetValueAsync(instruction.Arguments[0], instruction.Arguments[1]);
                                    if (outputS == 0)
                                        await SendMessageAsync(client, $"set: [{instruction.Arguments[0]}] = {instruction.Arguments[1]}");
                                    else
                                        await SendMessageAsync(client, $"Something gone wrong Error code [{outputS}]");
                                    break;
                            }
                    }
                }
            }
            catch
            {
                client.Close();
                ClientDisconnected(this, new ConnectionEventArgs(Clients.IndexOf(client)));
                Clients.Remove(client);
            }
        }

        public bool ConnectionCheck(string signal)
        {
            if (signal == "connectioncheck")
                return true;
            return false;
        }

        public string ReciveMessage(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];
            int i = stream.Read(data, 0, data.Length);
            return Encoding.ASCII.GetString(data, 0, i);
        }

        public async Task<string> ReciveMessageAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];
            int i = await stream.ReadAsync(data, 0, data.Length);
            return Encoding.ASCII.GetString(data, 0, i);
        }

        public void SendMessage(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }


        public async Task SendMessageAsync(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
