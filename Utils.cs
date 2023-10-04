using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lika
{
    internal class Utils
    {
        public static string NormalizePath(string path)
        {
            var normalized = new Uri(path).LocalPath;
            return Path.GetFullPath(normalized);
        }

        public static bool IsValidPath(string path)
        {
            try
            {
                NormalizePath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDirectory(string path)
        {
            var attr = File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory);
        }

        public static void ExecRegedit(string path)
        {
            string[] exec = { "regedit.exe", $"\"{path}\"" };
            Render.Str($"Exec: {String.Join(' ', exec)}");
            Process regeditProcess = Process.Start(exec[0], exec[1]);
            regeditProcess.WaitForExit();
        }

        // Creates temp file. Returns path to file.
        public static string CreateTempFile(string endOfFilename = "")
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + endOfFilename;
            using (File.Create(fileName))
            {
                return fileName;
            }
        }

        public static class Variables
        {
            public static readonly string CWDVar = "<CWD>";
            public static readonly string CWD = AppContext.BaseDirectory;

            public static readonly string SystemDirVar = "<SystemDir>";
            public static readonly string? SystemDir = Path.GetPathRoot(Environment.SystemDirectory);

            public static readonly string UserDocumentsVar = "<UserDocuments>";
            public static readonly string UserDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            public static readonly string PublicDocumentsVar = "<PublicDocuments>";
            public static readonly string PublicDocuments = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            public static readonly string ProgramFilesVar = "<ProgramFiles>";
            public static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            public static readonly string ProgramFiles86Var = "<ProgramFiles86>";
            public static readonly string ProgramFiles86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            public static readonly string CommonProgramFilesVar = "<CommonProgramFiles>";
            public static readonly string CommonProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

            public static readonly string CommonProgramFiles86Var = "<CommonProgramFiles86>";
            public static readonly string CommonProgramFiles86 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);

            public static readonly string CurrentUserVar = "<CurrentUser>";
            public static readonly string CurrentUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Example: <ProgramFiles>/hello = C:\Program Files\hello
            public static string Decode(string text, bool normalizePath = true)
            {
                if (SystemDir == null)
                {
                    throw new Exception("Null SystemDir");
                }

                if (text.Length == 0)
                {
                    return "";
                }

                var result = text.Replace(CWDVar, CWD);
                result = result.Replace(SystemDirVar, SystemDir);
                result = result.Replace(UserDocumentsVar, UserDocuments);
                result = result.Replace(PublicDocumentsVar, PublicDocuments);
                result = result.Replace(ProgramFilesVar, ProgramFiles);
                result = result.Replace(ProgramFiles86Var, ProgramFiles86);
                result = result.Replace(CommonProgramFilesVar, CommonProgramFiles);
                result = result.Replace(CommonProgramFiles86Var, CommonProgramFiles86);
                result = result.Replace(CurrentUserVar, CurrentUser);

                if (text == result)
                {
                    return text;
                }

                if (normalizePath)
                {
                    return NormalizePath(result);
                }
                return result;
            }

            // Example: <ProgramFiles>/hello = current_exe_dir\Program Files\hello
            public static string DecodeSandboxed(string text)
            {
                if (text.Length == 0)
                {
                    return "";
                }

                var result = text.Replace(CWDVar, CWD);
                result = result.Replace(SystemDirVar, $"{CWD}\\C");
                result = result.Replace(UserDocumentsVar, $"{CWD}\\C\\Users\\Current\\Documents");
                result = result.Replace(PublicDocumentsVar, $"{CWD}\\C\\Users\\Public\\Documents");
                result = result.Replace(ProgramFilesVar, $"{CWD}\\C\\Program Files");
                result = result.Replace(ProgramFiles86Var, $"{CWD}\\C\\Program Files (x86)");
                result = result.Replace(CommonProgramFilesVar, $"{CWD}\\C\\Program Files\\Common Files");
                result = result.Replace(CommonProgramFiles86Var, $"{CWD}\\C\\Program Files (x86)\\Common Files");
                result = result.Replace(CurrentUserVar, $"{CWD}\\C\\Users\\Current");

                return NormalizePath(result);
            }
        }
    }
}
