using XssVader.Controllers;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "http://www.bancocn.com/cat.php?id=1";

        ExploitController exploit = new ExploitController(url);
        MessageController messageController = new MessageController();
        WAFController wAFController = new WAFController(url);

        var (isWAF, WAF) = wAFController.WAFDetection();

        if (isWAF)
        {
            Console.Write("[!] WAF Detected: ");
            messageController.ShowMessageCyan(WAF);
            Console.WriteLine();
        }
        else
        {
            messageController.ShowMessageMagenta("[+] No WAF detected.");
            Console.WriteLine();
        }

        await exploit.ReflectedXss();
    }
}