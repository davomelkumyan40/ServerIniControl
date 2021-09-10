using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IniManager;
using Server.TCP;
using System.Linq;
using System.Net.NetworkInformation;

namespace Server
{
    class Program
    {
        private static IniConfigProvider iniProvider;
        private static ServerCommands currentCommand = ServerCommands.None;

        static async Task Main(string[] args)
        {
            UI.InitUi();

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");


            iniProvider = new IniConfigProvider();
            ServerHost host = new ServerHost(builder);
            host.ClientConnected += (sender, e) => Console.WriteLine($"\nConnected: [{e.ClientId}]");
            host.ClientDisconnected += (sender, e) => Console.WriteLine($"\nDisconnect: [{e.ClientId}]");
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                host.Stop();
            };

            if (PortIsNotFree(host.Configuration.Port))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Server Error: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\tCheck 'appsettings.json' section [Port] = {host.Configuration.Port} is used by another host");
            }

                while (currentCommand != ServerCommands.Exit)
            {
                Console.Write("host: $ ");
                var commands = Console.ReadLine().Split(" ");
                SetInstruction(commands[0]);
                var arguments = commands.Skip(1).ToArray();
                switch (currentCommand)
                {
                    case ServerCommands.Start:
                        try
                        {
                            host.Start();
                            UI.ListeningMessage(host);
                            host.Listen();
                        }
                        catch (Exception ex)
                        {
                            UI.FailedListening(host, ex.Message);
                        }
                        break;
                    case ServerCommands.Help:
                        UI.HelpMessage(host);
                        break;
                    case ServerCommands.Set:
                        var res1 = await host.IniProvider.SetValueAsync(arguments[0], arguments[1]); //TODO check string is null or empty
                        if (res1 == 0)
                            Console.WriteLine($">>: {arguments[0]} = {arguments[1]}");
                        else
                            Console.WriteLine($"Something gone wrong: check [{string.Join(" ", arguments)}]");
                        break;
                    case ServerCommands.Get:
                        var value = string.Empty;
                        var res2 = host.IniProvider.TryGetValue(arguments[0], ref value); //TODO check string is null or empty
                        if (res2 == 0)
                            Console.WriteLine($">>: {value}");
                        else
                            Console.WriteLine($"Something gone wrong: check [{string.Join(" ", arguments)}]");
                        break;
                    case ServerCommands.Reconnect:
                        try
                        {
                            host.Stop();
                            UI.ServerStopedMessage(host);
                            host.Start();
                            UI.ListeningMessage(host);
                            host.Listen();
                        }
                        catch (Exception ex)
                        {
                            UI.FailedListening(host, ex.Message);
                        }
                        break;
                    case ServerCommands.Load:
                        host.IniProvider.Load(host.Configuration.IniDirectory);
                        UI.DisplayData(host.IniProvider.Data);
                        break;
                    case ServerCommands.Stop:
                        host.StopListening();
                        UI.ServerStopedMessage(host);
                        break;
                    default:
                        UI.UnknownCommand(string.Join(" ", commands));
                        break;
                }
                if (currentCommand == ServerCommands.Exit)
                    host.Stop();
            }
            host.Stop();
        }

        private static bool PortIsNotFree(int port)
        {
            IPGlobalProperties props = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnectionInfos = props.GetActiveTcpConnections();
            return tcpConnectionInfos.Any(p => p.LocalEndPoint.Port == port);
        }

        private static void SetInstruction(string input)
        {
            switch (input)
            {
                case "START":
                    currentCommand = ServerCommands.Start;
                    break;
                case "STOP":
                    currentCommand = ServerCommands.Stop;
                    break;
                case "HELP":
                    currentCommand = ServerCommands.Help;
                    break;
                case "RESET":
                    currentCommand = ServerCommands.Reconnect;
                    break;
                case "EXIT":
                    currentCommand = ServerCommands.Exit;
                    break;
                case "GET":
                    currentCommand = ServerCommands.Get;
                    break;
                case "SET":
                    currentCommand = ServerCommands.Set;
                    break;
                case "LOAD":
                    currentCommand = ServerCommands.Load;
                    break;
                default:
                    currentCommand = ServerCommands.None;
                    break;
            }
        }
    }
}
