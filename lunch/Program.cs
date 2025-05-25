using Microsoft.Win32;
using System.Diagnostics;
using System.Net;

class Program
{
    static void RegisterProtocol(string protocolName)
    {
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        string keyPath = $@"SOFTWARE\Classes\{protocolName}";

        using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
        {
            key.SetValue("", $"URL:{protocolName} Protocol");
            key.SetValue("URL Protocol", "");

            using (RegistryKey shell = key.CreateSubKey(@"shell\open\command"))
            {
                shell.SetValue("", $"\"{exePath}\" \"%1\"");
            }
        }
    }

    static void Main(string[] args)
    {        
        var dirPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        Directory.SetCurrentDirectory(dirPath);
        RegisterProtocol("lunch");

        if (args.Count() <= 0)
        {
            return;
        }

        var raw = args[0].Substring(args[0].IndexOf(":") + 1).Trim();
        var decoded = WebUtility.UrlDecode(raw).Trim();
        foreach (var line in decoded.Split('\n'))
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{line}\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            var process = Process.Start(psi)!;
            process.WaitForExit();
        }
    }
}
