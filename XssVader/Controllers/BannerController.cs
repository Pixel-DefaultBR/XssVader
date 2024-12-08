using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;

namespace XssVader.Controllers
{
    internal class BannerController
    {
        private readonly RequestController _requetController;
        private readonly Uri _uri;
        private readonly string _url;

        public BannerController(string url)
        {
            _url = url;
            _uri = new Uri(url);
            _requetController = new RequestController(url);
        }


        static public void ShowBanner()
        {
            Console.Write(@"
____  ___  _________ _________     .-.
\   \/  / /   _____//   _____/    |_:_|
 \     /  \_____  \ \_____  \    /(_Y_)\
 /     \  /        \/        \  ( \/M\/ )
/___/\  \/_______  /_______  / .'-/'-'\-'.
      \_/        \/        \/ 
|HH%%%%%:::::....._________________::::::|
");

            Console.WriteLine("XSS VADER - Cross-Site Scripting Scanner");
            Console.WriteLine();
        }

        public async void GetTargetBanner()
        {
            Task<string> headerTask = _requetController.GetHeader();
            string hostName = GetTargetHostname();
            string ipAddress = GetTargetIpAddress();

            string keyValueRegex = @"^([\w\-]+):\s*(.+)$";
            string header = await headerTask;

            var matches = Regex.Matches(header, keyValueRegex, RegexOptions.Multiline);

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"+ Target Hostname : \u001b[33m{hostName}\u001b[0m");
            Console.WriteLine($"+ Target IP       : \u001b[33m{ipAddress}\u001b[0m");
            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (key == "Server" || key == "Date" || key == "X-Powered-By")
                {
                    Console.WriteLine($"+ {key,-15} : \u001b[33m{value}\u001b[0m");
                }
            }
            Console.WriteLine("------------------------------------------------------------");
        }
        public string GetTargetHostname()
        {
            return _uri.Host.Trim('/');
        }
        public string GetTargetIpAddress()
        {
            string host = GetTargetHostname();
            return Dns.GetHostAddresses(host)[0].ToString();
        }
    }
}
