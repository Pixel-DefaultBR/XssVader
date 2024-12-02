using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XssVader.Controllers
{
    internal class LogController
    {
        private readonly string _logFilePath;

        public LogController()
        {
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                .Parent
                .Parent
                .Parent
                .FullName;

            _logFilePath = Path.Combine(projectDirectory, "Logs", "log.txt");

            // Ensure the Logs directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        }

        public void Log(string message)
        {
            LogMessage("INFO", message);
        }

        public void LogWarning(string message)
        {
            LogMessage("WARNING", message);
        }

        public void LogError(string message)
        {
            LogMessage("ERROR", message);
        }

        private void LogMessage(string level, string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
    }
}

