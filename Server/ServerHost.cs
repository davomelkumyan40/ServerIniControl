using System;
using System.Threading.Tasks;
using System.ServiceModel;

using IniManager;

namespace Server
{
    class ServerHost
    {
        static void Main (string[] args)
        {
            IniConfigProvider provider = new IniConfigProvider();
            provider.Load("./settings.ini");
            Console.ReadKey();
        }

        //IniConfigProvider provider = new IniConfigProvider();
        //string s = string.Empty;
        //if(await provider.LoadAsync("./settings.ini") == 0)
        //{
        //    await provider.SetValueAsync("Scheme.color", "blue");
        //    await provider.SetValueAsync("Scheme.background-Image", "bg2.jpg");
        //    await provider.SetValueAsync("database.password", "123456");
        //    provider.TryGetValue("Scheme.color", ref s);
        //    provider.Dispose();
        //}
        //Console.WriteLine(s);
    }
}
