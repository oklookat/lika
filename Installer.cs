using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace lika
{
    internal class Installer
    {
        public static Data Decode(string absolutePath)
        {
            // Json to class.
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var data = JsonSerializer.Deserialize<Data>(File.ReadAllText(absolutePath), serializeOptions) ?? throw new Exception("Null data");

            // Links.
            for (int i = 0; i < data.Links.Length; i++)
            {
                if (data.Links[i].Src != null && data.Links[i].Src.Length > 0)
                {
                    data.Links[i].Src = Utils.Variables.Decode(data.Links[i].Src);
                }
                else
                {
                    data.Links[i].Src = Utils.Variables.DecodeSandboxed(data.Links[i].Target);
                }
                data.Links[i].Target = Utils.Variables.Decode(data.Links[i].Target);
            }

            // Reg.
            if (data.Reg != null)
            {
                if (data.Reg.Install != null)
                {
                    data.Reg.Install = Utils.Variables.Decode(data.Reg.Install);
                    if (data.Reg.InstallWithVars != null && data.Reg.InstallWithVars == true)
                    {
                        data.Reg.Install = Registry.RegWithVarsToRegSaveToTemp(data.Reg.Install);
                    }
                }
                if (data.Reg.Uninstall != null)
                {
                    data.Reg.Uninstall = Utils.Variables.Decode(data.Reg.Uninstall);
                    if (data.Reg.UninstallWithVars != null && data.Reg.UninstallWithVars == true)
                    {
                        data.Reg.Uninstall = Registry.RegWithVarsToRegSaveToTemp(data.Reg.Uninstall);
                    }
                }
            }

            // Process.
            if (data.Process != null)
            {
                if (data.Process.Install != null)
                {
                    data.Process.Install = Utils.Variables.Decode(data.Process.Install);
                }
                if (data.Process.Uninstall != null)
                {
                    data.Process.Uninstall = Utils.Variables.Decode(data.Process.Uninstall);
                }
            }

            return data;
        }

        public class Data
        {
            public string? InstallerName { get; set; }
            public string? Author { get; set; }
            public string? AuthorUrl { get; set; }

            // Create symlinks.
            public Link[] Links { get; set; } = Array.Empty<Link>();

            // Run reg file.
            public Reg? Reg { get; set; }

            // Run executable file.
            public Process? Process { get; set; }
        }

        public class Link
        {
            // Link source.
            // If null or empty, target will be used also as source.
            // See Variables.Decode & Variables.DecodeSandboxed.
            public string Src { get; set; } = "";


            public string Target { get; set; } = "";

            /** 
             * Example:
             * src = "C:\MyDir"
             * target = "D:\MyDir"
             * dirContents = true
             * 
             * Result (contents of target):
             * D:\MyDir\MySymlink1
             * D:\MyDir\MySymlink2
             * D:\MyDir\Etc
             * 
             * note: with dirContents you need
             * to create "MyDir" (see Data.Create)
             *
             */
            public bool? DirContents { get; set; }

            /** Exclude dir names from DirContents */
            public string[]? DirContentsExcept { get; set; }
        }

        public class Reg
        {
            public bool? InstallWithVars { get; set; } = false;
            public string? Install { get; set; }

            public bool? UninstallWithVars { get; set; } = false;
            public string? Uninstall { get; set; }
        }

        public class Process
        {
            public string? Install { get; set; }
            public string? Uninstall { get; set; }
        }
    }
}