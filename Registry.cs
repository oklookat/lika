using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lika
{
    internal class Registry
    {
        // Converts .reg with variables to .reg, save .reg to temp.
        // Returns path to .reg file.
        public static string RegWithVarsToRegSaveToTemp(string srcRegPath)
        {
            var lines = File.ReadAllLines(srcRegPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0 || lines[i].StartsWith("["))
                {
                    continue;
                }
                KeyValue kv;
                try
                {
                    kv = new KeyValue(lines[i]);
                }
                catch
                {
                    continue;
                }
                kv.Value = Utils.Variables.Decode(kv.Value);
                if (!Utils.IsValidPath(kv.Value))
                {
                    continue;
                }
                lines[i] = kv.ToString().Replace(@"\", @"\\");
            }

            var regFilePath = Utils.CreateTempFile(".reg");
            File.WriteAllLines(regFilePath, lines);
            return regFilePath;
        }

        private class KeyValue
        {
            public string Key = "";

            private readonly bool isValueStr = false;
            public string Value = "";

            public KeyValue(string line)
            {
                var kv = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                if(kv.Length != 2)
                {
                    throw new ArgumentException("Not an key-value");
                }
                Key = kv[0].TrimStart('"').TrimEnd('"');

                isValueStr = kv[1].StartsWith('"');
                Value = kv[1].TrimStart('"').TrimEnd('"');
            }

            public override string ToString()
            {
                var convertedValue = Value;
                if(isValueStr)
                {
                    convertedValue = '"' + convertedValue + '"';
                }
                var convertedKey = '"' + Key + '"';
                return $"{convertedKey}={convertedValue}";
            }
        }
    }
}
