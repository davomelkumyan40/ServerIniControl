using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public class ClientInstructions
    {
        public string Identifier { get; private set; }
        public string Command { get; private set; }
        public List<string> Arguments { get; private set; }

        public string FullInstruction => $"{Identifier} {Command} {string.Join(" ", Arguments)}";


        public ClientInstructions()
        {
            Identifier = "";
            Command = "";
            Arguments = new List<string>();
        }

        public ClientInstructions(string identifier, string command, params string[] arguments)
        {
            Identifier = identifier;
            Command = command;
            Arguments = arguments.ToList();
        }

        public static ClientInstructions Parse(string input)
        {
            var e = input.Split(" ").GetEnumerator();
            ClientInstructions instruction = new ClientInstructions();
            if (e.MoveNext())
            {
                instruction.Identifier = e.Current as string;
                if (e.MoveNext())
                {
                    instruction.Command = e.Current as string;
                    while (e.MoveNext())
                        instruction.Arguments.Add(e.Current as string);
                }
            }
            return instruction;
        }
    }
}
