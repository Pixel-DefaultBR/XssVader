using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal static class BannerController
    {
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
    }
}
