using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal class MessageController
    {
        private const string Reset = "\u001b[0m";

        public void ShowMessageMagenta(string message)
        {
            Console.WriteLine($"\u001b[35m{message}{Reset}");
        }

        public void ShowMessageGreen(string message)
        {
            Console.WriteLine($"\u001b[32m{message}{Reset}");
        }

        public void ShowMessageBlue(string message)
        {
            Console.WriteLine($"\u001b[34m{message}{Reset}");
        }

        public void ShowMessageYellow(string message)
        {
            Console.WriteLine($"\u001b[33m{message}{Reset}");
        }

        public void ShowMessageCyan(string message)
        {
            Console.WriteLine($"\u001b[36m{message}{Reset}");
        }

        public void ShowMessageRed(string message)
        {
            Console.WriteLine($"\u001b[31m{message}{Reset}");
        }

        public void ShowMessageWhite(string message)
        {
            Console.WriteLine($"\u001b[37m{message}{Reset}");
        }

        public void ShowMessageBrightRed(string message)
        {
            Console.WriteLine($"\u001b[91m{message}{Reset}");
        }

        public void ShowMessageBrightGreen(string message)
        {
            Console.WriteLine($"\u001b[92m{message}{Reset}");
        }

        public void ShowMessageBrightBlue(string message)
        {
            Console.WriteLine($"\u001b[94m{message}{Reset}");
        }

        public void ShowMessageBrightYellow(string message)
        {
            Console.WriteLine($"\u001b[93m{message}{Reset}");
        }

        public void ShowMessageBrightCyan(string message)
        {
            Console.WriteLine($"\u001b[96m{message}{Reset}");
        }

        public void ShowMessageBrightMagenta(string message)
        {
            Console.WriteLine($"\u001b[95m{message}{Reset}");
        }
    }
}