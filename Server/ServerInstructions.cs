using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class ServerInstructions
    {
        public string Command { get; private set; }
        public List<string> Arguments { get; private set; }
        public string Identifier { get; private set; }

        public string FullInstruction => $"{Command} {string.Join(" ", Arguments)}";


        public ServerInstructions()
        {
            Command = "";
            Arguments = new List<string>();
        }

        public ServerInstructions(string command, params string[] arguments)
        {
            Command = command;
            Arguments = arguments.ToList();
        }

        public static ServerInstructions Parse(string input)
        {
            var e = input.Split(" ").GetEnumerator();
            ServerInstructions instruction = new ServerInstructions();
            if (e.MoveNext())
            {
                instruction.Command = e.Current as string;
                while (e.MoveNext())
                    instruction.Arguments.Add(e.Current as string);
            }
            return instruction;
        }

        public static ServerInstructions ParseClient(string input)
        {
            var e = input.Split(" ").GetEnumerator();
            ServerInstructions instruction = new ServerInstructions();
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
