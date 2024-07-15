using System;
using System.IO;
using System.Threading.Tasks;

namespace aairos.Services
{
    public class FileLoggerService
    {
        private readonly string _logFilePath;
        private readonly TimeSpan _logRetentionPeriod;

        public FileLoggerService(string logFilePath, TimeSpan logRetentionPeriod)
        {
            _logFilePath = logFilePath;
            _logRetentionPeriod = logRetentionPeriod;
        }

        public async Task LogAsync(string message)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                await writer.WriteLineAsync($"{DateTime.UtcNow}: {message}");
            }
        }

        public void CleanUpLogs()
        {
            var logFile = new FileInfo(_logFilePath);
            if (logFile.Exists && logFile.LastWriteTimeUtc < DateTime.UtcNow - _logRetentionPeriod)
            {
                logFile.Delete();
            }
        }
    }
}
