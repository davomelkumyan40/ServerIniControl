using System;
using System.Threading.Tasks;
using IniManager;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IniConfigProvider provider = new IniConfigProvider();
            String s = String.Empty;
            if(await provider.LoadAsync("./settings.ini") == 0)
            {
                await provider.SetValueAsync("Scheme.color", "blue");
                await provider.SetValueAsync("Scheme.background-Image", "bg2.jpg");
                await provider.SetValueAsync("database.password", "123456");
                provider.TryGetValue("Scheme.color", ref s);
                provider.Dispose();
            }
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
