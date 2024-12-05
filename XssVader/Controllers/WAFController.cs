using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal class WAFController
    {
        private readonly MessageController _messageController;
        private readonly string _noise;
        private readonly string _path;
        private readonly string _url;
        private readonly RequestController _requestController;
        private readonly string _baseDirectory;
        public WAFController(string url)
        {
            _messageController = new MessageController();
            _noise = "\njaVasCript:/*-/*`/*\\`/*'/*\"/**/(/* */oNcliCk=alert() )//%0D%0A%0d%0a//</stYle/</titLe/</teXtarEa/</scRipt/--!>\\x3csVg/<sVg/oNloAd=alert()//>\\x3e\n";
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var parentDirectory = Directory.GetParent(_baseDirectory)?.Parent?.Parent?.Parent;

            if (parentDirectory == null)
            {
                throw new DirectoryNotFoundException("Parent directory not found.");
            }

            _path = Path.Combine(parentDirectory.FullName, "Database", "WAFSign.json");
            _url = url;
            _requestController = new RequestController($"{ _url }{ _noise}");
        }

        public bool WAFDetection()
        {
            bool isWAF = false;
            string WAF = string.Empty;
            string jsonContent = File.ReadAllText(_path);
            dynamic? wafSignatures = JsonConvert.DeserializeObject<dynamic>(jsonContent);

            if (wafSignatures == null)
            {
                throw new InvalidOperationException("WAF signatures could not be loaded.");
            }

            var header = _requestController.GetHeader().Result;

            _messageController.ShowMessageYellow($"[+] Searching for WAF's...");
            Thread.Sleep(new TimeSpan(0, 0, 10));

            foreach (var wafSignature in wafSignatures)
            {
                string headers = wafSignature.Value.headers.ToString();
                string code = wafSignature.Value.code.ToString();
                string page = wafSignature.Value.page.ToString();

                if ((header.Contains(headers) && headers != "")
                    || (header.Contains(code) && code != "")
                    || (header.Contains(page) && page != ""))
                {
                    isWAF = true;
                    WAF = wafSignature.Name;
                    break;
                }
            }

            if (isWAF)
            {
                _messageController.ShowMessageMagenta($"[!] WAF Detected: {WAF}");
                return isWAF;
            }
            else
            {
                _messageController.ShowMessageGreen("[+] No WAF detected.");
                return isWAF;
            }
        }
    }
}
