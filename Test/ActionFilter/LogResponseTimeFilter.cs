using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Test.API.ActionFilter
{
    public class LogResponseTimeFilter : IActionFilter
    {
        private readonly ILogger<LogResponseTimeFilter> _logger;
        private Stopwatch _stopwatch;

        public LogResponseTimeFilter(ILogger<LogResponseTimeFilter> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch.Start();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            var logMessage = $"Path: {context.HttpContext.Request.Path}, Method: {context.HttpContext.Request.Method}, Response Time: {elapsedMilliseconds} ms";
            LogToFile(logMessage);
        }

        private void LogToFile(string logMessage)
        {
            var logFilePath = "response_times.log";

            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging response time: {ex.Message}");
            }
        }
    }
}
