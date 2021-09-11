using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

namespace Client
{
    class Program
    {
        static string[] commands = new string[] { "--get", "--set", "--help", "--reset", "--exit" };

        static async Task Main(string[] args)
        {
            UI.InitUi();

            ClientCommands currentCommand = ClientCommands.None;
            while (currentCommand != ClientCommands.Exit)
            {
                IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

                Client client = new Client(configBuilder);
                AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                {
                    client.Disconnect();
                };

                if (client.Configuration.RetryConnection)
                {
                    while (!client.Connected)
                    {
                        try
                        {
                            await Task.Delay(client.Configuration.ConnectionTimeOut);
                            await client.ConnectAsync();
                            if (client.Connected)
                                UI.SuccessMessage(client);
                            else
                                UI.FailedMessage(client);
                        }
                        catch (Exception ex)
                        {
                            if (ex is SocketException)
                                UI.ServerIsNotAvailableMessage(client);
                            else
                                UI.FailedMessage(client, ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await client.ConnectAsync();
                        if (client.Connected)
                            UI.SuccessMessage(client);
                        else
                            UI.FailedMessage(client);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException)
                            UI.ServerIsNotAvailableMessage(client);
                        else
                            UI.FailedMessage(client, ex.Message);
                        if (!client.Connected)
                            break;
                    }
                }

                UI.DefaultMessage();

                while (currentCommand != ClientCommands.Exit & currentCommand != ClientCommands.Reconnect)
                {
                    await client.ConnectionCheckAsync();
                    if (!client.Connected)
                        break;
                    Console.Write($"{AppDomain.CurrentDomain.BaseDirectory} $ ");
                    string input = Console.ReadLine();
                    var instruction = ClientInstructions.Parse(input);
                    if (instruction.Identifier == "client")
                    {
                        if (commands.Any(c => c == instruction.Command))
                        {
                            switch (instruction.Command)
                            {
                                case "--help":
                                    currentCommand = ClientCommands.Help;
                                    break;
                                case "--exit":
                                    currentCommand = ClientCommands.Exit;
                                    break;
                                case "--reset":
                                    currentCommand = ClientCommands.Reconnect;
                                    break;
                                case "--set":
                                    currentCommand = ClientCommands.Set;
                                    break;
                                case "--get":
                                    currentCommand = ClientCommands.Get;
                                    break;
                            }

                            switch (currentCommand)
                            {
                                case ClientCommands.Help:
                                    UI.HelpMessage();
                                    break;
                                case ClientCommands.Set:
                                case ClientCommands.Get:
                                    client.RequestMessage = instruction.FullInstruction;
                                    if (await client.SendMessageAsync())
                                    {
                                        UI.SentMessage(client);
                                        if (await client.ReciveMessageAsync())
                                        {
                                            UI.RecivedMessage(client);
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (string.IsNullOrEmpty(instruction.Command) || string.IsNullOrWhiteSpace(instruction.Command))
                        {
                            foreach (var cmd in commands)
                            {
                                Console.WriteLine($"{new string(' ', 5)}{cmd}");
                            }
                        }
                        else
                            Console.WriteLine($"No such a command '{instruction.Command}'");
                    }
                    else
                        Console.WriteLine($"No such an identifier '{instruction.Identifier}'");
                }

                if (client.Disconnect())
                    UI.DisconnectedMessage(client);
                if (currentCommand == ClientCommands.Reconnect)
                {
                    currentCommand = ClientCommands.None;
                    continue;
                }
            }
        }
    }
}
