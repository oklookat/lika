using lika;
using System.Runtime.InteropServices;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        // Check OS.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Render.Str("Windows only.");
            Render.Idle();
            return;
        }

        var filePath = "installer.json";

        if (!File.Exists(filePath))
        {
            Render.Err($"{filePath} not found");
            Render.Idle();
            return;
        }

        // Decode.
        lika.Format.Data? decoded;
        try
        {
            decoded = lika.Format.Decoder.Decode(filePath);
            if (decoded == null)
            {
                Render.Err("invalid JSON file (?)");
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

        // Hello.
        Render.Str("lika v1.0");
        Render.Str("> https://github.com/oklookat/lika");
        Render.Str("> https://www.donationalerts.com/r/oklookat");
        Render.Str("> https://boosty.to/oklookat/donate");
        Render.Str("========\n");

        if (decoded.InstallerName != null)
        {
            Render.Str($"> Installer: {decoded.InstallerName}");
        }
        if (decoded.Author != null)
        {
            Render.Str($"> Author: {decoded.Author}");
        }
        if (decoded.AuthorUrl != null)
        {
            Render.Str($"> Author URL: {decoded.AuthorUrl}");
        }
        Render.Str("\n");

        // Choose.
        while (true)
        {
            Render.Str("Enter a digit:\n1. Install\n2. Uninstall\n");
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
                    Render.Str("Type '1' or '2'.");
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