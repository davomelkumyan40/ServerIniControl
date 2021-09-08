using System;
using IniManager;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IniConfigProvider provider = new IniConfigProvider("./settings.ini");
            String s = String.Empty;
            if(provider.Load() == 0)
            {
                provider.SetValue("Scheme.color", "blue");
                provider.SetValue("Scheme.background-Image", "bg2.jpg");
                provider.SetValue("database.password", "123456");
                provider.TryGetValue("Scheme.color", ref s);
                provider.Dispose();
            }
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
