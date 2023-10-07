using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static lika.Installer;

namespace lika
{
    internal static class Creator
    {
        public static void Installer(Data src, bool install = true)
        {
            // Create links.
            var counter = 0;
            foreach (var link in src.Links)
            {
                counter++;
                Render.Str($"Current link: {link.Src} => {link.Target}");

                // Create parent dirs if needed.
                var parentPath = Directory.GetParent(link.Target)?.FullName;
                if(parentPath != null)
                {
                    Directory.CreateDirectory(parentPath);
                }
   

                // Source = file.
                if (!Utils.IsDirectory(link.Src))
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

                // Source = dir.
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
                    else if (Directory.Exists(link.Target))
                    {
                        Directory.Delete(link.Target, true);
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

                    // Skip 'except' dir.
                    if (link.DirContentsExcept != null &&
                        link.DirContentsExcept.Length > 0 && link.DirContentsExcept.Contains(subDirName))
                    {
                        continue;
                    }

                    var target = link.Target + "\\" + subDirName;
                    if (install)
                    {
                        Directory.CreateSymbolicLink(target, subDirPath);
                    }
                    else if (Directory.Exists(target))
                    {
                        Directory.Delete(target, true);
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
                    else if (File.Exists(target))
                    {
                        File.Delete(target);
                    }
                }
            }

            // Reg.
            if (src.Reg != null)
            {
                if (install && src.Reg.Install != null)
                {
                    Utils.ExecRegedit(src.Reg.Install);
                }
                else if (!install && src.Reg.Uninstall != null)
                {
                    Utils.ExecRegedit(src.Reg.Uninstall);
                }
            }

            // Process.
            if (src.Process != null)
            {
                if (install && src.Process.Install != null)
                {
                    Utils.RunProcess(src.Process.Install);
                }
                else if (!install && src.Process.Uninstall != null)
                {
                    Utils.RunProcess(src.Process.Uninstall);
                }
            }
        }
    }
}
