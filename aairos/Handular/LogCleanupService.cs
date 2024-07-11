using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace aairos.Handular
{
    public class LogCleanupService : IHostedService, IDisposable
    {
        private readonly ILogger<LogCleanupService> _logger;
        private Timer _timer;

        public LogCleanupService(ILogger<LogCleanupService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Log cleanup service starting");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
            if (Directory.Exists(logDirectory))
            {
                var files = Directory.GetFiles(logDirectory);
                foreach (var file in files)
                {
                    var creationTime = File.GetCreationTime(file);
                    if (creationTime < DateTime.Now.AddDays(-7))
                    {
                        File.Delete(file);
                        _logger.LogInformation($"Deleted old log file: {file}");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Log cleanup service stopping");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
