using XssVader.Controllers;
using XssVader.Models;

class Program
{
    static async Task Main(string[] args)
    {
        ExploitController exploit = new ExploitController();
        await exploit.ReflectedXss();
    }
}