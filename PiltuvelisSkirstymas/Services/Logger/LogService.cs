using Serilog;
using System;

namespace PiltuvelisSkirstymas.Services.Logger
{
    public class LogService : ILogService
    {
        private ILogger _logger;

        public LogService()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Async(conf => 
                    conf.File(
                        path: "Logs/log.txt", 
                        rollingInterval: RollingInterval.Day, 
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}]:[{Level:u3}]: {Message:lj}{NewLine}{Exception}"))
                .CreateLogger();
        }

        public void Information(string message) => _logger.Information(message);

        public void Information(string message, Exception ex) => _logger.Information(ex, message);

        public void Warning(string message) => _logger.Warning(message);
        public void Warning(string message, Exception ex) => _logger.Warning(ex, message);

        public void Error(string message) => _logger.Error(message);

        public void Error(string message, Exception ex) => _logger.Error(ex, message);
    }
}
