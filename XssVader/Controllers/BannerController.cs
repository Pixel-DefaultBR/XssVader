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
        private readonly MessageController _messageController;

        public BannerController(string url)
        {
            _url = url;
            _uri = new Uri(url);
            _requetController = new RequestController(url);
            _messageController = new MessageController();
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

            BannerTamplate(hostName, ipAddress);
            BannerInfoTamplate(matches);
        }

        public void BannerTamplate(string hostname, string ip)
        {
            Console.WriteLine("------------------------------------------------------------");
            Console.Write("+ Target Hostname: ");
            _messageController.ShowMessageBrightBlue(hostname);
            Console.Write("+ Target IP: ");
            _messageController.ShowMessageBrightBlue(ip);
            Console.WriteLine("------------------------------------------------------------");
        }

        public void BannerInfoTamplate(dynamic matches)
        {
            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (key == "Server" || key == "Date" || key == "X-Powered-By")
                {
                    Console.Write($"+ {key}: ");
                    _messageController.ShowMessageBrightYellow($"{value}");
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
