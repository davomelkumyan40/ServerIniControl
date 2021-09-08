using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IniManager
{
    public class IniConfigProvider : IIniProvider
    {
        private string path;
        private bool isNotLoaded = false;
        private StreamReader reader;
        private StreamWriter writer;
        public IniConfigProvider(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                isNotLoaded = true;
            else
            {
                this.path = path;
            }
        }

        public Dictionary<string, string> Data { get; private set; }

        public ushort Load()
        {
            if (isNotLoaded)
                return 255;
            try
            {
                using (reader = new StreamReader(path))
                {
                    var tempData = new Dictionary<string, string>();
                    string section = string.Empty;
                    while (reader?.Peek() != -1)
                    {
                        string raw = reader.ReadLine();
                        string normalizedLine = raw.Trim();
                        if (string.IsNullOrWhiteSpace(normalizedLine))
                            continue;
                        char firstChar = normalizedLine.FirstOrDefault();
                        char lastChar = normalizedLine.LastOrDefault();
                        if (firstChar == ';' || firstChar == '#' || firstChar == '/')
                            continue;
                        if (firstChar == '[' && lastChar == ']')
                        {
                            section = $"{normalizedLine.Substring(1, normalizedLine.Length - 2)}.";
                            continue;
                        }

                        if (normalizedLine.IndexOf("=") == -1)
                            return 1;

                        string[] keyValue = normalizedLine.Split('=');
                        string key = $"{section}{keyValue[0]}";
                        string value = $"{keyValue[1]}";
                        value = value.Replace("\"", "");
                        value = value.Replace("\'", "");
                        if (tempData.ContainsKey(key))
                            return 1;
                        tempData[key] = value;
                    }
                    Data = tempData;
                }
            }
            catch
            {
                return 255;
            }
            return 0;
        }

        public ushort SetValue(string key, string value)
        {
            if (isNotLoaded)
                return 4;
            try
            {
                string data = string.Empty;
                using (reader = new StreamReader(path))
                {
                    data = reader.ReadToEnd();
                }

                using (writer = new StreamWriter(path))
                {
                    string[] hierarchy = key.Split(".");
                    if (hierarchy.Length > 1)
                    {
                        int index = data.IndexOf($"[{hierarchy[0]}]");
                        if (index == -1)
                            writer.Write($"[{hierarchy[0]}]\n{hierarchy[1]}={value}\n{data}");
                        else
                        {
                            if (data.IndexOf(hierarchy[1]) == -1)
                            {
                                data = data.Insert(index + 2 + hierarchy[0].Length, $"\n{hierarchy[1]}={value}");
                                writer.Write(data);
                            }
                            else
                            {
                                Regex r = new Regex($"{hierarchy[1]}=[A-Z,a-z,1-9,.]*");
                                data = r.Replace(data, $"{hierarchy[1]}={value}");
                                writer.Write(data);
                            }
                        }
                        Data[key] = value;
                    }
                }
            }
            catch
            {
                return 255;
            }
            return 0;

        }

        public ushort TryGetValue(string key, ref string buffer)
        {
            if (isNotLoaded)
                return 4;
            if (!Data.ContainsKey(key))
                return 3;
            try
            {
                Data.TryGetValue(key, out buffer);
            }
            catch
            {
                return 255;
            }
            return 0;
        }

        public void Dispose()
        {
            reader?.Dispose();
            writer?.Dispose();
            Data = null;
        }
    }
}
