using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal class WAFController
    {
        private readonly string _noise;
        private readonly string _path;
        private readonly string _url;
        private readonly RequestController _requestController;
        public WAFController(string url)
        {
            _noise = "\njaVasCript:/*-/*`/*\\`/*'/*\"/**/(/* */oNcliCk=alert() )//%0D%0A%0d%0a//</stYle/</titLe/</teXtarEa/</scRipt/--!>\\x3csVg/<sVg/oNloAd=alert()//>\\x3e\n";
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var parentDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent;

            if (parentDirectory == null)
            {
                throw new DirectoryNotFoundException("Parent directory not found.");
            }

            _path = Path.Combine(parentDirectory.FullName, "Database", "WAFSign.json");
            _url = url;
            _requestController = new RequestController($"{ _url }{ _noise}");
        }

        public (bool isWAF, string WAF) WAFDetection()
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

            foreach (var wafSignature in wafSignatures)
            {
                string headers = wafSignature.Value.headers.ToString();
                string code = wafSignature.Value.code.ToString();
                string page = wafSignature.Value.page.ToString();

                if ((header.Contains(headers) && headers != "") || (header.Contains(code) && code != "") || (header.Contains(page) && page != ""))
                {
                    isWAF = true;
                    WAF = wafSignature.Name;
                    break;
                }
            }

            return (isWAF, WAF);
        }
    }
}
