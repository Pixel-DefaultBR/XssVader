using System;
using System.Threading.Tasks;
using XssVader.Controllers;

class Program
{
    static async Task Main(string[] args)
    {
        BannerController.ShowBanner();

        Console.Write("[+] Target (https://www.example.com/?param=1): ");
        string url = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("[!] URL is required.");
            return;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) || (!url.StartsWith("http://") && !url.StartsWith("https://")))
        {
            Console.WriteLine("[!] Invalid URL format. Ensure it starts with http:// or https://");
            return;
        }

        if (!url.Contains("?"))
        {
            Console.WriteLine("[!] URL does not contain query string. (/cat.php?=1)");
            return;
        }

        try
        {
            var exploit = new ExploitController(url);
            var wafController = new WAFController(url);
            var messageController = new MessageController();

            bool isWAF = wafController.WAFDetection();

            if (isWAF)
            {
                messageController.ShowMessageMagenta("[!] Exiting...");
                return;
            }

            await exploit.ReflectedXss();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[!] An error occurred: {e.Message}");
            return;
        }
    }
}
