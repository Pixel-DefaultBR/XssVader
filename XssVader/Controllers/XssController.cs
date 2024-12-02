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
        private readonly XssModel _xssReflected;
        private readonly XssModel _xssDOMBased;
        public XssController()
        {
            _xssReflected = new XssModel();
            _xssDOMBased = new XssModel();
        }

        public XssModel GetReflectedPayloads()
        {
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                .Parent
                .Parent
                .Parent
                .FullName;

            _xssReflected.Type = "REFLECETED";
            _xssReflected.Description = "Reflected XSS payloads";
            _xssReflected.Payloads = ReadPayloadFile(Path.Combine(projectDirectory, "Payloads", "p-reflected.txt"));

            return _xssReflected;
        } 
        public List<string> ReadPayloadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath).ToList();
            }
            else
            {
                throw new FileNotFoundException("O arquivo especificado não foi encontrado.", filePath);
            }
        }
    }
}
