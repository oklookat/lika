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
    internal class Format
    {
        public static class Decoder
        {
            public static Data Decode(string absolutePath)
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var data = JsonSerializer.Deserialize<Data>(File.ReadAllText(absolutePath), serializeOptions) ?? throw new Exception("Null data");

                // Create.
                for (int i = 0; i < data.Create.Length; i++)
                {
                    data.Create[i] = Variables.Decode(data.Create[i]);
                }

                // Links.
                for (int i = 0; i < data.Links.Length; i++)
                {
                    data.Links[i].Src = Variables.Decode(data.Links[i].Src);
                    data.Links[i].Target = Variables.Decode(data.Links[i].Target);
                }

                // Reg.
                if (data.Reg != null)
                {
                    if (data.Reg.Install != null)
                    {
                        data.Reg.Install = Variables.Decode(data.Reg.Install);
                    }
                    if (data.Reg.Uninstall != null)
                    {
                        data.Reg.Uninstall = Variables.Decode(data.Reg.Uninstall);
                    }
                }

                return data;
            }
        }

        public class Data
        {
            public string? InstallerName { get; set; }
            public string? Author { get; set; }
            public string? AuthorUrl { get; set; }


            // Create folders.
            public string[]? Create { get; set; }
            // Create symlinks.
            public Link[] Links { get; set; } = Array.Empty<Link>();
            // Run reg file.
            public Reg? Reg { get; set; }
        }

        public class Link
        {
            // Link source relative to current dir.
            public string Src { get; set; } = "";
            // Absolute path to target.
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
            public string? Install { get; set; }
            public string? Uninstall { get; set; }
        }

        public static class Variables
        {
            // Current bin dir.
            public static readonly string CWD = "<CWD>";
            // System directory (example: C:\).
            public static readonly string SystemDir = "<SystemDir>";
            // Current user documents.
            public static readonly string UserDocuments = "<UserDocuments>";
            // Public documents.
            public static readonly string PublicDocuments = "<PublicDocuments>";
            public static readonly string ProgramFiles = "<ProgramFiles>";
            public static readonly string ProgramFiles86 = "<ProgramFiles86>";
            public static readonly string CommonProgramFiles = "<CommonProgramFiles>";
            public static readonly string CurrentUser = "<CurrentUser>";

            public static string Decode(string source)
            {
                var result = source.Replace(CWD, AppContext.BaseDirectory);
                result = result.Replace(SystemDir, Path.GetPathRoot(Environment.SystemDirectory));
                result = result.Replace(UserDocuments, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                result = result.Replace(PublicDocuments, Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
                result = result.Replace(ProgramFiles, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                result = result.Replace(ProgramFiles86, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
                result = result.Replace(CommonProgramFiles, Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
                result = result.Replace(CurrentUser, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

                // Normalize.
                string normalizedPath = Path.GetFullPath(Path.Combine(result.Split(Path.GetInvalidPathChars())));
                return normalizedPath;
            }
        }
    }
}