using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            var doubleEncodedPayloads = DoubleHtmlEncodePayload(htmlEncodedPayloads);

            AddPayloadsToModel(_xssReflected, htmlEncodedPayloads);
            AddPayloadsToModel(_xssReflected, doubleEncodedPayloads);

            return _xssReflected;
        }

        private void AddPayloadsToModel(XssModel xssModel, List<string> payloads)
        {
            foreach (var payload in payloads)
            {
                xssModel.Payloads.Add(payload);
            }
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
            string[] tags = ReadFileLines($"{_projectDirectory}/Database/tags.txt");
            string[] events = ReadFileLines($"{_projectDirectory}/Database/events.txt");
            string[] openStructure = ReadFileLines($"{_projectDirectory}/Database/openStructure.txt");
            string[] payloadTemplates = ReadFileLines($"{_projectDirectory}/Database/payloadTemplates.txt");
            string[] payloads = ReadFileLines($"{_projectDirectory}/Database/payloads.txt");

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

        private string[] ReadFileLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public List<string> HtmlEncodePayload(XssModel xssModel)
        {
            _messageController.ShowMessageBlue("+ Encoding payloads to HTML...");
            Thread.Sleep(new TimeSpan(0, 0, 10));
            return xssModel.Payloads.Select(payload => Uri.EscapeDataString(payload)).ToList();
        }

        public List<string> DoubleHtmlEncodePayload(List<string> payloadList)
        {
            _messageController.ShowMessageBlue("+ Double Encoding payloads...");
            Thread.Sleep(new TimeSpan(0, 0, 10));
            return payloadList.Select(payload => Uri.EscapeDataString(payload)).ToList();
        }

        public List<string> DOMBasedXSS(string response)
        {
            var highlighted = new List<string>();

            string sources = @"(?:document\.(URL|documentURI|URLUnencoded|baseURI|cookie|referrer)|location\.(href|search|hash|pathname)|window\.name|history\.(pushState|replaceState)(local|session)Storage)";
            string sinks = @"(?:eval|evaluate|execCommand|assign|navigate|getResponseHeaderopen|showModalDialog|Function|set(Timeout|Interval|Immediate)|execScript|crypto.generateCRMFRequest|ScriptElement\.(src|text|textContent|innerText)|.*?\.onEventName|document\.(write|writeln)|.*?\.innerHTML|Range\.createContextualFragment|(document|window)\.location)";

            var scripts = Regex.Matches(response, "(?i)(?s)<script[^>]*>(.*?)</script>");
            bool sinkFound = false, sourceFound = false;

            foreach (Match scriptMatch in scripts)
            {
                var script = scriptMatch.Groups[1].Value.Split('\n');
                int num = 1;
                var allControlledVariables = new HashSet<string>();

                foreach (var newLine in script)
                {
                    string line = newLine;
                    var parts = line.Split(new[] { "var ", "let", "const" }, StringSplitOptions.None);
                    var controlledVariables = new HashSet<string>();

                    if (parts.Length > 1)
                    {
                        foreach (var part in parts)
                        {
                            foreach (var controlledVariable in allControlledVariables)
                            {
                                if (part.Contains(controlledVariable))
                                {
                                    var match = Regex.Match(part, "[a-zA-Z$_][a-zA-Z0-9$_]+");
                                    if (match.Success)
                                    {
                                        controlledVariables.Add(match.Value);
                                    }
                                }
                            }
                        }
                    }

                    HighlightSources(ref line, sources, parts, allControlledVariables, controlledVariables);
                    HighlightControlledVariables(ref line, allControlledVariables, ref sourceFound);
                    HighlightSinks(ref line, sinks, ref sinkFound);

                    if (!line.Equals(newLine))
                    {
                        highlighted.Add($"{num,-3} {line.TrimStart()}");
                    }

                    num++;
                }
            }

            return sinkFound || sourceFound ? highlighted : new List<string>();
        }

        private void HighlightSources(ref string line, string sources, string[] parts, HashSet<string> allControlledVariables, HashSet<string> controlledVariables)
        {
            foreach (Match grp in Regex.Matches(line, sources))
            {
                var source = line.Substring(grp.Index, grp.Length).Replace(" ", "");
                if (!string.IsNullOrEmpty(source))
                {
                    if (parts.Length > 1)
                    {
                        foreach (var part in parts)
                        {
                            if (part.Contains(source))
                            {
                                var match = Regex.Match(part, "[a-zA-Z$_][a-zA-Z0-9$_]+");
                                if (match.Success)
                                {
                                    controlledVariables.Add(match.Value);
                                }
                            }
                        }
                    }
                    line = line.Replace(source, $"\u001b[33m{source}\u001b[0m");
                }
            }

            foreach (var controlledVariable in controlledVariables)
            {
                allControlledVariables.Add(controlledVariable);
            }
        }

        private void HighlightControlledVariables(ref string line, HashSet<string> allControlledVariables, ref bool sourceFound)
        {
            foreach (var controlledVariable in allControlledVariables)
            {
                var matches = Regex.Matches(line, $"\\b{controlledVariable}\\b");
                if (matches.Count > 0)
                {
                    sourceFound = true;
                    line = Regex.Replace(line, $"\\b{controlledVariable}\\b", $"\u001b[33m{controlledVariable}\u001b[0m");
                }
            }
        }

        private void HighlightSinks(ref string line, string sinks, ref bool sinkFound)
        {
            foreach (Match grp in Regex.Matches(line, sinks))
            {
                var sink = line.Substring(grp.Index, grp.Length).Replace(" ", "");
                if (!string.IsNullOrEmpty(sink))
                {
                    line = line.Replace(sink, $"\u001b[31m{sink}\u001b[0m");
                    sinkFound = true;
                }
            }
        }
    }
}
