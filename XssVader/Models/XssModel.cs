using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XssVader.Models
{
    internal class XssModel
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public List<string> Payloads { get; set; }
    }
}
