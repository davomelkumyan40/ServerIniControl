using System;
using System.Threading.Tasks;

namespace IniManager
{
    public interface IIniProvider : IDisposable
    {
        public ushort Load(string path);

        public ushort TryGetValue(string key, ref string buffer);

        public ushort SetValue(string key, string value);
    }
}
