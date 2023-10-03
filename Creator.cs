using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static lika.Format;

namespace lika
{
    internal static class Creator
    {
        public static void Installer(Format.Data src, bool install = true)
        {
            // Create dirs.
            if (!install && src.Create != null)
            {
                foreach (var path in src.Create)
                {
                    if (path.Length > 0)
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }

            // Create links.
            var counter = 0;
            foreach (var link in src.Links)
            {
                counter++;
                Render.Str($"Current link: {link.Src} => {link.Target}");

                // File.
                if (!IsDirectory(link.Src))
                {
                    if (!File.Exists(link.Src))
                    {
                        if (!install)
                        {
                            throw new Exception("Not exists: " + link.Src);
                        }
                    }
                    if (!install)
                    {
                        File.Delete(link.Target);
                        continue;
                    }
                    File.CreateSymbolicLink(link.Target, link.Src);
                    continue;
                }

                // Dir.
                if (!Directory.Exists(link.Src))
                {
                    if (!install)
                    {
                        continue;
                    }
                    throw new Exception("Not exists: " + link.Src);
                }
                if (link.DirContents == null || link.DirContents == false)
                {
                    if (install)
                    {
                        Directory.CreateSymbolicLink(link.Target, link.Src);
                    }
                    else
                    {
                        if (Directory.Exists(link.Target))
                        {
                            Directory.Delete(link.Target, true);
                        }
                    }
                    continue;
                }

                var subDirs = Directory.GetDirectories(link.Src);
                var subFiles = Directory.GetFiles(link.Src);
                foreach (var subDirPath in subDirs)
                {
                    var subDirName = new DirectoryInfo(subDirPath).Name;
                    if (subDirName == null)
                    {
                        Render.Err("Empty sub dir name.");
                        continue;
                    }

                    if (link.DirContentsExcept != null && link.DirContentsExcept.Length > 0)
                    {
                        if (link.DirContentsExcept.Contains(subDirName))
                        {
                            continue;
                        }
                    }

                    var target = link.Target + "\\" + subDirName;
                    if (install)
                    {
                        Directory.CreateSymbolicLink(target, subDirPath);
                    }
                    else
                    {
                        if (Directory.Exists(target))
                        {
                            Directory.Delete(target, true);
                        }
                    }
                }
                foreach (var subFilePath in subFiles)
                {
                    var subFileName = Path.GetFileName(Path.GetDirectoryName(subFilePath));
                    var target = link.Target + "\\" + subFileName;
                    if (install)
                    {
                        File.CreateSymbolicLink(target, subFilePath);
                    }
                    else
                    {
                        if (File.Exists(target))
                        {
                            File.Delete(target);
                        }
                    }
                }
            }

            // Reg.
            if (src.Reg != null)
            {
                if (install && src.Reg.Install != null)
                {
                    ExecRegedit(src.Reg.Install);
                }
                else if (!install && src.Reg.Uninstall != null)
                {
                    ExecRegedit(src.Reg.Uninstall);
                }
            }
        }

        private static bool IsDirectory(string path)
        {
            var attr = File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory);
        }

        private static void ExecRegedit(string path)
        {
            string[] exec = { "regedit.exe", $"\"{path}\"" };
            Render.Str($"Exec: {String.Join(' ', exec)}");
            Process regeditProcess = Process.Start(exec[0], exec[1]);
            regeditProcess.WaitForExit();
        }
    }
}
