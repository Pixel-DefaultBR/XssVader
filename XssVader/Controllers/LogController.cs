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
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?
                .Parent?
                .Parent?
                .Parent?
                .FullName;

            if (projectDirectory == null)
            {
                throw new InvalidOperationException("Logs could not be loaded.");
            }

            _logFilePath = Path.Combine(projectDirectory, "Logs", "log.txt");
            _logFilePath = Path.GetFullPath(_logFilePath);

            try
            {
                Directory.CreateDirectory(_logFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating log file: " + e.Message);
            }
            
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

