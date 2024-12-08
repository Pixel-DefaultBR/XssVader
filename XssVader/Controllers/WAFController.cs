using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal class WAFController
    {
        private readonly MessageController _messageController;
        private readonly RequestController _requestController;
        private readonly string _noise;
        private readonly string _path;
        private readonly string _url;
        private readonly string _baseDirectory;

        public WAFController(string url)
        {
            _messageController = new MessageController();
            _noise = "\njaVasCript:/*-/*`/*\\`/*'/*\"/**/(/* */oNcliCk=alert() )//%0D%0A%0d%0a//</stYle/</titLe/</teXtarEa/</scRipt/--!>\\x3csVg/<sVg/oNloAd=alert()//>\\x3e\n";
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _path = GetWAFSignaturesPath();
            _url = url;
            _requestController = new RequestController($"{_url}{_noise}");
        }

        private string GetWAFSignaturesPath()
        {
            var parentDirectory = Directory.GetParent(_baseDirectory)?.Parent?.Parent?.Parent;

            if (parentDirectory == null)
            {
                throw new DirectoryNotFoundException("Parent directory not found.");
            }

            return Path.Combine(parentDirectory.FullName, "Database", "WAFSign.json");
        }

        public bool WAFDetection()
        {
            
            var header = GetRequestHeader();

            Thread.Sleep(new TimeSpan(0, 0, 10));

            (bool detectedWAF,string detectedWAFName) = DetectWAF(header);

            if (detectedWAF)
            {
                _messageController.ShowMessageMagenta($"! WAF Detected: {detectedWAFName}");
            }
            else
            {
                _messageController.ShowMessageGreen("+ No WAF detected.");
            }

            return detectedWAF;
        }

        private dynamic? LoadWAFSignatures()
        {
            string jsonContent = File.ReadAllText(_path);
            dynamic? wafSignatures = JsonConvert.DeserializeObject<dynamic>(jsonContent);

            if (wafSignatures == null)
            {
                throw new InvalidOperationException("WAF signatures could not be loaded.");
            }

            return wafSignatures;
        }

        private string GetRequestHeader()
        {
            return _requestController.GetHeader().Result;
        }

        private (bool isWAF, string WAF) DetectWAF(string header)
        {
            var wafSignatures = LoadWAFSignatures();

            foreach (var wafSignature in wafSignatures)
            {
                string headers = wafSignature.Value.headers.ToString();
                string code = wafSignature.Value.code.ToString();
                string page = wafSignature.Value.page.ToString();

                if ((header.Contains(headers) && !string.IsNullOrEmpty(headers))
                    || (header.Contains(code) && !string.IsNullOrEmpty(code))
                    || (header.Contains(page) && !string.IsNullOrEmpty(page)))
                {
                    return (true, wafSignature.Name.ToString());
                }
            }

            return (false, string.Empty);
        }
    }
}
