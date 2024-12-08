using System;
using System.Linq;
using System.Threading.Tasks;
using XssVader.Controllers;

class Program
{
    static async Task Main(string[] args)
    {
        BannerController.ShowBanner();

        string? url = null;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-u" || args[i] == "--url")
            {
                if (i + 1 < args.Length)
                {
                    url = args[i + 1];
                    break;
                }
            }
        }



        if (url == null || string.IsNullOrWhiteSpace(url))
        {
            Console.Write("[+] Target (https://www.example.com/?param=1): ");
            url = Console.ReadLine() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("! Usage: XssVader.exe -u <target_url>");
            return;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) || (!url.StartsWith("http://") && !url.StartsWith("https://")))
        {
            Console.WriteLine("! Invalid URL format. Ensure it starts with http:// or https://");
            return;
        }

        if (!url.Contains("?"))
        {
            Console.WriteLine("! URL does not contain query string. (/cat.php?=1)");
            return;
        }

        BannerController getTargetBanner = new BannerController(url);
        getTargetBanner.GetTargetBanner();

        try
        {
            var exploit = new ExploitController(url);
            var wafController = new WAFController(url);
            var messageController = new MessageController();

            bool detectedWaf = wafController.WAFDetection();

            if (detectedWaf)
            {
                messageController.ShowMessageMagenta("! Exiting...");
                return;
            }

            await exploit.DOMXSS();
            await exploit.ReflectedXss();
        }
        catch (Exception e)
        {
            Console.WriteLine($"! An error occurred: {e.Message}");
            return;
        }
    }
}
