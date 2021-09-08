using System;

namespace IniManager
{
    public interface IIniProvider : IDisposable
    {
        public ushort Load();
        public ushort TryGetValue(string key, ref string buffer);
        public ushort SetValue(string key, string value);
    }
}
