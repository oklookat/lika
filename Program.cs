using lika;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        // Hello.
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        string version = fvi.FileVersion ?? "???";
        Render.StrWhiteBlack("lika symlink installer v" + version);
        Render.StrWhiteBlack("> https://github.com/oklookat/lika");
        Render.StrWhiteBlack("> https://www.donationalerts.com/r/oklookat");
        Render.StrWhiteBlack("> https://boosty.to/oklookat/donate");
        Render.Str("");

        // Check OS.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Render.Err("Windows only.");
            Render.Idle();
            return;
        }

        var installerPath = "installer.json";

        if (!File.Exists(installerPath))
        {
            Render.Err($"{installerPath} not found");
            Render.Idle();
            return;
        }

        // Decode.
        lika.Installer.Data? decoded;
        try
        {
            decoded = lika.Installer.Decode(installerPath);
            if (decoded == null)
            {
                Render.Err("decoded == null (wtf)");
                Render.Idle();
                return;
            }
        }
        catch (Exception ex)
        {
            Render.Str(ex.ToString());
            Render.Idle();
            return;
        }

        // Hello from installer.
        if (decoded.InstallerName != null)
        {
            Render.StrWhiteBlack($"> Installer: {decoded.InstallerName}");
        }
        if (decoded.Author != null)
        {
            Render.StrWhiteBlack($"> Author: {decoded.Author}");
        }
        if (decoded.AuthorUrl != null)
        {
            Render.StrWhiteBlack($"> Author URL: {decoded.AuthorUrl}");
        }
        Render.Str("");
        Render.Str("1. Install\n2. Uninstall\n");

        // Choose.
        while (true)
        {
            var pressed = Console.ReadKey(true);
            var chard = pressed.KeyChar;
            try
            {
                if (chard == '1')
                {
                    Render.Str("MODE: INSTALL");
                    lika.Creator.Installer(decoded);
                }
                else if (chard == '2')
                {
                    Render.Str("MODE: UNINSTALL");
                    lika.Creator.Installer(decoded, false);
                } else
                {
                    continue;  
                }
                Render.Str("Done.");
                break;
            }
            catch (Exception ex)
            {
                Render.Err(ex.ToString());
                break;
            }
        }

        Render.Idle();
    }
}