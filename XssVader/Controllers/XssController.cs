using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XssVader.Models;

namespace XssVader.Controllers
{
    internal class XssController
    {
        private readonly MessageController _messageController;
        private readonly XssModel _xssReflected;
        private readonly XssModel _xssDOMBased;
        private readonly string? _projectDirectory;

        public XssController()
        {
            _messageController = new MessageController();

            _xssReflected = new XssModel();
            _xssDOMBased = new XssModel();
            _projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?
                .Parent?
                .Parent?
                .Parent?
                .FullName;
        }
        public XssModel GetReflectedPayloads()
        {
            if (_projectDirectory == null)
            {
                throw new InvalidOperationException("Payloads could not be loaded.");
            }

            _xssReflected.Type = "Reflected";
            _xssReflected.Description = "Reflected XSS payloads";
            _xssReflected.Payloads = GenRandomPayloads();

            var htmlEncodedPayloads = HtmlEncodePayload(_xssReflected);

            foreach (var payload in htmlEncodedPayloads)
            {
                _xssReflected.Payloads.Add(payload);
            }

            return _xssReflected;
        }
        public List<string> ReadPayloadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath).ToList();
            }

            throw new FileNotFoundException("O arquivo especificado não foi encontrado.", filePath);
        }
        public List<string> GenRandomPayloads()
        {
            string[] tags = File.ReadAllLines($"{_projectDirectory}/Database/tags.txt");
            string[] events = File.ReadAllLines($"{_projectDirectory}/Database/events.txt");
            string[] openStructure = File.ReadAllLines($"{_projectDirectory}/Database/openStructure.txt");
            string[] payloadTemplates = File.ReadAllLines($"{_projectDirectory}/Database/payloadTemplates.txt");
            string[] payloads = File.ReadAllLines($"{_projectDirectory}/Database/payloads.txt");

            Random random = new Random();
            List<string> generatedPayloads = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                foreach (string template in payloadTemplates)
                {
                    string open = openStructure[random.Next(openStructure.Length)];
                    string tag = tags[random.Next(tags.Length)];
                    string evt = events[random.Next(events.Length)];
                    string pay = payloads[random.Next(payloads.Length)];

                    string payload = template
                        .Replace("{open}", open)
                        .Replace("{tag}", tag)
                        .Replace("{evt}", evt)
                        .Replace("{pay}", pay);

                     generatedPayloads.Add(payload);
                }
            }

            return generatedPayloads;
        } 
        public List<string> HtmlEncodePayload(XssModel xssModel)
        {
            _messageController.ShowMessageBlue("[+] Encoding payloads to HTML...");
            Thread.Sleep(new TimeSpan(0, 0, 10));
            var payloads = xssModel.Payloads.Select(payload => Uri.EscapeDataString(payload)).ToList();
            return payloads;
        }
    }
}
