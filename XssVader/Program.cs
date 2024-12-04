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
            var wAFController = new WAFController(url);
            var messageController = new MessageController();

            var (isWAF, WAF) = wAFController.WAFDetection();

            if (isWAF)
            {
                messageController.ShowMessageMagenta($"[!] WAF Detected: {WAF}");
            }
            else
            {
                messageController.ShowMessageGreen("[+] No WAF detected.");
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
